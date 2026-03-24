using System;
using System.Collections.Generic;
using System.Text;

namespace JeuDePoints.Domain.Models
{
    public class ValidatedLine
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public string Direction { get; set; } = "";
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public int EndRow { get; set; }
        public int EndCol { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public IEnumerable<(int row, int col)> GetCells()
        {
            int dRow = 0, dCol = 0;

            switch (Direction)
            {
                case "horizontal": dRow = 0; dCol = 1; break;
                case "vertical": dRow = 1; dCol = 0; break;
                case "diagonal_asc": dRow = -1; dCol = 1; break;
                case "diagonal_desc": dRow = 1; dCol = 1; break;
            }

            for (int i = 0; i < 5; i++)
                yield return (StartRow + i * dRow, StartCol + i * dCol);
        }
    }
}
