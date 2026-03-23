using CanonPoint.App.Domain.Enums;

namespace CanonPoint.App.Domain.Models;

public sealed class TurnCommand
{
    public TurnCommand(MoveType moveType, PlayerSide player, int row, int col, int? power = null)
    {
        if (player == PlayerSide.None)
        {
            throw new ArgumentException("Le joueur de commande doit etre Player1 ou Player2.", nameof(player));
        }

        MoveType = moveType;
        Player = player;
        Row = row;
        Col = col;
        Power = power;
    }

    public MoveType MoveType { get; }
    public PlayerSide Player { get; }
    public int Row { get; }
    public int Col { get; }
    public int? Power { get; }
}
