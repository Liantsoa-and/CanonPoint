using CanonPoint.App.Domain.Enums;
using CanonPoint.App.Domain.Models;

namespace CanonPoint.App.Domain.Services;

public sealed class GameStateFactory : IGameStateFactory
{
    public GameState CreateNewGameState(int gameId, int rows, int cols)
    {
        var gameState = new GameState(gameId, rows, cols)
        {
            Status = GameStatus.InProgress,
            CurrentPlayer = PlayerSide.Player1,
            ScorePlayer1 = 0,
            ScorePlayer2 = 0
        };

        gameState.SetCanonRows(0, 0);
        return gameState;
    }
}