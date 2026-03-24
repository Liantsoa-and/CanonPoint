using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class Cannon
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int RowPosition { get; set; }

        public void MoveUp()
        {
            if (RowPosition > 0)
                RowPosition--;
        }

        public void MoveDown(int maxRow)
        {
            if (RowPosition < maxRow)
                RowPosition++;
        }
    }
}
