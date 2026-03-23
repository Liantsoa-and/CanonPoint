using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public sealed class LineValidationService : ILineValidationService
{
    public IReadOnlyList<LineData> GetValidatedLines(GameState gameState, IReadOnlyList<LineData> candidateLines, PlayerSide player)
    {
        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        if (candidateLines is null)
        {
            throw new ArgumentNullException(nameof(candidateLines));
        }

        if (player == PlayerSide.None)
        {
            throw new ArgumentException("Le joueur doit etre Player1 ou Player2.", nameof(player));
        }

        var validated = new List<LineData>();

        foreach (var line in candidateLines)
        {
            if (line.Owner != player)
            {
                continue;
            }

            if (gameState.CountedLineHashes.Contains(line.Hash))
            {
                continue;
            }

            if (!AllCellsBelongToPlayer(gameState, line, player))
            {
                continue;
            }

            MarkCellsInvulnerable(gameState, line);
            gameState.CountedLineHashes.Add(line.Hash);
            IncrementScore(gameState, player);
            validated.Add(line);
        }

        return validated;
    }

    private static bool AllCellsBelongToPlayer(GameState gameState, LineData line, PlayerSide player)
    {
        foreach (var position in line.Cells)
        {
            if (!gameState.IsIntersectionInBounds(position.Row, position.Col))
            {
                return false;
            }

            var cell = gameState.GetCell(position.Row, position.Col);
            if (cell.Owner != player)
            {
                return false;
            }
        }

        return true;
    }

    private static void MarkCellsInvulnerable(GameState gameState, LineData line)
    {
        foreach (var position in line.Cells)
        {
            var cell = gameState.GetCell(position.Row, position.Col);
            cell.SetInvulnerable(true);
        }
    }

    private static void IncrementScore(GameState gameState, PlayerSide player)
    {
        if (player == PlayerSide.Player1)
        {
            gameState.ScorePlayer1 += 1;
            return;
        }

        gameState.ScorePlayer2 += 1;
    }
}
