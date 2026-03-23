using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class GameStateReconstructionServiceTests
{
    [Fact]
    public void LoadGameStateFromHistory_RebuildsSameStateAsOriginalPlay()
    {
        var factory = new GameStateFactory();
        var turnService = BuildTurnService();
        var reconstruction = new GameStateReconstructionService(factory, turnService);

        var original = factory.CreateNewGameState(gameId: 7, rows: 6, cols: 6);
        var history = new List<MoveData>();

        history.Add(PlayAndGetMove(turnService, original, new TurnCommand(MoveType.PlacePoint, PlayerSide.Player1, row: 2, col: 2)));
        history.Add(PlayAndGetMove(turnService, original, new TurnCommand(MoveType.PlacePoint, PlayerSide.Player2, row: 2, col: 1)));
        history.Add(PlayAndGetMove(turnService, original, new TurnCommand(MoveType.FireShot, PlayerSide.Player1, row: 2, col: 0, power: 2)));
        history.Add(PlayAndGetMove(turnService, original, new TurnCommand(MoveType.PlacePoint, PlayerSide.Player2, row: 0, col: 0)));

        var rebuilt = reconstruction.LoadGameStateFromHistory(gameId: 7, rows: 6, cols: 6, history);

        AssertEquivalentStates(original, rebuilt);
    }

    [Fact]
    public void LoadGameStateFromHistory_SameHistoryTwice_IsDeterministic()
    {
        var factory = new GameStateFactory();
        var turnService = BuildTurnService();
        var reconstruction = new GameStateReconstructionService(factory, turnService);

        var history = new List<MoveData>
        {
            new(1, PlayerSide.Player1, MoveType.PlacePoint, row: 1, col: 1, power: null, createdAtUtc: DateTime.UtcNow),
            new(2, PlayerSide.Player2, MoveType.PlacePoint, row: 1, col: 2, power: null, createdAtUtc: DateTime.UtcNow),
            new(3, PlayerSide.Player1, MoveType.FireShot, row: 1, col: 0, power: 3, createdAtUtc: DateTime.UtcNow)
        };

        var first = reconstruction.LoadGameStateFromHistory(gameId: 10, rows: 6, cols: 6, history);
        var second = reconstruction.LoadGameStateFromHistory(gameId: 10, rows: 6, cols: 6, history);

        AssertEquivalentStates(first, second);
    }

    private static ITurnService BuildTurnService()
    {
        return new TurnService(
            new PlacementService(),
            new LineDetectionService(),
            new LineValidationService(),
            new ShotService());
    }

    private static MoveData PlayAndGetMove(ITurnService turnService, GameState gameState, TurnCommand command)
    {
        var result = turnService.PlayTurn(gameState, command);
        Assert.True(result.Success);
        Assert.NotNull(result.Move);
        return result.Move!;
    }

    private static void AssertEquivalentStates(GameState expected, GameState actual)
    {
        Assert.Equal(expected.GameId, actual.GameId);
        Assert.Equal(expected.Rows, actual.Rows);
        Assert.Equal(expected.Cols, actual.Cols);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Equal(expected.CurrentPlayer, actual.CurrentPlayer);
        Assert.Equal(expected.ScorePlayer1, actual.ScorePlayer1);
        Assert.Equal(expected.ScorePlayer2, actual.ScorePlayer2);
        Assert.Equal(expected.LeftCanonRow, actual.LeftCanonRow);
        Assert.Equal(expected.RightCanonRow, actual.RightCanonRow);
        Assert.Equal(expected.NextSequenceNumber, actual.NextSequenceNumber);
        Assert.Equal(expected.CountedLineHashes.OrderBy(x => x), actual.CountedLineHashes.OrderBy(x => x));
        Assert.Equal(expected.Cells.Count, actual.Cells.Count);

        foreach (var kvp in expected.Cells)
        {
            var other = actual.Cells[kvp.Key];
            Assert.Equal(kvp.Value.Owner, other.Owner);
            Assert.Equal(kvp.Value.IsInvulnerable, other.IsInvulnerable);
        }
    }
}
