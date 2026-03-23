using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public interface IGameStateFactory
{
    GameState CreateNew(int gameId, int rows, int cols);
}
