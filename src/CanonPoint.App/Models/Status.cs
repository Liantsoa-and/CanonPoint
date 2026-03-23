namespace CanonPoint.App.Models;

public class Status
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
