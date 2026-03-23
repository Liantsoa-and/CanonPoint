using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class PlacementServiceTests
{
    [Fact]
    public void TryPlacePoint_WhenOutOfBounds_ReturnsError()
    {
        var factory = new GameStateFactory();
        var placement = new PlacementService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 5, cols: 5);

        var result = placement.TryPlacePoint(gameState, PlayerSide.Player1, row: 6, col: 1);

        Assert.False(result.Success);
        Assert.Equal("Croisement hors limites de la grille.", result.ErrorMessage);
    }

    [Fact]
    public void TryPlacePoint_WhenOccupied_ReturnsError()
    {
        var factory = new GameStateFactory();
        var placement = new PlacementService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 5, cols: 5);

        var first = placement.TryPlacePoint(gameState, PlayerSide.Player1, row: 2, col: 2);
        Assert.True(first.Success);

        gameState.CurrentPlayer = PlayerSide.Player2;
        var second = placement.TryPlacePoint(gameState, PlayerSide.Player2, row: 2, col: 2);

        Assert.False(second.Success);
        Assert.Equal("Ce croisement est deja occupe.", second.ErrorMessage);
    }

    [Fact]
    public void TryPlacePoint_WhenEmpty_ReturnsSuccessAndPlacesPoint()
    {
        var factory = new GameStateFactory();
        var placement = new PlacementService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 5, cols: 5);

        var result = placement.TryPlacePoint(gameState, PlayerSide.Player1, row: 1, col: 3);

        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);
        Assert.NotNull(result.Position);
        Assert.Equal(1, result.Position!.Value.Row);
        Assert.Equal(3, result.Position!.Value.Col);

        var cell = gameState.GetCell(1, 3);
        Assert.Equal(PlayerSide.Player1, cell.Owner);
    }
}
