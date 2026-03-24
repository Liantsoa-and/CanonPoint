using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class GameState
    {
        public int GameId { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string Player1Name { get; set; } = "Joueur 1";
        public string Player2Name { get; set; } = "Joueur 2";
        public int CurrentTurn { get; set; } = 1;
        public int ScoreP1 { get; set; } = 0;
        public int ScoreP2 { get; set; } = 0;
        public string Status { get; set; } = "ongoing";

        // Feedback du dernier tir pour l'UI.
        public bool LastShotWasValid { get; set; } = true;
        public string LastShotMessage { get; set; } = "";
        public int? LastShotTargetRow { get; set; }
        public int? LastShotTargetCol { get; set; }

        public List<Point> Points { get; set; } = new();
        public Dictionary<int, Cannon> Cannons { get; set; } = new();
        public List<ValidatedLine> ValidatedLines { get; set; } = new();
        public List<BlockedCell> BlockedCells { get; set; } = new();

        public bool IsFinished() => Status == "finished";

        public string GetCurrentPlayerName() =>
            CurrentTurn == 1 ? Player1Name : Player2Name;
    }
}
