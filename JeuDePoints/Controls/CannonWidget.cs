using System;
using System.Collections.Generic;
using System.Text;
using JeuDePoints.Domain.Models;
using JeuDePoints.Services;
using JeuDePoints.Ui;

namespace JeuDePoints.Controls
{
    public class CannonWidget : Panel
    {
        private readonly int _playerId;
        private int _currentRow;
        private Button _btnUp;
        private Button _btnDown;
        private Label _lblPosition;

        public event Action<int, Direction>? CannonMoved;

        public CannonWidget(int playerId)
        {
            _playerId = playerId;
            Width = 128;
            BackColor = GameTheme.Surface;
            Padding = new Padding(10);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var color = _playerId == 1 ? GameTheme.Player1 : GameTheme.Player2;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));

            _btnUp = new Button
            {
                Text = "Monter",
                Dock = DockStyle.Fill,
                BackColor = color,
                ForeColor = Color.White
            };
            GameTheme.StyleNeutralButton(_btnUp);
            _btnUp.BackColor = color;
            _btnUp.Click += (s, e) => CannonMoved?.Invoke(_playerId, Direction.Up);

            _lblPosition = new Label
            {
                Text = $"Canon J{_playerId}\nLigne 0",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = GameTheme.UiFontBold,
                ForeColor = Color.FromArgb(49, 60, 78)
            };

            _btnDown = new Button
            {
                Text = "Descendre",
                Dock = DockStyle.Fill,
                BackColor = color,
                ForeColor = Color.White
            };
            GameTheme.StyleNeutralButton(_btnDown);
            _btnDown.BackColor = color;
            _btnDown.Click += (s, e) => CannonMoved?.Invoke(_playerId, Direction.Down);

            layout.Controls.Add(_btnUp, 0, 0);
            layout.Controls.Add(_lblPosition, 0, 1);
            layout.Controls.Add(_btnDown, 0, 2);
            Controls.Add(layout);
        }

        public void UpdatePosition(int row)
        {
            _currentRow = row;
            _lblPosition.Text = $"J{_playerId}\nLigne {row}";
        }
    }
}
