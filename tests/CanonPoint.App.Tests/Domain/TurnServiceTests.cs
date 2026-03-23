using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class TurnServiceTests
{
    private static ITurnService BuildService()
    {
        return new TurnService(
            new PlacementService(),
            new LineDetectionService(),
            new LineValidationService(),
            new ShotService());
    }

    [Fact]
    public void PlayTurn_WithPlacement_UsesUnifiedPipelineAndSwitchesPlayer()
    {
        var factory = new GameStateFactory();
        var turnService = BuildService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        var command = new TurnCommand(MoveType.PlacePoint, PlayerSide.Player1, row: 2, col: 2);
        var result = turnService.PlayTurn(gameState, command);

        Assert.True(result.Success);
        Assert.NotNull(result.Move);
        Assert.Equal(MoveType.PlacePoint, result.Move!.Type);
        Assert.Equal(1, result.Move.SequenceNumber);
        Assert.Equal(PlayerSide.Player1, gameState.GetCell(2, 2).Owner);
        Assert.Equal(PlayerSide.Player2, gameState.CurrentPlayer);
        Assert.Equal(2, gameState.NextSequenceNumber);
    }

    [Fact]
    public void PlayTurn_WithShot_UsesUnifiedPipelineAndSwitchesPlayer()
    {
        var factory = new GameStateFactory();
        var turnService = BuildService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        gameState.CurrentPlayer = PlayerSide.Player2;
        gameState.GetCell(4, 6).SetOwner(PlayerSide.Player1);

        var command = new TurnCommand(MoveType.FireShot, PlayerSide.Player2, row: 4, col: 0, power: 1);
        var result = turnService.PlayTurn(gameState, command);

        Assert.True(result.Success);
        Assert.NotNull(result.Move);
        Assert.Equal(MoveType.FireShot, result.Move!.Type);
        Assert.Equal(1, result.Move.SequenceNumber);
        Assert.Null(gameState.GetCell(4, 6).Owner);
        Assert.Equal(PlayerSide.Player1, gameState.CurrentPlayer);
        Assert.Equal(2, gameState.NextSequenceNumber);
    }

    [Fact]
    public void PlayTurn_WhenActionInvalid_DoesNotChangeState()
    {
        var factory = new GameStateFactory();
        var turnService = BuildService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        var initialPlayer = gameState.CurrentPlayer;
        var initialStatus = gameState.Status;
        var initialNextSequence = gameState.NextSequenceNumber;

        var command = new TurnCommand(MoveType.PlacePoint, PlayerSide.Player2, row: 2, col: 2);
        var result = turnService.PlayTurn(gameState, command);

        Assert.False(result.Success);
        Assert.Null(result.Move);
        Assert.True(gameState.GetCell(2, 2).IsEmpty());
        Assert.Equal(initialPlayer, gameState.CurrentPlayer);
        Assert.Equal(initialStatus, gameState.Status);
        Assert.Equal(initialNextSequence, gameState.NextSequenceNumber);
        Assert.Equal(0, gameState.ScorePlayer1);
        Assert.Equal(0, gameState.ScorePlayer2);
    }
}
