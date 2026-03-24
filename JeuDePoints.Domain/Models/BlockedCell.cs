using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class BlockedCell
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int ValidatedLineId { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int BlockingPlayerId { get; set; }
    }
}
