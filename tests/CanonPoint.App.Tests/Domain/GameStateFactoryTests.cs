using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class GameStateFactoryTests
{
    [Fact]
    public void CreateNewGameState_WithSameInput_ReturnsSameInitialState()
    {
        var factory = new GameStateFactory();

        var first = factory.CreateNewGameState(gameId: 1, rows: 20, cols: 20);
        var second = factory.CreateNewGameState(gameId: 1, rows: 20, cols: 20);

        Assert.Equal(1, first.GameId);
        Assert.Equal(1, second.GameId);
        Assert.Equal(first.Rows, second.Rows);
        Assert.Equal(first.Cols, second.Cols);

        Assert.Equal(GameStatus.InProgress, first.Status);
        Assert.Equal(GameStatus.InProgress, second.Status);
        Assert.Equal(PlayerSide.Player1, first.CurrentPlayer);
        Assert.Equal(PlayerSide.Player1, second.CurrentPlayer);

        Assert.Equal(0, first.ScorePlayer1);
        Assert.Equal(0, first.ScorePlayer2);
        Assert.Equal(0, second.ScorePlayer1);
        Assert.Equal(0, second.ScorePlayer2);

        Assert.Equal(0, first.LeftCanonRow);
        Assert.Equal(0, first.RightCanonRow);
        Assert.Equal(0, second.LeftCanonRow);
        Assert.Equal(0, second.RightCanonRow);

        Assert.Equal(first.Cells.Count, second.Cells.Count);
        Assert.Equal(400, first.Cells.Count);

        Assert.All(first.Cells.Values, cell =>
        {
            Assert.Null(cell.Owner);
            Assert.False(cell.IsInvulnerable);
        });

        Assert.All(second.Cells.Values, cell =>
        {
            Assert.Null(cell.Owner);
            Assert.False(cell.IsInvulnerable);
        });
    }
}
