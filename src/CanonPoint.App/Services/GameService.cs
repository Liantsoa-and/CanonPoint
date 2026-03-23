using CanonPoint.App.Data;
using CanonPoint.App.Models;
using CanonPoint.App.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CanonPoint.App.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _dbContext;

    public GameService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GameResponseDto> CreateGameAsync(CreateGameRequestDto request, CancellationToken cancellationToken = default)
    {
        var statusId = request.StatusId ?? await EnsureDefaultStatusAsync(cancellationToken);

        var game = new Game
        {
            Name = request.Name,
            StatusId = statusId,
            GridRows = request.GridRows,
            GridCols = request.GridCols
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GameResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            StatusId = game.StatusId,
            GridRows = game.GridRows,
            GridCols = game.GridCols
        };
    }

    public async Task<IReadOnlyList<GameResponseDto>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .OrderBy(g => g.Id)
            .Select(g => new GameResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                StatusId = g.StatusId,
                GridRows = g.GridRows,
                GridCols = g.GridCols
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<GameStateResponseDto?> GetGameStateAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var game = await _dbContext.Games
            .AsNoTracking()
            .Include(g => g.Points)
            .Include(g => g.Shots)
            .Include(g => g.Moves)
            .FirstOrDefaultAsync(g => g.Id == gameId, cancellationToken);

        if (game is null)
        {
            return null;
        }

        return new GameStateResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            StatusId = game.StatusId,
            GridRows = game.GridRows,
            GridCols = game.GridCols,
            Points = game.Points
                .OrderBy(p => p.Id)
                .Select(p => new PointResponseDto
                {
                    Id = p.Id,
                    OwnerId = p.OwnerId,
                    Row = p.Row,
                    Col = p.Col,
                    IsInvulnerable = p.IsInvulnerable
                })
                .ToList(),
            Shots = game.Shots
                .OrderBy(s => s.Id)
                .Select(s => new ShotResponseDto
                {
                    Id = s.Id,
                    PlayerId = s.PlayerId,
                    TargetRow = s.TargetRow,
                    TargetCol = s.TargetCol,
                    Power = s.Power
                })
                .ToList(),
            Moves = game.Moves
                .OrderBy(m => m.SequenceNumber)
                .Select(m => new MoveResponseDto
                {
                    Id = m.Id,
                    PlayerId = m.PlayerId,
                    SequenceNumber = m.SequenceNumber,
                    IsShot = m.IsShot,
                    PointId = m.PointId,
                    ShotId = m.ShotId
                })
                .ToList()
        };
    }

    public async Task<MoveResponseDto?> AddPointAsync(int gameId, AddPointRequestDto request, CancellationToken cancellationToken = default)
    {
        var gameExists = await _dbContext.Games.AnyAsync(g => g.Id == gameId, cancellationToken);
        if (!gameExists)
        {
            return null;
        }

        var playerExists = await _dbContext.Players.AnyAsync(p => p.Id == request.PlayerId, cancellationToken);
        if (!playerExists)
        {
            return null;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var point = new Point
        {
            GameId = gameId,
            OwnerId = request.PlayerId,
            Row = request.Row,
            Col = request.Col,
            IsInvulnerable = request.IsInvulnerable
        };

        _dbContext.Points.Add(point);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var nextSequence = await GetNextSequenceNumberAsync(gameId, cancellationToken);
        var move = new Move
        {
            GameId = gameId,
            PlayerId = request.PlayerId,
            SequenceNumber = nextSequence,
            PointId = point.Id,
            IsShot = false
        };

        _dbContext.Moves.Add(move);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new MoveResponseDto
        {
            Id = move.Id,
            PlayerId = move.PlayerId,
            SequenceNumber = move.SequenceNumber,
            IsShot = move.IsShot,
            PointId = move.PointId,
            ShotId = move.ShotId
        };
    }

    public async Task<MoveResponseDto?> FireShotAsync(int gameId, FireShotRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Power < 1 || request.Power > 9)
        {
            return null;
        }

        var gameExists = await _dbContext.Games.AnyAsync(g => g.Id == gameId, cancellationToken);
        if (!gameExists)
        {
            return null;
        }

        var playerExists = await _dbContext.Players.AnyAsync(p => p.Id == request.PlayerId, cancellationToken);
        if (!playerExists)
        {
            return null;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var shot = new Shot
        {
            GameId = gameId,
            PlayerId = request.PlayerId,
            TargetRow = request.TargetRow,
            TargetCol = request.TargetCol,
            Power = request.Power
        };

        _dbContext.Shots.Add(shot);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var nextSequence = await GetNextSequenceNumberAsync(gameId, cancellationToken);
        var move = new Move
        {
            GameId = gameId,
            PlayerId = request.PlayerId,
            SequenceNumber = nextSequence,
            ShotId = shot.Id,
            IsShot = true
        };

        _dbContext.Moves.Add(move);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new MoveResponseDto
        {
            Id = move.Id,
            PlayerId = move.PlayerId,
            SequenceNumber = move.SequenceNumber,
            IsShot = move.IsShot,
            PointId = move.PointId,
            ShotId = move.ShotId
        };
    }

    public async Task<PlayerResponseDto> CreatePlayerAsync(CreatePlayerRequestDto request, CancellationToken cancellationToken = default)
    {
        var player = new Player
        {
            Name = request.Name,
            ColorHex = request.ColorHex
        };

        _dbContext.Players.Add(player);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PlayerResponseDto
        {
            Id = player.Id,
            Name = player.Name,
            ColorHex = player.ColorHex
        };
    }

    public async Task<IReadOnlyList<PlayerResponseDto>> GetPlayersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Players
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new PlayerResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                ColorHex = p.ColorHex
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<int> GetNextSequenceNumberAsync(int gameId, CancellationToken cancellationToken)
    {
        var maxSequence = await _dbContext.Moves
            .Where(m => m.GameId == gameId)
            .Select(m => (int?)m.SequenceNumber)
            .MaxAsync(cancellationToken);

        return (maxSequence ?? 0) + 1;
    }

    private async Task<int> EnsureDefaultStatusAsync(CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Statuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == "En cours", cancellationToken);

        if (existing is not null)
        {
            return existing.Id;
        }

        var status = new Status
        {
            Name = "En cours"
        };

        _dbContext.Statuses.Add(status);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return status.Id;
    }
}
