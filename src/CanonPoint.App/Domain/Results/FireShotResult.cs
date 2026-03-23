using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Results;

public sealed record FireShotResult(
	bool Success,
	string? ErrorMessage = null,
	BoardPosition? Target = null,
	bool WasPointDestroyed = false);
