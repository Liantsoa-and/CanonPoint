using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class LineDetectionServiceTests
{
    [Fact]
    public void FindCandidateLines_FindsHorizontalLineOfFive()
    {
        var factory = new GameStateFactory();
        var detection = new LineDetectionService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        for (var col = 1; col <= 5; col++)
        {
            gameState.GetCell(3, col).SetOwner(PlayerSide.Player1);
        }

        var lines = detection.FindCandidateLines(gameState, lastRow: 3, lastCol: 3, PlayerSide.Player1);

        Assert.Single(lines);
        Assert.Equal(5, lines[0].Cells.Count);
        Assert.Equal("3:1|3:2|3:3|3:4|3:5", lines[0].Hash);
    }

    [Fact]
    public void FindCandidateLines_WhenPlacementCreatesTwoLines_ReturnsTwoDistinctLines()
    {
        var factory = new GameStateFactory();
        var detection = new LineDetectionService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        for (var col = 1; col <= 5; col++)
        {
            gameState.GetCell(3, col).SetOwner(PlayerSide.Player1);
        }

        for (var row = 1; row <= 5; row++)
        {
            gameState.GetCell(row, 3).SetOwner(PlayerSide.Player1);
        }

        var lines = detection.FindCandidateLines(gameState, lastRow: 3, lastCol: 3, PlayerSide.Player1);

        Assert.Equal(2, lines.Count);
        Assert.Equal(2, lines.Select(l => l.Hash).Distinct().Count());
        Assert.Contains(lines, l => l.Hash == "3:1|3:2|3:3|3:4|3:5");
        Assert.Contains(lines, l => l.Hash == "1:3|2:3|3:3|4:3|5:3");
    }

    [Fact]
    public void FindCandidateLines_WhenLastPointIsNotOwnedByPlayer_ReturnsEmpty()
    {
        var factory = new GameStateFactory();
        var detection = new LineDetectionService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        gameState.GetCell(3, 3).SetOwner(PlayerSide.Player2);

        var lines = detection.FindCandidateLines(gameState, lastRow: 3, lastCol: 3, PlayerSide.Player1);

        Assert.Empty(lines);
    }
}
