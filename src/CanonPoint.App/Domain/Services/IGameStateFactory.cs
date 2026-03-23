using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public interface IGameStateFactory
{
    GameState CreateNewGameState(int gameId, int rows, int cols);
}
