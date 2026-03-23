using CanonPoint.App.Models.Dtos;

namespace CanonPoint.App.Services;

public interface IGameService
{
    Task<GameResponseDto> CreateGameAsync(CreateGameRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GameResponseDto>> GetGamesAsync(CancellationToken cancellationToken = default);
    Task<GameStateResponseDto?> GetGameStateAsync(int gameId, CancellationToken cancellationToken = default);
    Task<MoveResponseDto?> AddPointAsync(int gameId, AddPointRequestDto request, CancellationToken cancellationToken = default);
    Task<MoveResponseDto?> FireShotAsync(int gameId, FireShotRequestDto request, CancellationToken cancellationToken = default);

    Task<PlayerResponseDto> CreatePlayerAsync(CreatePlayerRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlayerResponseDto>> GetPlayersAsync(CancellationToken cancellationToken = default);
}
