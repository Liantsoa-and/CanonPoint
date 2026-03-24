using System;
using System.Collections.Generic;
using System.Text;

using Dapper;
using JeuDePoints.Domain.Models;
using Npgsql;

namespace JeuDePoints.Data.Repositories
{
    public class GameListRow
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = "";
        public string Score { get; set; } = "";
    }

    public class GameRepository
    {
        private readonly DatabaseConnection _db;

        public GameRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public int CreateGame(int rows, int cols, string p1, string p2)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var sql = @"INSERT INTO games (rows, columns, player1_name, player2_name)
                        VALUES (@rows, @cols, @p1, @p2) RETURNING id";
            return conn.ExecuteScalar<int>(sql, new { rows, cols, p1, p2 });
        }

        public GameState? GetGameById(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var sql = "SELECT * FROM games WHERE id = @gameId";
            var row = conn.QueryFirstOrDefault(sql, new { gameId });
            if (row == null) return null;

            return new GameState
            {
                GameId = row.id,
                Rows = row.rows,
                Columns = row.columns,
                Player1Name = row.player1_name,
                Player2Name = row.player2_name,
                CurrentTurn = row.current_turn,
                ScoreP1 = row.score_p1,
                ScoreP2 = row.score_p2,
                Status = row.status
            };
        }

        public void UpdateTurn(int gameId, int newTurn)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute("UPDATE games SET current_turn = @newTurn WHERE id = @gameId",
                new { newTurn, gameId });
        }

        public void UpdateScores(int gameId, int scoreP1, int scoreP2)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute("UPDATE games SET score_p1 = @scoreP1, score_p2 = @scoreP2 WHERE id = @gameId",
                new { scoreP1, scoreP2, gameId });
        }

        public void SetStatus(int gameId, string status)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute("UPDATE games SET status = @status WHERE id = @gameId",
                new { status, gameId });
        }

        public void DeleteGame(int gameId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute("DELETE FROM games WHERE id = @gameId", new { gameId });
        }

        public List<GameListRow> GetGamesForStartScreen()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"SELECT
                            id AS GameId,
                            (player1_name || ' vs ' || player2_name) AS GameName,
                            (score_p1::text || ' - ' || score_p2::text) AS Score
                        FROM games
                        ORDER BY updated_at DESC, id DESC";

            return conn.Query<GameListRow>(sql).ToList();
        }
    }
}
