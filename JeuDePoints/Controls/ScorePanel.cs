using System;
using System.Collections.Generic;
using System.Text;
using SystemPoint = System.Drawing.Point;

using JeuDePoints.Domain.Models;

namespace JeuDePoints.Controls
{
    public class ScorePanel : Panel
    {
        private Label _lblScore1;
        private Label _lblScore2;
        private Label _lblTurn;
        private Label _lblMove;

        public ScorePanel()
        {
            Height = 60;
            Dock = DockStyle.Top;
            BackColor = Color.FromArgb(30, 30, 30);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            _lblScore1 = new Label
            {
                ForeColor = Color.SteelBlue,
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new SystemPoint(20, 15)
            };
            _lblScore2 = new Label
            {
                ForeColor = Color.Crimson,
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new SystemPoint(200, 15)
            };
            _lblTurn = new Label
            {
                ForeColor = Color.White,
                Font = new Font("Arial", 11),
                AutoSize = true,
                Location = new SystemPoint(400, 10)
            };
            _lblMove = new Label
            {
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 9),
                AutoSize = true,
                Location = new SystemPoint(400, 35)
            };

            Controls.AddRange(new Control[] { _lblScore1, _lblScore2, _lblTurn, _lblMove });
        }

        public void Refresh(GameState state, int moveNumber)
        {
            _lblScore1.Text = $"{state.Player1Name} : {state.ScoreP1}";
            _lblScore2.Text = $"{state.Player2Name} : {state.ScoreP2}";
            _lblTurn.Text = $"Tour : {state.GetCurrentPlayerName()}";
            _lblMove.Text = $"Mouvement n°{moveNumber}";
        }
    }
}
