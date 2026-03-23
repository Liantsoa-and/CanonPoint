using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Results;

namespace CanonPoint.App.Domain.Services;

public interface ITurnService
{
    TurnResult PlayTurn(GameState gameState, TurnCommand command);
}