using CanonPoint.App.Domain.Enums;

namespace CanonPoint.App.Domain.Models;

public sealed class MoveData
{
    public MoveData(int sequenceNumber, PlayerSide player, MoveType type, int row, int col, int? power, DateTime createdAtUtc)
    {
        if (sequenceNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequenceNumber), "SequenceNumber doit etre strictement superieur a 0.");
        }

        if (player == PlayerSide.None)
        {
            throw new ArgumentException("Le joueur d'un move doit etre Player1 ou Player2.", nameof(player));
        }

        SequenceNumber = sequenceNumber;
        Player = player;
        Type = type;
        Row = row;
        Col = col;
        Power = power;
        CreatedAtUtc = createdAtUtc;
    }

    public int SequenceNumber { get; }
    public PlayerSide Player { get; }
    public MoveType Type { get; }
    public int Row { get; }
    public int Col { get; }
    public int? Power { get; }
    public DateTime CreatedAtUtc { get; }
}
