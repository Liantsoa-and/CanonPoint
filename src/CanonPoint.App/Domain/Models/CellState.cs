using CanonPoint.App.Domain.Enums;

namespace CanonPoint.App.Domain.Models;

public sealed class CellState
{
    // Row/Col represent an intersection coordinate of the board grid.
    public CellState(int row, int col, PlayerSide? owner = null, bool isInvulnerable = false)
    {
        if (isInvulnerable && (!owner.HasValue || owner.Value == PlayerSide.None))
        {
            throw new ArgumentException("Une cellule invulnerable doit avoir un proprietaire.", nameof(isInvulnerable));
        }

        Row = row;
        Col = col;
        Owner = owner;
        IsInvulnerable = isInvulnerable;
    }

    public int Row { get; }
    public int Col { get; }
    public PlayerSide? Owner { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public void SetOwner(PlayerSide owner)
    {
        if (owner == PlayerSide.None)
        {
            throw new ArgumentException("Le proprietaire doit etre Player1 ou Player2.", nameof(owner));
        }

        Owner = owner;
    }

    public bool IsEmpty() => !Owner.HasValue;

    public void SetInvulnerable(bool isInvulnerable)
    {
        if (isInvulnerable && (!Owner.HasValue || Owner.Value == PlayerSide.None))
        {
            throw new InvalidOperationException("Impossible de rendre invulnerable une cellule sans proprietaire.");
        }

        IsInvulnerable = isInvulnerable;
    }
}
