using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class ShotResult
    {
        public int CannonRow { get; set; }
        public int TargetRow { get; set; }
        public int TargetCol { get; set; }
        public bool Hit { get; set; }
        public bool IsValid { get; set; } = true;
        public string InvalidReason { get; set; } = "";
        public List<(int row, int col)> Trajectory { get; set; } = new();
        public bool WasBlockedByLine { get; set; } = false;
    }
}