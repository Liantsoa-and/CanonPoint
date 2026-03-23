using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public sealed class LineDetectionService : ILineDetectionService
{
    private static readonly (int dRow, int dCol)[] Directions =
    [
        (0, 1),   // Horizontal
        (1, 0),   // Vertical
        (1, 1),   // Diagonale descendante
        (1, -1),   // Diagonale montante
        (-1, 1),   // Diagonale descendante inverse
        (-1, -1)   // Diagonale montante inverse

    ];

    public IReadOnlyList<LineData> FindCandidateLines(GameState gameState, int lastRow, int lastCol, PlayerSide player)
    {
        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        if (player == PlayerSide.None)
        {
            throw new ArgumentException("Le joueur doit etre Player1 ou Player2.", nameof(player));
        }

        if (!gameState.IsIntersectionInBounds(lastRow, lastCol))
        {
            return [];
        }

        var lastCell = gameState.GetCell(lastRow, lastCol);
        if (lastCell.Owner != player)
        {
            return [];
        }

        var byHash = new Dictionary<string, LineData>(StringComparer.Ordinal);

        foreach (var (dRow, dCol) in Directions)
        {
            for (var startOffset = -4; startOffset <= 0; startOffset++)
            {
                var cells = TryBuildFiveCellWindow(gameState, lastRow, lastCol, dRow, dCol, startOffset, player);
                if (cells is null)
                {
                    continue;
                }

                var hash = ComputeCanonicalHash(cells);
                if (!byHash.ContainsKey(hash))
                {
                    byHash[hash] = new LineData(player, cells, hash);
                }
            }
        }

        return byHash.Values.ToList();
    }

    private static List<BoardPosition>? TryBuildFiveCellWindow(
        GameState gameState,
        int lastRow,
        int lastCol,
        int dRow,
        int dCol,
        int startOffset,
        PlayerSide player)
    {
        var window = new List<BoardPosition>(5);

        for (var i = 0; i < 5; i++)
        {
            var row = lastRow + (startOffset + i) * dRow;
            var col = lastCol + (startOffset + i) * dCol;

            if (!gameState.IsIntersectionInBounds(row, col))
            {
                return null;
            }

            var cell = gameState.GetCell(row, col);
            if (cell.Owner != player)
            {
                return null;
            }

            window.Add(new BoardPosition(row, col));
        }

        return window;
    }

    private static string ComputeCanonicalHash(IReadOnlyList<BoardPosition> cells)
    {
        var ordered = cells
            .OrderBy(p => p.Row)
            .ThenBy(p => p.Col)
            .Select(p => $"{p.Row}:{p.Col}");

        return string.Join("|", ordered);
    }
}
