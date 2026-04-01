using System;
using System.Collections.Generic;
using System.Text;

using JeuDePoints.Data;
using JeuDePoints.Data.Repositories;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Services
{
    public enum Direction { Up, Down }

    public class GameService
    {
        private readonly GameRepository _gameRepo;
        private readonly CannonRepository _cannonRepo;
        private readonly PointRepository _pointRepo;
        private readonly ValidatedLineRepository _lineRepo;
        private readonly BlockedCellRepository _blockedRepo;
        private readonly MoveRepository _moveRepo;
        private readonly SnapshotRepository _snapshotRepo;
        private readonly AlignmentChecker _alignmentChecker;
        private readonly CannonService _cannonService;
        private readonly TurnManager _turnManager;

        public GameService(
            GameRepository gameRepo, CannonRepository cannonRepo,
            PointRepository pointRepo, ValidatedLineRepository lineRepo,
            BlockedCellRepository blockedRepo, MoveRepository moveRepo,
            SnapshotRepository snapshotRepo)
        {
            _gameRepo = gameRepo;
            _cannonRepo = cannonRepo;
            _pointRepo = pointRepo;
            _lineRepo = lineRepo;
            _blockedRepo = blockedRepo;
            _moveRepo = moveRepo;
            _snapshotRepo = snapshotRepo;
            _alignmentChecker = new AlignmentChecker();
            _cannonService = new CannonService();
            _turnManager = new TurnManager(_cannonService);
        }

        public GameState CreateNewGame(int rows, int cols, string p1, string p2)
        {
            int gameId = _gameRepo.CreateGame(rows, cols, p1, p2);
            _cannonRepo.InitCannons(gameId);

            var state = new GameState
            {
                GameId = gameId,
                Rows = rows,
                Columns = cols,
                Player1Name = p1,
                Player2Name = p2,
                CurrentTurn = 1
            };

            state.Cannons[1] = new Cannon { GameId = gameId, PlayerId = 1, RowPosition = 0 };
            state.Cannons[2] = new Cannon { GameId = gameId, PlayerId = 2, RowPosition = 0 };

            var snapshot = GameSnapshot.FromGameState(state, 0);
            _snapshotRepo.SaveSnapshot(gameId, 0, snapshot);

            return state;
        }

        public GameState PlacePoint(GameState state, int row, int col)
        {
            int playerId = state.CurrentTurn;
            var validation = _turnManager.ValidatePlacement(state, row, col, playerId);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            // Ajouter le point
            int pointId = _pointRepo.AddPoint(state.GameId, playerId, row, col);
            var existing = state.Points.FirstOrDefault(p => p.Row == row && p.Col == col && !p.IsActive);
            if (existing != null)
            {
                existing.Id = pointId;
                existing.PlayerId = playerId;
                existing.IsActive = true;
            }
            else
            {
                state.Points.Add(new Point
                {
                    Id = pointId,
                    GameId = state.GameId,
                    PlayerId = playerId,
                    Row = row,
                    Col = col,
                    IsActive = true
                });
            }

            state.ResurrectionRights.RemoveAll(r =>
                r.PlayerId == playerId &&
                r.Row == row &&
                r.Col == col);

            // Vérifier les alignements
            var newLines = _alignmentChecker.CheckAlignments(state, row, col, playerId);
            foreach (var line in newLines)
            {
                line.GameId = state.GameId;
                int lineId = _lineRepo.AddValidatedLine(line);
                line.Id = lineId;
                state.ValidatedLines.Add(line);

                var cells = line.GetCells().ToList();
                _blockedRepo.AddBlockedCells(state.GameId, lineId, cells, playerId);
                foreach (var (r, c) in cells)
                    state.BlockedCells.Add(new BlockedCell
                    {
                        GameId = state.GameId,
                        ValidatedLineId = lineId,
                        Row = r,
                        Col = c,
                        BlockingPlayerId = playerId
                    });

                if (playerId == 1) state.ScoreP1++;
                else state.ScoreP2++;
            }

            _gameRepo.UpdateScores(state.GameId, state.ScoreP1, state.ScoreP2);

            // Enregistrer le mouvement
            int moveNumber = _moveRepo.GetLastMoveNumber(state.GameId) + 1;
            _moveRepo.AddMove(new Move
            {
                GameId = state.GameId,
                MoveNumber = moveNumber,
                PlayerId = playerId,
                MoveType = "PLACE",
                Row = row,
                Col = col,
                ScoreP1Snapshot = state.ScoreP1,
                ScoreP2Snapshot = state.ScoreP2
            });

            // Passer le tour
            state.CurrentTurn = _turnManager.NextTurn(state.CurrentTurn);
            _gameRepo.UpdateTurn(state.GameId, state.CurrentTurn);

            // Sauvegarder le snapshot
            var snapshot = GameSnapshot.FromGameState(state, moveNumber);
            _snapshotRepo.SaveSnapshot(state.GameId, moveNumber, snapshot);

            return state;
        }

        public GameState ShootCannon(GameState state, int power)
        {
            int playerId = state.CurrentTurn;

            var validation = _turnManager.ValidateShot(state, playerId, power);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            // Résoudre le tir (un seul appel)
            var shotResult = _cannonService.ResolveShot(state, playerId, power);

            state.LastShotWasValid = shotResult.IsValid;
            state.LastShotMessage = shotResult.IsValid
                ? BuildShotSuccessMessage(shotResult)
                : shotResult.InvalidReason;
            state.LastShotTargetRow = shotResult.TargetRow;
            state.LastShotTargetCol = shotResult.TargetCol;

            // Si la balle a touché un point adverse, le désactiver
            if (shotResult.ShouldDeactivateTargetPoint)
            {
                var point = state.Points.FirstOrDefault(p =>
                    p.Row == shotResult.TargetRow && p.Col == shotResult.TargetCol && p.IsActive);
                if (point != null)
                {
                    _pointRepo.DeactivatePoint(state.GameId, shotResult.TargetRow, shotResult.TargetCol);
                    point.IsActive = false;

                    if (!state.ResurrectionRights.Any(r =>
                        r.PlayerId == point.PlayerId &&
                        r.Row == point.Row &&
                        r.Col == point.Col))
                    {
                        state.ResurrectionRights.Add(new ResurrectionRight
                        {
                            PlayerId = point.PlayerId,
                            Row = point.Row,
                            Col = point.Col
                        });
                    }
                }
            }

            if (shotResult.ShouldResurrectOwnPoint)
            {
                int resurrectedId = _pointRepo.AddPoint(
                    state.GameId,
                    playerId,
                    shotResult.TargetRow,
                    shotResult.TargetCol);

                var existing = state.Points.FirstOrDefault(p =>
                    p.Row == shotResult.TargetRow &&
                    p.Col == shotResult.TargetCol);

                if (existing != null)
                {
                    existing.Id = resurrectedId;
                    existing.PlayerId = playerId;
                    existing.IsActive = true;
                }
                else
                {
                    state.Points.Add(new Point
                    {
                        Id = resurrectedId,
                        GameId = state.GameId,
                        PlayerId = playerId,
                        Row = shotResult.TargetRow,
                        Col = shotResult.TargetCol,
                        IsActive = true
                    });
                }

                state.ResurrectionRights.RemoveAll(r =>
                    r.PlayerId == playerId &&
                    r.Row == shotResult.TargetRow &&
                    r.Col == shotResult.TargetCol);

                // Une resurrection est equivalente a un nouveau point pose:
                // elle peut donc creer une ligne validee.
                var newLines = _alignmentChecker.CheckAlignments(
                    state,
                    shotResult.TargetRow,
                    shotResult.TargetCol,
                    playerId);

                foreach (var line in newLines)
                {
                    line.GameId = state.GameId;
                    int lineId = _lineRepo.AddValidatedLine(line);
                    line.Id = lineId;
                    state.ValidatedLines.Add(line);

                    var cells = line.GetCells().ToList();
                    _blockedRepo.AddBlockedCells(state.GameId, lineId, cells, playerId);
                    foreach (var (r, c) in cells)
                        state.BlockedCells.Add(new BlockedCell
                        {
                            GameId = state.GameId,
                            ValidatedLineId = lineId,
                            Row = r,
                            Col = c,
                            BlockingPlayerId = playerId
                        });

                    if (playerId == 1) state.ScoreP1++;
                    else state.ScoreP2++;
                }

                if (newLines.Count > 0)
                    _gameRepo.UpdateScores(state.GameId, state.ScoreP1, state.ScoreP2);
            }

            int moveNumber = _moveRepo.GetLastMoveNumber(state.GameId) + 1;
            _moveRepo.AddMove(new Move
            {
                GameId = state.GameId,
                MoveNumber = moveNumber,
                PlayerId = playerId,
                MoveType = "SHOOT",
                CannonRow = state.Cannons[playerId].RowPosition,
                Power = power,
                TargetRow = shotResult.TargetRow,
                TargetCol = shotResult.TargetCol,
                Hit = shotResult.Hit,
                ScoreP1Snapshot = state.ScoreP1,
                ScoreP2Snapshot = state.ScoreP2
            });

            state.CurrentTurn = _turnManager.NextTurn(state.CurrentTurn);
            _gameRepo.UpdateTurn(state.GameId, state.CurrentTurn);

            var snapshot = GameSnapshot.FromGameState(state, moveNumber);
            _snapshotRepo.SaveSnapshot(state.GameId, moveNumber, snapshot);

            return state;
        }

        public GameState MoveCannon(GameState state, int playerId, Direction direction)
        {
            var cannon = state.Cannons[playerId];
            if (direction == Direction.Up)
                cannon.MoveUp();
            else
                cannon.MoveDown(state.Rows - 1);

            _cannonRepo.UpdatePosition(state.GameId, playerId, cannon.RowPosition);
            return state;
        }

        private static string BuildShotSuccessMessage(ShotResult shotResult)
        {
            if (shotResult.ShouldDeactivateTargetPoint && shotResult.ShouldResurrectOwnPoint)
                return "Tir valide: point adverse detruit et point ressuscite.";

            if (shotResult.ShouldResurrectOwnPoint)
                return "Tir valide: point ressuscite.";

            if (shotResult.ShouldDeactivateTargetPoint)
                return "Tir valide: point adverse detruit.";

            return "Tir valide.";
        }

        public GameState UndoLastMove(GameState state)
        {
            int lastMove = _moveRepo.GetLastMoveNumber(state.GameId);
            if (lastMove == 0)
                throw new InvalidOperationException("Aucun mouvement à annuler.");

            _moveRepo.DeleteMove(state.GameId, lastMove);
            _snapshotRepo.DeleteSnapshot(state.GameId, lastMove);

            var snapshot = _snapshotRepo.GetLatestSnapshot(state.GameId);
            if (snapshot == null)
                throw new InvalidOperationException("Snapshot introuvable.");

            var restored = snapshot.ToGameState();
            restored.GameId = state.GameId;
            restored.Rows = state.Rows;
            restored.Columns = state.Columns;
            restored.Player1Name = state.Player1Name;
            restored.Player2Name = state.Player2Name;

            _gameRepo.UpdateTurn(state.GameId, restored.CurrentTurn);
            _gameRepo.UpdateScores(state.GameId, restored.ScoreP1, restored.ScoreP2);

            return restored;
        }

        public GameState ResetGame(GameState state)
        {
            int rows = state.Rows, cols = state.Columns;
            string p1 = state.Player1Name, p2 = state.Player2Name;
            _gameRepo.DeleteGame(state.GameId);
            return CreateNewGame(rows, cols, p1, p2);
        }

        public GameState EndGame(GameState state)
        {
            state.Status = "finished";
            _gameRepo.SetStatus(state.GameId, "finished");

            int moveNumber = _moveRepo.GetLastMoveNumber(state.GameId) + 1;
            _moveRepo.AddMove(new Move
            {
                GameId = state.GameId,
                MoveNumber = moveNumber,
                PlayerId = state.CurrentTurn,
                MoveType = "END",
                ScoreP1Snapshot = state.ScoreP1,
                ScoreP2Snapshot = state.ScoreP2
            });

            var snapshot = GameSnapshot.FromGameState(state, moveNumber);
            _snapshotRepo.SaveSnapshot(state.GameId, moveNumber, snapshot);

            return state;
        }

        public GameState LoadLatestSnapshotState(int gameId, out int moveNumber)
        {
            var game = _gameRepo.GetGameById(gameId)
                ?? throw new InvalidOperationException("Partie introuvable.");

            var latest = _snapshotRepo.GetLatestSnapshot(gameId)
                ?? throw new InvalidOperationException("Aucun snapshot disponible pour cette partie.");

            moveNumber = latest.MoveNumber;

            var state = latest.ToGameState();
            state.GameId = game.GameId;
            state.Rows = game.Rows;
            state.Columns = game.Columns;
            state.Player1Name = game.Player1Name;
            state.Player2Name = game.Player2Name;
            state.Status = game.Status;

            return state;
        }
    }
}
