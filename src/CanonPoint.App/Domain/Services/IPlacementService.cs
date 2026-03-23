using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Results;

namespace CanonPoint.App.Domain.Services;

public interface IPlacementService
{
    PlacePointResult TryPlacePoint(GameState gameState, PlayerSide player, int row, int col);
}
