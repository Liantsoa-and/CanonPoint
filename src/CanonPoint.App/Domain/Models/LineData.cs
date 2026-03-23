using CanonPoint.App.Domain.Enums;

namespace CanonPoint.App.Domain.Models;

public sealed class LineData
{
    public LineData(PlayerSide owner, IReadOnlyList<BoardPosition> cells, string hash)
    {
        if (owner == PlayerSide.None)
        {
            throw new ArgumentException("Le proprietaire de ligne doit etre Player1 ou Player2.", nameof(owner));
        }

        if (cells is null || cells.Count != 5)
        {
            throw new ArgumentException("Une ligne valide doit contenir exactement 5 cellules.", nameof(cells));
        }

        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Le hash de ligne est obligatoire.", nameof(hash));
        }

        Owner = owner;
        Cells = cells;
        Hash = hash;
    }

    public PlayerSide Owner { get; }
    public IReadOnlyList<BoardPosition> Cells { get; }
    public string Hash { get; }
}
