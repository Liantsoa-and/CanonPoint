namespace CanonPoint.App.Models.Dtos;

public class CreatePlayerRequestDto
{
    public string? Name { get; set; }
    public string? ColorHex { get; set; }
}

public class PlayerResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ColorHex { get; set; }
}
