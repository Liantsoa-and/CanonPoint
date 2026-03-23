using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Results;

namespace CanonPoint.App.Domain.Services;

public sealed class PlacementService : IPlacementService
{
    public PlacePointResult TryPlacePoint(GameState gameState, PlayerSide player, int row, int col)
    {
        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        if (player == PlayerSide.None)
        {
            return new PlacePointResult(false, "Le joueur doit etre Player1 ou Player2.");
        }

        if (player != gameState.CurrentPlayer)
        {
            return new PlacePointResult(false, "Ce n'est pas le tour de ce joueur.");
        }

        if (!gameState.IsIntersectionInBounds(row, col))
        {
            return new PlacePointResult(false, "Croisement hors limites de la grille.");
        }

        var cell = gameState.GetCell(row, col);
        if (!cell.IsEmpty())
        {
            return new PlacePointResult(false, "Ce croisement est deja occupe.");
        }

        cell.SetOwner(player);
        return new PlacePointResult(true, Position: new BoardPosition(row, col));
    }
}
