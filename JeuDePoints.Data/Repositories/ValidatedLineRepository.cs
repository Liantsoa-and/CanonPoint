using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using JeuDePoints.Domain.Models;

namespace JeuDePoints.Data.Repositories
{
    public class ValidatedLineRepository
    {
        private readonly DatabaseConnection _db;

        public ValidatedLineRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public int AddValidatedLine(ValidatedLine line)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var sql = @"INSERT INTO validated_lines
                        (game_id, player_id, direction, start_row, start_col, end_row, end_col)
                        VALUES (@GameId, @PlayerId, @Direction, @StartRow, @StartCol, @EndRow, @EndCol)
                        RETURNING id";
            return conn.ExecuteScalar<int>(sql, line);
        }

        public List<ValidatedLine> GetValidatedLines(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var rows = conn.Query(
                "SELECT * FROM validated_lines WHERE game_id = @gameId",
                new { gameId });
            return rows.Select(r => new ValidatedLine
            {
                Id = r.id,
                GameId = r.game_id,
                PlayerId = r.player_id,
                Direction = r.direction,
                StartRow = r.start_row,
                StartCol = r.start_col,
                EndRow = r.end_row,
                EndCol = r.end_col,
                CreatedAt = r.created_at
            }).ToList();
        }

        public bool LineAlreadyExists(int gameId, int playerId, string direction, int startRow, int startCol)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var count = conn.ExecuteScalar<int>(@"SELECT COUNT(*) FROM validated_lines
                WHERE game_id = @gameId AND player_id = @playerId
                AND direction = @direction AND start_row = @startRow AND start_col = @startCol",
                new { gameId, playerId, direction, startRow, startCol });
            return count > 0;
        }
    }
}
