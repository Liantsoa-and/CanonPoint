namespace CanonPoint.App.Models;

public class Player
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ColorHex { get; set; }

    public ICollection<Point> Points { get; set; } = new List<Point>();
    public ICollection<Shot> Shots { get; set; } = new List<Shot>();
    public ICollection<Move> Moves { get; set; } = new List<Move>();
}
