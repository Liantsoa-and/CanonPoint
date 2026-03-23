namespace CanonPoint.App.Models;

public class Shot
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int TargetRow { get; set; }
    public int TargetCol { get; set; }
    public int Power { get; set; }

    public ICollection<Move> Moves { get; set; } = new List<Move>();
}
