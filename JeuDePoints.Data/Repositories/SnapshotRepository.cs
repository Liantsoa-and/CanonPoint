using Dapper;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Data.Repositories
{
    public class SnapshotRepository
    {
        private readonly DatabaseConnection _db;

        public SnapshotRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public void SaveSnapshot(int gameId, int moveNumber, GameSnapshot snapshot)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute(@"INSERT INTO game_snapshots (game_id, move_number, snapshot_data)
                          VALUES (@gameId, @moveNumber, @data::jsonb)",
                new { gameId, moveNumber, data = snapshot.SnapshotData });
        }

        public GameSnapshot? GetSnapshot(int gameId, int moveNumber)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var row = conn.QueryFirstOrDefault(@"SELECT * FROM game_snapshots
                WHERE game_id = @gameId AND move_number = @moveNumber",
                new { gameId, moveNumber });
            if (row == null) return null;
            return new GameSnapshot
            {
                Id = row.id,
                GameId = row.game_id,
                MoveNumber = row.move_number,
                SnapshotData = row.snapshot_data.ToString()
            };
        }

        public GameSnapshot? GetLatestSnapshot(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var row = conn.QueryFirstOrDefault(@"SELECT * FROM game_snapshots
                WHERE game_id = @gameId ORDER BY move_number DESC LIMIT 1",
                new { gameId });
            if (row == null) return null;
            return new GameSnapshot
            {
                Id = row.id,
                GameId = row.game_id,
                MoveNumber = row.move_number,
                SnapshotData = row.snapshot_data.ToString()
            };
        }

        public List<int> GetSnapshotMoveNumbers(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            return conn.Query<int>(@"SELECT move_number FROM game_snapshots
                WHERE game_id = @gameId ORDER BY move_number",
                new { gameId }).ToList();
        }

        public void DeleteSnapshot(int gameId, int moveNumber)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute(@"DELETE FROM game_snapshots
                WHERE game_id = @gameId AND move_number = @moveNumber",
                new { gameId, moveNumber });
        }
    }
}