using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Data.Repositories
{
    public class CannonRepository
    {
        private readonly DatabaseConnection _db;

        public CannonRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public void InitCannons(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute(@"INSERT INTO cannons (game_id, player_id, row_position) VALUES
                          (@gameId, 1, 0), (@gameId, 2, 0)", new { gameId });
        }

        public Cannon? GetCannon(int gameId, int playerId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var row = conn.QueryFirstOrDefault(
                "SELECT * FROM cannons WHERE game_id = @gameId AND player_id = @playerId",
                new { gameId, playerId });
            if (row == null) return null;
            return new Cannon
            {
                Id = row.id,
                GameId = row.game_id,
                PlayerId = row.player_id,
                RowPosition = row.row_position
            };
        }

        public void UpdatePosition(int gameId, int playerId, int newRow)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute(@"UPDATE cannons SET row_position = @newRow
                          WHERE game_id = @gameId AND player_id = @playerId",
                new { newRow, gameId, playerId });
        }
    }
}
