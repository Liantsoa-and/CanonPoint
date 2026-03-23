using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public interface IGameStateReconstructionService
{
    GameState LoadGameStateFromHistory(int gameId, int rows, int cols, IReadOnlyList<MoveData> moves);
}
