using CanonPoint.App.Domain.Enums;

namespace CanonPoint.App.Domain.Models;

public sealed class GameState
{
    public GameState(int gameId, int rows, int cols)
    {
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Rows doit etre superieur a 0.");
        }

        if (cols <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cols), "Cols doit etre superieur a 0.");
        }

        GameId = gameId;
        Rows = rows;
        Cols = cols;
        Status = GameStatus.InProgress;
        CurrentPlayer = PlayerSide.Player1;
        Cells = new Dictionary<BoardPosition, CellState>((rows + 1) * (cols + 1));
        CountedLineHashes = new HashSet<string>(StringComparer.Ordinal);

        // Points are placed on intersections, so indexes go from 0..Rows and 0..Cols.
        for (var row = 0; row <= rows; row++)
        {
            for (var col = 0; col <= cols; col++)
            {
                var position = new BoardPosition(row, col);
                Cells[position] = new CellState(row, col);
            }
        }
    }

    public int GameId { get; }
    public int Rows { get; }
    public int Cols { get; }
    public int IntersectionRows => Rows + 1;
    public int IntersectionCols => Cols + 1;

    public GameStatus Status { get; set; }
    public PlayerSide CurrentPlayer { get; set; }
    public int ScorePlayer1 { get; set; }
    public int ScorePlayer2 { get; set; }

    public int LeftCanonRow { get; private set; }
    public int RightCanonRow { get; private set; }

    public Dictionary<BoardPosition, CellState> Cells { get; }
    public HashSet<string> CountedLineHashes { get; }

    public bool IsIntersectionInBounds(int row, int col)
    {
        return row >= 0 && row < IntersectionRows && col >= 0 && col < IntersectionCols;
    }

    public CellState GetCell(int row, int col)
    {
        EnsurePositionInBounds(row, col);
        return Cells[new BoardPosition(row, col)];
    }

    public void SetCanonRows(int leftCanonRow, int rightCanonRow)
    {
        EnsureRowInBounds(leftCanonRow);
        EnsureRowInBounds(rightCanonRow);

        LeftCanonRow = leftCanonRow;
        RightCanonRow = rightCanonRow;
    }

    public void EnsurePositionInBounds(int row, int col)
    {
        BoardPosition.EnsureInBounds(row, col, IntersectionRows, IntersectionCols);
    }

    private void EnsureRowInBounds(int row)
    {
        if (row < 0 || row >= IntersectionRows)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "La ligne du canon est hors limites.");
        }
    }
}
