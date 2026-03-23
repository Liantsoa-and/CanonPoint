using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class ShotServiceTests
{
    [Fact]
    public void TryFire_WhenTargetIsEmpty_DoesNothing()
    {
        var factory = new GameStateFactory();
        var shot = new ShotService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        gameState.CurrentPlayer = PlayerSide.Player1;
        var result = shot.TryFire(gameState, PlayerSide.Player1, cannonRow: 3, power: 2);

        Assert.True(result.Success);
        Assert.False(result.WasPointDestroyed);
        Assert.NotNull(result.Target);
        Assert.True(gameState.GetCell(3, 1).IsEmpty());
        Assert.Equal(0, gameState.ScorePlayer1);
        Assert.Equal(0, gameState.ScorePlayer2);
    }

    [Fact]
    public void TryFire_WhenTargetIsOwnPoint_DoesNothing()
    {
        var factory = new GameStateFactory();
        var shot = new ShotService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        gameState.GetCell(2, 0).SetOwner(PlayerSide.Player1);

        var result = shot.TryFire(gameState, PlayerSide.Player1, cannonRow: 2, power: 1);

        Assert.True(result.Success);
        Assert.False(result.WasPointDestroyed);
        Assert.Equal(PlayerSide.Player1, gameState.GetCell(2, 0).Owner);
        Assert.Equal(0, gameState.ScorePlayer1);
    }

    [Fact]
    public void TryFire_WhenTargetIsAdverseInvulnerable_DoesNothing()
    {
        var factory = new GameStateFactory();
        var shot = new ShotService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        gameState.GetCell(4, 0).SetOwner(PlayerSide.Player2);
        gameState.GetCell(4, 0).SetInvulnerable(true);

        var result = shot.TryFire(gameState, PlayerSide.Player1, cannonRow: 4, power: 1);

        Assert.True(result.Success);
        Assert.False(result.WasPointDestroyed);
        Assert.Equal(PlayerSide.Player2, gameState.GetCell(4, 0).Owner);
        Assert.True(gameState.GetCell(4, 0).IsInvulnerable);
        Assert.Equal(0, gameState.ScorePlayer1);
    }

    [Fact]
    public void TryFire_WhenTargetIsAdverseNonInvulnerable_DestroysPoint()
    {
        var factory = new GameStateFactory();
        var shot = new ShotService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        gameState.GetCell(1, 0).SetOwner(PlayerSide.Player2);

        var result = shot.TryFire(gameState, PlayerSide.Player1, cannonRow: 1, power: 1);

        Assert.True(result.Success);
        Assert.True(result.WasPointDestroyed);
        Assert.Null(gameState.GetCell(1, 0).Owner);
        Assert.False(gameState.GetCell(1, 0).IsInvulnerable);
        Assert.Equal(0, gameState.ScorePlayer1);
        Assert.Equal(0, gameState.ScorePlayer2);
    }

    [Fact]
    public void TryFire_WhenPowerIsInvalid_ReturnsError()
    {
        var factory = new GameStateFactory();
        var shot = new ShotService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        var result = shot.TryFire(gameState, PlayerSide.Player1, cannonRow: 0, power: 10);

        Assert.False(result.Success);
        Assert.Equal("La puissance doit etre comprise entre 1 et 9.", result.ErrorMessage);
    }
}
