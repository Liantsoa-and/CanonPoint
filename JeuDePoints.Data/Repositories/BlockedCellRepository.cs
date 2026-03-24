using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Data.Repositories
{
    public class BlockedCellRepository
    {
        private readonly DatabaseConnection _db;

        public BlockedCellRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public void AddBlockedCells(int gameId, int lineId,
            IEnumerable<(int row, int col)> cells, int blockingPlayerId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            foreach (var (row, col) in cells)
                conn.Execute(@"INSERT INTO blocked_cells
                    (game_id, validated_line_id, row, col, blocking_player_id)
                    VALUES (@gameId, @lineId, @row, @col, @blockingPlayerId)",
                    new { gameId, lineId, row, col, blockingPlayerId });
        }

        public bool IsCellBlocked(int gameId, int row, int col, int forPlayerId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var count = conn.ExecuteScalar<int>(@"SELECT COUNT(*) FROM blocked_cells
                WHERE game_id = @gameId AND row = @row AND col = @col
                AND blocking_player_id <> @forPlayerId",
                new { gameId, row, col, forPlayerId });
            return count > 0;
        }

        public List<BlockedCell> GetBlockedCells(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var rows = conn.Query(
                "SELECT * FROM blocked_cells WHERE game_id = @gameId",
                new { gameId });
            return rows.Select(r => new BlockedCell
            {
                Id = r.id,
                GameId = r.game_id,
                ValidatedLineId = r.validated_line_id,
                Row = r.row,
                Col = r.col,
                BlockingPlayerId = r.blocking_player_id
            }).ToList();
        }
    }
}
