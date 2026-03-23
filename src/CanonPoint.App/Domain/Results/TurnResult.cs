using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Results;

public sealed record TurnResult(bool Success, string? ErrorMessage = null, MoveData? Move = null);
