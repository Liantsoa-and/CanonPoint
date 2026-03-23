namespace CanonPoint.App.Models;

public class Point
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int OwnerId { get; set; }
    public Player Owner { get; set; } = null!;

    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsInvulnerable { get; set; } = false;

    public ICollection<Move> Moves { get; set; } = new List<Move>();
}
