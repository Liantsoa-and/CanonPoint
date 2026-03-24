using JeuDePoints.Domain.Models;
using SystemPoint = System.Drawing.Point;

namespace JeuDePoints.Controls
{
    public class BoardPanel : Panel
    {
        private GameState? _state;
        private int _cellSize = 40;

        public event Action<int, int>? IntersectionClicked;

        public BoardPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        public void UpdateState(GameState state)
        {
            _state = state;
            // Réserver de l'espace pour les canons dessinés dans le panel + padding
            int availableW = Width - 120;
            int availableH = Height - 40;
            _cellSize = Math.Min(availableW / (state.Columns + 1), availableH / (state.Rows + 1));
            _cellSize = Math.Max(_cellSize, 20);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_state == null) return;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            DrawGrid(g);
            DrawBlockedCells(g);
            DrawValidatedLines(g);
            DrawPoints(g);
            DrawCannons(g);
        }

        private void DrawGrid(Graphics g)
        {
            var pen = new Pen(Color.LightGray, 1);
            for (int r = 0; r < _state!.Rows; r++)
                g.DrawLine(pen, GetX(0), GetY(r), GetX(_state.Columns - 1), GetY(r));
            for (int c = 0; c < _state.Columns; c++)
                g.DrawLine(pen, GetX(c), GetY(0), GetX(c), GetY(_state.Rows - 1));
        }

        private void DrawPoints(Graphics g)
        {
            foreach (var p in _state!.Points.Where(p => p.IsActive))
            {
                var brush = p.PlayerId == 1 ? Brushes.SteelBlue : Brushes.Crimson;
                int x = GetX(p.Col) - 8;
                int y = GetY(p.Row) - 8;
                g.FillEllipse(brush, x, y, 16, 16);
            }
        }

        private void DrawValidatedLines(Graphics g)
        {
            foreach (var line in _state!.ValidatedLines)
            {
                var color = line.PlayerId == 1 ? Color.SteelBlue : Color.Crimson;
                var pen = new Pen(color, 3);
                var cells = line.GetCells().ToList();
                var first = cells.First();
                var last = cells.Last();
                g.DrawLine(pen, GetX(first.col), GetY(first.row),
                                GetX(last.col), GetY(last.row));
            }
        }

        private void DrawBlockedCells(Graphics g)
        {
            var brush = new SolidBrush(Color.FromArgb(40, Color.Gray));
            foreach (var b in _state!.BlockedCells)
            {
                int x = GetX(b.Col) - _cellSize / 2;
                int y = GetY(b.Row) - _cellSize / 2;
                g.FillRectangle(brush, x, y, _cellSize, _cellSize);
            }
        }

        private void DrawCannons(Graphics g)
        {
            if (_state == null) return;
            foreach (var kvp in _state.Cannons)
            {
                int playerId = kvp.Key;
                var cannon = kvp.Value;
                int y = GetY(cannon.RowPosition);

                // Canon joueur 1 à gauche
                // Canon joueur 2 à droite
                int x = playerId == 1
                    ? GetX(0) - _cellSize - 10
                    : GetX(_state.Columns - 1) + 10;

                var color = playerId == 1 ? Color.SteelBlue : Color.Crimson;
                var brush = new SolidBrush(color);

                // Corps
                g.FillRectangle(brush, x, y - 8, 20, 16);

                // Tube
                if (playerId == 1)
                    g.FillRectangle(brush, x + 20, y - 4, 14, 8);
                else
                    g.FillRectangle(brush, x - 14, y - 4, 14, 8);

                // Roues
                g.FillEllipse(Brushes.DarkGray, x + 1, y + 6, 8, 8);
                g.FillEllipse(Brushes.DarkGray, x + 11, y + 6, 8, 8);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (_state == null) return;
            var (row, col) = ScreenToGrid(e.Location);
            if (row >= 0 && row < _state.Rows && col >= 0 && col < _state.Columns)
                IntersectionClicked?.Invoke(row, col);
        }

        public (int row, int col) ScreenToGrid(SystemPoint screenPos)
        {
            if (_state == null) return (-1, -1);
            int col = (int)Math.Round((double)(screenPos.X - OffsetX) / _cellSize);
            int row = (int)Math.Round((double)(screenPos.Y - OffsetY) / _cellSize);
            return (row, col);
        }

        // Offset pour centrer la grille dans le panel
        private int OffsetX => (Width - (_state!.Columns - 1) * _cellSize) / 2;
        private int OffsetY => (Height - (_state!.Rows - 1) * _cellSize) / 2;

        private int GetX(int col) => OffsetX + col * _cellSize;
        private int GetY(int row) => OffsetY + row * _cellSize;
    }
}