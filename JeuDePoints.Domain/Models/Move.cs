using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class Move
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int MoveNumber { get; set; }
        public int PlayerId { get; set; }
        public string MoveType { get; set; } = "";
        public int? Row { get; set; }
        public int? Col { get; set; }
        public int? CannonRow { get; set; }
        public int? Power { get; set; }
        public int? TargetRow { get; set; }
        public int? TargetCol { get; set; }
        public bool? Hit { get; set; }
        public int ScoreP1Snapshot { get; set; }
        public int ScoreP2Snapshot { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
