namespace CanonPoint.App.Models.Dtos;

public class CreateGameRequestDto
{
    public string? Name { get; set; }
    public int? StatusId { get; set; }
    public int GridRows { get; set; } = 20;
    public int GridCols { get; set; } = 20;
}

public class GameResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int StatusId { get; set; }
    public int GridRows { get; set; }
    public int GridCols { get; set; }
}

public class AddPointRequestDto
{
    public int PlayerId { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsInvulnerable { get; set; }
}

public class FireShotRequestDto
{
    public int PlayerId { get; set; }
    public int TargetRow { get; set; }
    public int TargetCol { get; set; }
    public int Power { get; set; }
}

public class PointResponseDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsInvulnerable { get; set; }
}

public class ShotResponseDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int TargetRow { get; set; }
    public int TargetCol { get; set; }
    public int Power { get; set; }
}

public class MoveResponseDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int SequenceNumber { get; set; }
    public bool IsShot { get; set; }
    public int? PointId { get; set; }
    public int? ShotId { get; set; }
}

public class GameStateResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int StatusId { get; set; }
    public int GridRows { get; set; }
    public int GridCols { get; set; }
    public List<PointResponseDto> Points { get; set; } = new();
    public List<ShotResponseDto> Shots { get; set; } = new();
    public List<MoveResponseDto> Moves { get; set; } = new();
}
