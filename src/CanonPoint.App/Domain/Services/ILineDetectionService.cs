using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public interface ILineDetectionService
{
    IReadOnlyList<LineData> FindCandidateLines(GameState gameState, int lastRow, int lastCol, PlayerSide player);
}
