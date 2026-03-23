using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Results;

namespace CanonPoint.App.Domain.Services;

public sealed class ShotService : IShotService
{
    public FireShotResult TryFire(GameState gameState, PlayerSide player, int cannonRow, int power)
    {
        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        if (player == PlayerSide.None)
        {
            return new FireShotResult(false, "Le joueur doit etre Player1 ou Player2.");
        }

        if (player != gameState.CurrentPlayer)
        {
            return new FireShotResult(false, "Ce n'est pas le tour de ce joueur.");
        }

        if (power < 1 || power > 9)
        {
            return new FireShotResult(false, "La puissance doit etre comprise entre 1 et 9.");
        }

        if (cannonRow < 0 || cannonRow >= gameState.IntersectionRows)
        {
            return new FireShotResult(false, "La ligne du canon est hors limites.");
        }

        var targetCol = player == PlayerSide.Player1
            ? power - 1
            : gameState.IntersectionCols - power;

        if (!gameState.IsIntersectionInBounds(cannonRow, targetCol))
        {
            return new FireShotResult(false, "La cible du tir est hors limites.");
        }

        var target = new BoardPosition(cannonRow, targetCol);
        var cell = gameState.GetCell(cannonRow, targetCol);

        if (cell.IsEmpty())
        {
            return new FireShotResult(true, Target: target, WasPointDestroyed: false);
        }

        if (cell.Owner == player)
        {
            return new FireShotResult(true, Target: target, WasPointDestroyed: false);
        }

        if (cell.IsInvulnerable)
        {
            return new FireShotResult(true, Target: target, WasPointDestroyed: false);
        }

        cell.ClearOwner();
        return new FireShotResult(true, Target: target, WasPointDestroyed: true);
    }
}
