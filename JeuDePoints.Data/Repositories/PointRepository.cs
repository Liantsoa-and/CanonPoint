using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Data.Repositories
{
    public class PointRepository
    {
        private readonly DatabaseConnection _db;

        public PointRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public int AddPoint(int gameId, int playerId, int row, int col)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            // Réactiver une case précédemment touchée au lieu d'insérer un doublon
            var reactivateSql = @"UPDATE points
                                  SET player_id = @playerId, is_active = TRUE
                                  WHERE game_id = @gameId AND row = @row AND col = @col AND is_active = FALSE
                                  RETURNING id";
            var existingId = conn.ExecuteScalar<int?>(reactivateSql, new { gameId, playerId, row, col });
            if (existingId.HasValue) return existingId.Value;

            var insertSql = @"INSERT INTO points (game_id, player_id, row, col)
                              VALUES (@gameId, @playerId, @row, @col) RETURNING id";
            return conn.ExecuteScalar<int>(insertSql, new { gameId, playerId, row, col });
        }

        public List<Point> GetActivePoints(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var rows = conn.Query(
                "SELECT * FROM points WHERE game_id = @gameId AND is_active = TRUE",
                new { gameId });
            return rows.Select(r => new Point
            {
                Id = r.id,
                GameId = r.game_id,
                PlayerId = r.player_id,
                Row = r.row,
                Col = r.col,
                IsActive = r.is_active
            }).ToList();
        }

        public List<Point> GetActivePointsByPlayer(int gameId, int playerId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var rows = conn.Query(
                "SELECT * FROM points WHERE game_id = @gameId AND player_id = @playerId AND is_active = TRUE",
                new { gameId, playerId });
            return rows.Select(r => new Point
            {
                Id = r.id,
                GameId = r.game_id,
                PlayerId = r.player_id,
                Row = r.row,
                Col = r.col,
                IsActive = r.is_active
            }).ToList();
        }

        public bool DeactivatePoint(int gameId, int row, int col)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var affected = conn.Execute(@"UPDATE points SET is_active = FALSE
                WHERE game_id = @gameId AND row = @row AND col = @col AND is_active = TRUE",
                new { gameId, row, col });
            return affected > 0;
        }

        public bool IsCellOccupied(int gameId, int row, int col)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var count = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM points WHERE game_id = @gameId AND row = @row AND col = @col AND is_active = TRUE",
                new { gameId, row, col });
            return count > 0;
        }
    }
}
