using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Services;
using Xunit;

namespace CanonPoint.App.Tests.Domain;

public class LineValidationServiceTests
{
    [Fact]
    public void GetValidatedLines_WhenNewLine_GrantsScoreAndSetsInvulnerable()
    {
        var factory = new GameStateFactory();
        var detection = new LineDetectionService();
        var validation = new LineValidationService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        for (var col = 1; col <= 5; col++)
        {
            gameState.GetCell(3, col).SetOwner(PlayerSide.Player1);
        }

        var candidates = detection.FindCandidateLines(gameState, lastRow: 3, lastCol: 3, PlayerSide.Player1);
        var validated = validation.GetValidatedLines(gameState, candidates, PlayerSide.Player1);

        Assert.Single(validated);
        Assert.Equal(1, gameState.ScorePlayer1);
        Assert.Equal(0, gameState.ScorePlayer2);
        Assert.Contains(validated[0].Hash, gameState.CountedLineHashes);

        foreach (var position in validated[0].Cells)
        {
            Assert.True(gameState.GetCell(position.Row, position.Col).IsInvulnerable);
        }
    }

    [Fact]
    public void GetValidatedLines_WhenSameLineProvidedAgain_DoesNotRecount()
    {
        var factory = new GameStateFactory();
        var detection = new LineDetectionService();
        var validation = new LineValidationService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        for (var col = 1; col <= 5; col++)
        {
            gameState.GetCell(3, col).SetOwner(PlayerSide.Player1);
        }

        var candidates = detection.FindCandidateLines(gameState, lastRow: 3, lastCol: 3, PlayerSide.Player1);

        var firstValidated = validation.GetValidatedLines(gameState, candidates, PlayerSide.Player1);
        var secondValidated = validation.GetValidatedLines(gameState, candidates, PlayerSide.Player1);

        Assert.Single(firstValidated);
        Assert.Empty(secondValidated);
        Assert.Equal(1, gameState.ScorePlayer1);
    }

    [Fact]
    public void GetValidatedLines_WhenTwoDistinctLines_GrantsTwoPoints()
    {
        var factory = new GameStateFactory();
        var detection = new LineDetectionService();
        var validation = new LineValidationService();
        var gameState = factory.CreateNewGameState(gameId: 1, rows: 6, cols: 6);

        for (var col = 1; col <= 5; col++)
        {
            gameState.GetCell(3, col).SetOwner(PlayerSide.Player1);
        }

        for (var row = 1; row <= 5; row++)
        {
            gameState.GetCell(row, 3).SetOwner(PlayerSide.Player1);
        }

        var candidates = detection.FindCandidateLines(gameState, lastRow: 3, lastCol: 3, PlayerSide.Player1);
        var validated = validation.GetValidatedLines(gameState, candidates, PlayerSide.Player1);

        Assert.Equal(2, validated.Count);
        Assert.Equal(2, gameState.ScorePlayer1);
        Assert.Equal(0, gameState.ScorePlayer2);
    }
}
