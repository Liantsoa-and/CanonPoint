using CanonPoint.App.Data;
using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Services;
using CanonPoint.App.Models;
using CanonPoint.App.Models.Dtos;
using CanonPoint.App.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CanonPoint.App.Tests.Integration;

public class GameServiceIntegrationTests
{
    [Fact]
    public async Task FullFlow_PersistsMoves_WithStrictlyIncreasingSequence()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var service = new GameService(db);
        var p1 = await service.CreatePlayerAsync(new CreatePlayerRequestDto { Name = "P1", ColorHex = "#AA0000" });
        var p2 = await service.CreatePlayerAsync(new CreatePlayerRequestDto { Name = "P2", ColorHex = "#00AA00" });
        var game = await service.CreateGameAsync(new CreateGameRequestDto { Name = "Integration", GridRows = 6, GridCols = 6 });

        var m1 = await service.AddPointAsync(game.Id, new AddPointRequestDto { PlayerId = p1.Id, Row = 1, Col = 1, IsInvulnerable = false });
        var m2 = await service.AddPointAsync(game.Id, new AddPointRequestDto { PlayerId = p2.Id, Row = 1, Col = 2, IsInvulnerable = false });
        var m3 = await service.FireShotAsync(game.Id, new FireShotRequestDto { PlayerId = p1.Id, TargetRow = 4, TargetCol = 4, Power = 2 });

        Assert.NotNull(m1);
        Assert.NotNull(m2);
        Assert.NotNull(m3);
        Assert.Equal(1, m1!.SequenceNumber);
        Assert.Equal(2, m2!.SequenceNumber);
        Assert.Equal(3, m3!.SequenceNumber);

        var state = await service.GetGameStateAsync(game.Id);
        Assert.NotNull(state);
        Assert.Equal([1, 2, 3], state!.Moves.Select(m => m.SequenceNumber).ToArray());
    }

    [Fact]
    public async Task Reconstruction_FromPersistedHistory_RebuildsEquivalentState()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var service = new GameService(db);
        var p1 = await service.CreatePlayerAsync(new CreatePlayerRequestDto { Name = "Alice", ColorHex = "#AA0000" });
        var p2 = await service.CreatePlayerAsync(new CreatePlayerRequestDto { Name = "Bob", ColorHex = "#00AA00" });
        var game = await service.CreateGameAsync(new CreateGameRequestDto { Name = "Replay", GridRows = 6, GridCols = 6 });

        await service.AddPointAsync(game.Id, new AddPointRequestDto { PlayerId = p1.Id, Row = 1, Col = 1, IsInvulnerable = false });
        await service.AddPointAsync(game.Id, new AddPointRequestDto { PlayerId = p2.Id, Row = 2, Col = 2, IsInvulnerable = false });
        await service.FireShotAsync(game.Id, new FireShotRequestDto { PlayerId = p1.Id, TargetRow = 4, TargetCol = 4, Power = 2 });

        var persisted = await service.GetGameStateAsync(game.Id);
        Assert.NotNull(persisted);

        var turnService = new TurnService(
            new PlacementService(),
            new LineDetectionService(),
            new LineValidationService(),
            new ShotService());

        var reconstruction = new GameStateReconstructionService(new GameStateFactory(), turnService);

        var pointById = persisted!.Points.ToDictionary(p => p.Id);
        var shotById = persisted.Shots.ToDictionary(s => s.Id);

        var moveData = persisted.Moves
            .OrderBy(m => m.SequenceNumber)
            .Select(m =>
            {
                var playerSide = m.PlayerId == p1.Id ? PlayerSide.Player1 : PlayerSide.Player2;

                if (!m.IsShot)
                {
                    var point = pointById[m.PointId!.Value];
                    return new MoveData(m.SequenceNumber, playerSide, MoveType.PlacePoint, point.Row, point.Col, null, DateTime.UtcNow);
                }

                var shot = shotById[m.ShotId!.Value];
                return new MoveData(m.SequenceNumber, playerSide, MoveType.FireShot, shot.TargetRow, shot.TargetCol, shot.Power, DateTime.UtcNow);
            })
            .ToList();

        var rebuilt = reconstruction.LoadGameStateFromHistory(game.Id, persisted.GridRows, persisted.GridCols, moveData);

        var persistedSet = persisted.Points
            .Select(p => $"{p.Row}:{p.Col}:{p.OwnerId}:{p.IsInvulnerable}")
            .OrderBy(x => x)
            .ToArray();

        var rebuiltSet = rebuilt.Cells.Values
            .Where(c => c.Owner.HasValue)
            .Select(c =>
            {
                var ownerId = c.Owner == PlayerSide.Player1 ? p1.Id : p2.Id;
                return $"{c.Row}:{c.Col}:{ownerId}:{c.IsInvulnerable}";
            })
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(persistedSet, rebuiltSet);
    }

    [Fact]
    public async Task TransactionFailure_RollsBackChanges_WhenUniqueSequenceIsViolated()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var setupDb = new AppDbContext(options))
        {
            await setupDb.Database.EnsureCreatedAsync();

            var status = new Status { Name = "En cours" };
            setupDb.Statuses.Add(status);
            await setupDb.SaveChangesAsync();

            var game = new Game { Name = "Tx", StatusId = status.Id, GridRows = 6, GridCols = 6 };
            var player = new Player { Name = "TxPlayer", ColorHex = "#123456" };
            setupDb.Games.Add(game);
            setupDb.Players.Add(player);
            await setupDb.SaveChangesAsync();
        }

        await using var db = new AppDbContext(options);
        await using var tx = await db.Database.BeginTransactionAsync();

        db.Points.Add(new Point
        {
            GameId = 1,
            OwnerId = 1,
            Row = 0,
            Col = 0,
            IsInvulnerable = false
        });

        db.Moves.Add(new Move { GameId = 1, PlayerId = 1, SequenceNumber = 1, IsShot = false });
        db.Moves.Add(new Move { GameId = 1, PlayerId = 1, SequenceNumber = 1, IsShot = false });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        await tx.RollbackAsync();

        await using var verifyDb = new AppDbContext(options);
        var pointsInDb = await verifyDb.Points.CountAsync(p => p.GameId == 1);
        Assert.Equal(0, pointsInDb);
    }
}
