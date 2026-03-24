using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class Point
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsOwnedBy(int playerId) => PlayerId == playerId;
    }
}
