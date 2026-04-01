using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace JeuDePoints.Domain.Models
{
    public class GameSnapshot
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int MoveNumber { get; set; }
        public string SnapshotData { get; set; } = "";

        public static GameSnapshot FromGameState(GameState state, int moveNumber)
        {
            var data = new
            {
                current_turn = state.CurrentTurn,
                score_p1 = state.ScoreP1,
                score_p2 = state.ScoreP2,
                cannons = new
                {
                    player1 = new { row_position = state.Cannons.ContainsKey(1) ? state.Cannons[1].RowPosition : 0 },
                    player2 = new { row_position = state.Cannons.ContainsKey(2) ? state.Cannons[2].RowPosition : 0 }
                },
                points = state.Points.Select(p => new
                {
                    player_id = p.PlayerId,
                    row = p.Row,
                    col = p.Col,
                    is_active = p.IsActive
                }),
                validated_lines = state.ValidatedLines.Select(l => new
                {
                    player_id = l.PlayerId,
                    direction = l.Direction,
                    start_row = l.StartRow,
                    start_col = l.StartCol,
                    end_row = l.EndRow,
                    end_col = l.EndCol
                }),
                blocked_cells = state.BlockedCells.Select(b => new
                {
                    row = b.Row,
                    col = b.Col,
                    blocking_player_id = b.BlockingPlayerId
                }),
                resurrection_rights = state.ResurrectionRights.Select(r => new
                {
                    player_id = r.PlayerId,
                    row = r.Row,
                    col = r.Col
                })
            };

            return new GameSnapshot
            {
                GameId = state.GameId,
                MoveNumber = moveNumber,
                SnapshotData = JsonConvert.SerializeObject(data)
            };
        }

        public GameState ToGameState()
        {
            dynamic d = JsonConvert.DeserializeObject(SnapshotData)!;

            var state = new GameState
            {
                GameId = GameId,
                CurrentTurn = (int)d.current_turn,
                ScoreP1 = (int)d.score_p1,
                ScoreP2 = (int)d.score_p2
            };

            state.Cannons[1] = new Cannon { PlayerId = 1, RowPosition = (int)d.cannons.player1.row_position };
            state.Cannons[2] = new Cannon { PlayerId = 2, RowPosition = (int)d.cannons.player2.row_position };

            foreach (var p in d.points)
                state.Points.Add(new Point
                {
                    PlayerId = (int)p.player_id,
                    Row = (int)p.row,
                    Col = (int)p.col,
                    IsActive = (bool)p.is_active
                });

            foreach (var l in d.validated_lines)
                state.ValidatedLines.Add(new ValidatedLine
                {
                    PlayerId = (int)l.player_id,
                    Direction = (string)l.direction,
                    StartRow = (int)l.start_row,
                    StartCol = (int)l.start_col,
                    EndRow = (int)l.end_row,
                    EndCol = (int)l.end_col
                });

            foreach (var b in d.blocked_cells)
                state.BlockedCells.Add(new BlockedCell
                {
                    Row = (int)b.row,
                    Col = (int)b.col,
                    BlockingPlayerId = (int)b.blocking_player_id
                });

            try
            {
                foreach (var rr in d.resurrection_rights)
                    state.ResurrectionRights.Add(new ResurrectionRight
                    {
                        PlayerId = (int)rr.player_id,
                        Row = (int)rr.row,
                        Col = (int)rr.col
                    });
            }
            catch
            {
                // Snapshots plus anciens: pas de droits de resurrection.
            }

            return state;
        }
    }
}
