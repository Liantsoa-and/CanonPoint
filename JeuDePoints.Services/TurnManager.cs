using System;
using System.Collections.Generic;
using System.Text;

using JeuDePoints.Domain.Models;

namespace JeuDePoints.Services
{
    public class TurnManager
    {
        private readonly CannonService _cannonService;

        public TurnManager(CannonService cannonService)
        {
            _cannonService = cannonService;
        }

        public ValidationResult ValidatePlacement(GameState state, int row, int col, int playerId)
        {
            // Cellule déjà occupée
            if (state.Points.Any(p => p.Row == row && p.Col == col && p.IsActive))
                return ValidationResult.Fail("Cette intersection est déjà occupée.");

            return ValidationResult.Ok();
        }

        public ValidationResult ValidateShot(GameState state, int playerId, int power)
        {
            if (power < 1 || power > 9)
                return ValidationResult.Fail("La puissance doit être entre 1 et 9.");

            return ValidationResult.Ok();
        }

        public int NextTurn(int currentTurn) => currentTurn == 1 ? 2 : 1;
    }
}
