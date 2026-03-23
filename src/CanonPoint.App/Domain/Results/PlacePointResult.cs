using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Results;

public sealed record PlacePointResult(bool Success, string? ErrorMessage = null, BoardPosition? Position = null);
