using Dapper;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Data.Repositories
{
    public class MoveRepository
    {
        private readonly DatabaseConnection _db;

        public MoveRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public int AddMove(Move move)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var sql = @"INSERT INTO moves
                (game_id, move_number, player_id, move_type,
                 row, col, cannon_row, power, target_row, target_col, hit,
                 score_p1_snapshot, score_p2_snapshot)
                VALUES
                (@GameId, @MoveNumber, @PlayerId, @MoveType,
                 @Row, @Col, @CannonRow, @Power, @TargetRow, @TargetCol, @Hit,
                 @ScoreP1Snapshot, @ScoreP2Snapshot)
                RETURNING id";
            return conn.ExecuteScalar<int>(sql, move);
        }

        public int GetLastMoveNumber(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            return conn.ExecuteScalar<int>(
                "SELECT COALESCE(MAX(move_number), 0) FROM moves WHERE game_id = @gameId",
                new { gameId });
        }

        public void DeleteMove(int gameId, int moveNumber)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute("DELETE FROM moves WHERE game_id = @gameId AND move_number = @moveNumber",
                new { gameId, moveNumber });
        }

        public List<Move> GetMoveHistory(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var rows = conn.Query(
                "SELECT * FROM moves WHERE game_id = @gameId ORDER BY move_number",
                new { gameId });
            return rows.Select(r => new Move
            {
                Id = r.id,
                GameId = r.game_id,
                MoveNumber = r.move_number,
                PlayerId = r.player_id,
                MoveType = r.move_type,
                Row = r.row,
                Col = r.col,
                CannonRow = r.cannon_row,
                Power = r.power,
                TargetRow = r.target_row,
                TargetCol = r.target_col,
                Hit = r.hit,
                ScoreP1Snapshot = r.score_p1_snapshot,
                ScoreP2Snapshot = r.score_p2_snapshot
            }).ToList();
        }
    }
}