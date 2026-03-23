namespace CanonPoint.App.Models;

public class Game
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int StatusId { get; set; }
    public Status Status { get; set; } = null!;

    public int GridRows { get; set; } = 20;
    public int GridCols { get; set; } = 20;

    public ICollection<Point> Points { get; set; } = new List<Point>();
    public ICollection<Shot> Shots { get; set; } = new List<Shot>();
    public ICollection<Move> Moves { get; set; } = new List<Move>();
}
