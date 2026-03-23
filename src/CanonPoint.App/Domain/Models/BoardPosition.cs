namespace CanonPoint.App.Domain.Models;

public readonly record struct BoardPosition(int Row, int Col)
{
    public static void EnsureInBounds(int row, int col, int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Rows et Cols doivent etre superieurs a 0.");
        }

        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "Position hors limites de la grille.");
        }
    }
}
