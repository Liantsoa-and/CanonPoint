using System;
using System.Collections.Generic;
using System.Text;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Services
{
    public class CannonService
    {
        public int CalculateTargetCol(int power, int columns, int playerId)
        {
            if (power < 1 || power > 9)
                throw new ArgumentException("La puissance doit être entre 1 et 9.");
            // La formule métier est en colonnes 1..N, on convertit ensuite en index 0..N-1
            int colOneBased = (int)Math.Floor((double)(power * columns) / 9);
            int colZeroBased = colOneBased - 1;
            // Plafonner dans [0, columns-1]
            int baseCol = Math.Clamp(colZeroBased, 0, columns - 1);
            // Joueur 2 vise depuis la droite: miroir horizontal.
            return playerId == 2 ? (columns - 1) - baseCol : baseCol;
        }

        public ShotResult ResolveShot(GameState state, int playerId, int power)
        {
            var cannon = state.Cannons[playerId];
            int targetCol = CalculateTargetCol(power, state.Columns, playerId);
            int targetRow = cannon.RowPosition;

            bool targetIsProtected = IsOnValidatedLine(state, targetRow, targetCol);
            var targetPoint = state.Points.FirstOrDefault(p =>
                p.Row == targetRow &&
                p.Col == targetCol &&
                p.IsActive);

            var result = new ShotResult
            {
                CannonRow = cannon.RowPosition,
                TargetRow = targetRow,
                TargetCol = targetCol,
                Trajectory = GetTrajectory(targetRow, targetCol, playerId, state.Columns)
            };

            if (targetPoint == null)
            {
                result.IsValid = false;
                result.Hit = false;
                result.InvalidReason = "Aucun point sur la cible.";
                return result;
            }

            if (targetPoint.PlayerId == playerId)
            {
                result.IsValid = false;
                result.Hit = false;
                result.InvalidReason = "La cible contient un point allie.";
                return result;
            }

            if (targetIsProtected)
            {
                result.IsValid = false;
                result.Hit = false;
                result.WasBlockedByLine = true;
                result.InvalidReason = "Le point adverse cible est protege par une ligne validee.";
                return result;
            }

            result.IsValid = true;
            result.Hit = true;
            return result;
        }

        private List<(int row, int col)> GetTrajectory(int cannonRow, int targetCol,
            int playerId, int columns)
        {
            var path = new List<(int, int)>();
            int startCol = playerId == 1 ? 0 : columns - 1;
            int direction = playerId == 1 ? 1 : -1;

            for (int c = startCol; c != targetCol; c += direction)
                path.Add((cannonRow, c));

            path.Add((cannonRow, targetCol));
            return path;
        }

        private bool IsOnValidatedLine(GameState state, int row, int col)
        {
            return state.ValidatedLines.Any(l =>
                l.GetCells().Any(cell => cell.row == row && cell.col == col));
        }
    }
}
