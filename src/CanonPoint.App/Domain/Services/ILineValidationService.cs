using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public interface ILineValidationService
{
    IReadOnlyList<LineData> GetValidatedLines(GameState gameState, IReadOnlyList<LineData> candidateLines, PlayerSide player);
}
