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
                    dRow, dCol, dir, state.Rows - 1, state.Columns - 1, state.BlockedCells, state.ValidatedLines);

                if (line != null && IsNewLine(line, state.ValidatedLines))
                    result.Add(line);
            }

            return result;
        }

        private ValidatedLine? CheckDirection(List<Point> points, int row, int col,
            int playerId, int dRow, int dCol, string dir, int maxRow, int maxCol,
            List<BlockedCell> blockedCells, List<ValidatedLine> validatedLines)
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

            int adversaire = playerId == 1 ? 2 : 1;

            // Une sequence > 5 peut contenir plusieurs lignes de 5.
            // On garde la premiere fenetre valide qui inclut le dernier point joue.
            for (int offset = 0; offset <= count - 5; offset++)
            {
                var candidate = new ValidatedLine
                {
                    PlayerId = playerId,
                    Direction = dir,
                    StartRow = startRow + offset * dRow,
                    StartCol = startCol + offset * dCol,
                    EndRow = startRow + (offset + 4) * dRow,
                    EndCol = startCol + (offset + 4) * dCol
                };

                var candidateCells = candidate.GetCells().ToList();

                // La ligne detectee doit etre celle creee par le coup courant.
                if (!candidateCells.Contains((row, col)))
                    continue;

                bool blockedByOpponent = candidateCells.Any(cell =>
                    blockedCells.Any(b =>
                        b.Row == cell.row &&
                        b.Col == cell.col &&
                        b.BlockingPlayerId != playerId));

                if (blockedByOpponent)
                    continue;

                bool crossesOpponentLine = validatedLines.Any(vline =>
                    vline.PlayerId == adversaire &&
                    SegmentsIntersect(candidate.StartCol, candidate.StartRow, candidate.EndCol, candidate.EndRow,
                        vline.StartCol, vline.StartRow, vline.EndCol, vline.EndRow));

                if (crossesOpponentLine)
                    continue;

                return candidate;
            }

            return null;
        }

        private static bool SegmentsIntersect(int ax, int ay, int bx, int by,
            int cx, int cy, int dx, int dy)
        {
            long o1 = Orientation(ax, ay, bx, by, cx, cy);
            long o2 = Orientation(ax, ay, bx, by, dx, dy);
            long o3 = Orientation(cx, cy, dx, dy, ax, ay);
            long o4 = Orientation(cx, cy, dx, dy, bx, by);

            if (o1 == 0 && OnSegment(ax, ay, bx, by, cx, cy)) return true;
            if (o2 == 0 && OnSegment(ax, ay, bx, by, dx, dy)) return true;
            if (o3 == 0 && OnSegment(cx, cy, dx, dy, ax, ay)) return true;
            if (o4 == 0 && OnSegment(cx, cy, dx, dy, bx, by)) return true;

            return ((o1 > 0 && o2 < 0) || (o1 < 0 && o2 > 0)) &&
                ((o3 > 0 && o4 < 0) || (o3 < 0 && o4 > 0));
        }

        private static long Orientation(int ax, int ay, int bx, int by, int cx, int cy)
        {
            return (long)(bx - ax) * (cy - ay) - (long)(by - ay) * (cx - ax);
        }

        private static bool OnSegment(int ax, int ay, int bx, int by, int px, int py)
        {
            return px >= Math.Min(ax, bx) && px <= Math.Max(ax, bx) &&
                py >= Math.Min(ay, by) && py <= Math.Max(ay, by);
        }

        public bool IsNewLine(ValidatedLine line, List<ValidatedLine> existing)
        {
            // Autoriser un recouvrement d'une seule case (extrémité partagée),
            // mais refuser les recouvrements plus larges qui représentent la même ligne.
            var lineCells = line.GetCells().ToHashSet();

            return !existing.Any(e =>
            {
                if (e.PlayerId != line.PlayerId || e.Direction != line.Direction)
                    return false;

                int overlap = e.GetCells().Count(c => lineCells.Contains(c));
                return overlap >= 2;
            });
        }
    }
}
