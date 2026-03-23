using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public sealed class GameStateReconstructionService : IGameStateReconstructionService
{
    private readonly IGameStateFactory _gameStateFactory;
    private readonly ITurnService _turnService;

    public GameStateReconstructionService(IGameStateFactory gameStateFactory, ITurnService turnService)
    {
        _gameStateFactory = gameStateFactory;
        _turnService = turnService;
    }

    public GameState LoadGameStateFromHistory(int gameId, int rows, int cols, IReadOnlyList<MoveData> moves)
    {
        if (moves is null)
        {
            throw new ArgumentNullException(nameof(moves));
        }

        var reconstructed = _gameStateFactory.CreateNewGameState(gameId, rows, cols);
        var orderedMoves = moves.OrderBy(m => m.SequenceNumber);

        foreach (var move in orderedMoves)
        {
            var command = new TurnCommand(move.Type, move.Player, move.Row, move.Col, move.Power);
            var result = _turnService.PlayTurn(reconstructed, command);

            if (!result.Success)
            {
                throw new InvalidOperationException($"Impossible de rejouer le move #{move.SequenceNumber}: {result.ErrorMessage}");
            }

            if (result.Move is null || result.Move.SequenceNumber != move.SequenceNumber)
            {
                throw new InvalidOperationException($"Sequence incoherente pendant reconstruction au move #{move.SequenceNumber}.");
            }
        }

        return reconstructed;
    }
}
