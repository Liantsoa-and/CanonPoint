using System;
using System.Collections.Generic;
using System.Text;
using JeuDePoints.Domain.Models;
using JeuDePoints.Services;

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
            Size = new Size(80, 120);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var color = _playerId == 1 ? Color.SteelBlue : Color.Crimson;

            _btnUp = new Button
            {
                Text = "▲",
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = color,
                ForeColor = Color.White
            };
            _btnUp.Click += (s, e) => CannonMoved?.Invoke(_playerId, Direction.Up);

            _lblPosition = new Label
            {
                Text = $"J{_playerId}\nLigne 0",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            _btnDown = new Button
            {
                Text = "▼",
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = color,
                ForeColor = Color.White
            };
            _btnDown.Click += (s, e) => CannonMoved?.Invoke(_playerId, Direction.Down);

            Controls.Add(_lblPosition);
            Controls.Add(_btnUp);
            Controls.Add(_btnDown);
        }

        public void UpdatePosition(int row)
        {
            _currentRow = row;
            _lblPosition.Text = $"J{_playerId}\nLigne {row}";
        }
    }
}
