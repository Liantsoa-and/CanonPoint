using System;
using System.Collections.Generic;
using System.Text;
using SystemPoint = System.Drawing.Point;

using JeuDePoints.Domain.Models;
using JeuDePoints.Ui;

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
            Height = 68;
            BackColor = GameTheme.Surface;
            Padding = new Padding(10, 6, 10, 6);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            _lblScore1 = new Label
            {
                ForeColor = GameTheme.Player1,
                Font = new Font("Bahnschrift SemiBold", 14, FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _lblScore2 = new Label
            {
                ForeColor = GameTheme.Player2,
                Font = new Font("Bahnschrift SemiBold", 14, FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _lblTurn = new Label
            {
                ForeColor = Color.FromArgb(47, 57, 73),
                Font = GameTheme.UiFontBold,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _lblMove = new Label
            {
                ForeColor = Color.FromArgb(98, 111, 133),
                Font = GameTheme.UiFont,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            layout.Controls.Add(_lblScore1, 0, 0);
            layout.Controls.Add(_lblScore2, 1, 0);
            layout.Controls.Add(_lblTurn, 2, 0);
            layout.Controls.Add(_lblMove, 3, 0);
            Controls.Add(layout);
        }

        public void Refresh(GameState state, int moveNumber)
        {
            _lblScore1.Text = $"{state.Player1Name} : {state.ScoreP1}";
            _lblScore2.Text = $"{state.Player2Name} : {state.ScoreP2}";
            _lblTurn.Text = $"Tour : {state.GetCurrentPlayerName()}";
            _lblMove.Text = $"Mouvement n°{moveNumber}";

            _lblTurn.ForeColor = _lblTurn.Text.Contains(state.Player1Name, StringComparison.OrdinalIgnoreCase)
                ? GameTheme.Player1
                : GameTheme.Player2;
        }
    }
}
