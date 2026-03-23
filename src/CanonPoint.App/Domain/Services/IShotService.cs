using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Results;

namespace CanonPoint.App.Domain.Services;

public interface IShotService
{
    FireShotResult TryFire(GameState gameState, PlayerSide player, int cannonRow, int power);
}
