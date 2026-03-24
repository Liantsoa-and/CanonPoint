using JeuDePoints.Domain.Models;

namespace JeuDePoints.Services
{
    public class AlignmentChecker
    {
        private static readonly (int dRow, int dCol, string dir)[] Directions =
        {
            (0,  1, "horizontal"),
            (1,  0, "vertical"),
            (-1, 1, "diagonal_asc"),
            (1,  1, "diagonal_desc")
        };

        public List<ValidatedLine> CheckAlignments(GameState state, int lastRow, int lastCol, int playerId)
        {
            var result = new List<ValidatedLine>();
            var points = state.Points
                .Where(p => p.PlayerId == playerId && p.IsActive)
                .ToList();

            foreach (var (dRow, dCol, dir) in Directions)
            {
                var line = CheckDirection(points, lastRow, lastCol, playerId,
                    dRow, dCol, dir, state.Rows - 1, state.Columns - 1, state.BlockedCells);

                if (line != null && IsNewLine(line, state.ValidatedLines))
                    result.Add(line);
            }

            return result;
        }

        private ValidatedLine? CheckDirection(List<Point> points, int row, int col,
            int playerId, int dRow, int dCol, string dir, int maxRow, int maxCol,
            List<BlockedCell> blockedCells)
        {
            var set = points.Select(p => (p.Row, p.Col)).ToHashSet();

            int startRow = row, startCol = col;
            while (true)
            {
                int nr = startRow - dRow, nc = startCol - dCol;
                if (nr < 0 || nc < 0 || nr > maxRow || nc > maxCol) break;
                if (!set.Contains((nr, nc))) break;
                startRow = nr;
                startCol = nc;
            }

            int count = 0;
            int r = startRow, c = startCol;
            while (r >= 0 && c >= 0 && r <= maxRow && c <= maxCol && set.Contains((r, c)))
            {
                count++;
                r += dRow;
                c += dCol;
            }

            if (count < 5) return null;

            var candidate = new ValidatedLine
            {
                PlayerId = playerId,
                Direction = dir,
                StartRow = startRow,
                StartCol = startCol,
                EndRow = startRow + 4 * dRow,
                EndCol = startCol + 4 * dCol
            };

            // Regle gardee: rejeter la ligne si une de ses 5 cases est protegee par l'adversaire.
            bool blockedByOpponent = candidate.GetCells().Any(cell =>
                blockedCells.Any(b =>
                    b.Row == cell.row &&
                    b.Col == cell.col &&
                    b.BlockingPlayerId != playerId));

            return blockedByOpponent ? null : candidate;
        }

        public bool IsNewLine(ValidatedLine line, List<ValidatedLine> existing)
        {
            // Si une ligne du même joueur et même direction partage au moins une case,
            // on considère que c'est la même ligne (extension) → pas de nouveau point.
            return !existing.Any(e =>
                e.PlayerId == line.PlayerId &&
                e.Direction == line.Direction &&
                e.GetCells().Any(c => line.GetCells().Contains(c)));
        }
    }
}
