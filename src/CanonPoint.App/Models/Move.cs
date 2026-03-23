namespace CanonPoint.App.Models;

public class Move
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int SequenceNumber { get; set; }

    public int? PointId { get; set; }
    public Point? Point { get; set; }

    public int? ShotId { get; set; }
    public Shot? Shot { get; set; }

    public bool IsShot { get; set; } = false;
}
