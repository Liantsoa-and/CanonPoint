using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;
using CanonPoint.App.Domain.Results;

namespace CanonPoint.App.Domain.Services;

public sealed class TurnService : ITurnService
{
    private readonly IPlacementService _placementService;
    private readonly ILineDetectionService _lineDetectionService;
    private readonly ILineValidationService _lineValidationService;
    private readonly IShotService _shotService;

    public TurnService(
        IPlacementService placementService,
        ILineDetectionService lineDetectionService,
        ILineValidationService lineValidationService,
        IShotService shotService)
    {
        _placementService = placementService;
        _lineDetectionService = lineDetectionService;
        _lineValidationService = lineValidationService;
        _shotService = shotService;
    }

    public TurnResult PlayTurn(GameState gameState, TurnCommand command)
    {
        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (command.MoveType == MoveType.PlacePoint)
        {
            return PlayPlacementTurn(gameState, command);
        }

        if (command.MoveType == MoveType.FireShot)
        {
            return PlayShotTurn(gameState, command);
        }

        return new TurnResult(false, "Type de coup inconnu.");
    }

    private TurnResult PlayPlacementTurn(GameState gameState, TurnCommand command)
    {
        var placement = _placementService.TryPlacePoint(gameState, command.Player, command.Row, command.Col);
        if (!placement.Success)
        {
            return new TurnResult(false, placement.ErrorMessage);
        }

        var candidateLines = _lineDetectionService.FindCandidateLines(gameState, command.Row, command.Col, command.Player);
        _lineValidationService.GetValidatedLines(gameState, candidateLines, command.Player);

        FinalizeTurnState(gameState);

        var move = new MoveData(
            sequenceNumber: gameState.ConsumeNextSequenceNumber(),
            player: command.Player,
            type: MoveType.PlacePoint,
            row: command.Row,
            col: command.Col,
            power: null,
            createdAtUtc: DateTime.UtcNow);

        gameState.SwitchCurrentPlayer();
        return new TurnResult(true, Move: move);
    }

    private TurnResult PlayShotTurn(GameState gameState, TurnCommand command)
    {
        if (!command.Power.HasValue)
        {
            return new TurnResult(false, "La puissance est obligatoire pour un tir.");
        }

        var shot = _shotService.TryFire(gameState, command.Player, command.Row, command.Power.Value);
        if (!shot.Success)
        {
            return new TurnResult(false, shot.ErrorMessage);
        }

        FinalizeTurnState(gameState);

        var target = shot.Target ?? new BoardPosition(command.Row, command.Col);
        var move = new MoveData(
            sequenceNumber: gameState.ConsumeNextSequenceNumber(),
            player: command.Player,
            type: MoveType.FireShot,
            row: target.Row,
            col: target.Col,
            power: command.Power,
            createdAtUtc: DateTime.UtcNow);

        gameState.SwitchCurrentPlayer();
        return new TurnResult(true, Move: move);
    }

    private static void FinalizeTurnState(GameState gameState)
    {
        gameState.Status = gameState.HasAnyEmptyIntersection()
            ? GameStatus.InProgress
            : GameStatus.Finished;
    }
}
