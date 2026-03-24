using System;
using System.Collections.Generic;
using System.Text;

using JeuDePoints.Data.Repositories;
using JeuDePoints.Data;
using JeuDePoints.Services;
using JeuDePoints.Domain.Models;


namespace JeuDePoints.Forms
{
    public class FormConfig : Form
    {
        private NumericUpDown _numRows;
        private NumericUpDown _numCols;
        private TextBox _txtPlayer1;
        private TextBox _txtPlayer2;
        private Button _btnStart;
        private Label _lblError;
        private DataGridView _gridGames;

        private readonly DatabaseConnection _db;
        private readonly GameRepository _gameRepo;

        public FormConfig()
        {
            _db = new DatabaseConnection();
            _gameRepo = new GameRepository(_db);
            InitializeComponents();
            LoadGamesList();
        }

        private void InitializeComponents()
        {
            Text = "Jeu de Points — Configuration";
            Size = new Size(860, 560);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 8,
                ColumnCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            layout.Controls.Add(new Label { Text = "Nombre de lignes :", Anchor = AnchorStyles.Left | AnchorStyles.Right }, 0, 0);
            _numRows = new NumericUpDown { Minimum = 2, Maximum = 50, Value = 15, Dock = DockStyle.Fill };
            layout.Controls.Add(_numRows, 1, 0);

            layout.Controls.Add(new Label { Text = "Nombre de colonnes :", Anchor = AnchorStyles.Left | AnchorStyles.Right }, 0, 1);
            _numCols = new NumericUpDown { Minimum = 2, Maximum = 50, Value = 20, Dock = DockStyle.Fill };
            layout.Controls.Add(_numCols, 1, 1);

            layout.Controls.Add(new Label { Text = "Nom Joueur 1 :", Anchor = AnchorStyles.Left | AnchorStyles.Right }, 0, 2);
            _txtPlayer1 = new TextBox { Text = "Joueur 1", Dock = DockStyle.Fill };
            layout.Controls.Add(_txtPlayer1, 1, 2);

            layout.Controls.Add(new Label { Text = "Nom Joueur 2 :", Anchor = AnchorStyles.Left | AnchorStyles.Right }, 0, 3);
            _txtPlayer2 = new TextBox { Text = "Joueur 2", Dock = DockStyle.Fill };
            layout.Controls.Add(_txtPlayer2, 1, 3);

            _lblError = new Label { ForeColor = Color.Red, Dock = DockStyle.Fill, Text = "" };
            layout.Controls.Add(_lblError, 0, 4);
            layout.SetColumnSpan(_lblError, 2);

            _btnStart = new Button
            {
                Text = "Démarrer la partie",
                Dock = DockStyle.Fill,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            _btnStart.Click += BtnStart_Click;
            layout.Controls.Add(_btnStart, 0, 5);
            layout.SetColumnSpan(_btnStart, 2);

            var lblGames = new Label
            {
                Text = "Parties existantes",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            layout.Controls.Add(lblGames, 0, 6);
            layout.SetColumnSpan(lblGames, 2);

            _gridGames = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            _gridGames.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GameId",
                HeaderText = "Game ID",
                Width = 90
            });

            _gridGames.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GameName",
                HeaderText = "Nom du game",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            _gridGames.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Score",
                HeaderText = "Score",
                Width = 120
            });

            layout.Controls.Add(_gridGames, 0, 7);
            layout.SetColumnSpan(_gridGames, 2);

            Controls.Add(layout);
        }

        private void LoadGamesList()
        {
            try
            {
                var rows = _gameRepo.GetGamesForStartScreen();
                _gridGames.DataSource = rows;
                _lblError.Text = "";
            }
            catch (Exception ex)
            {
                _lblError.Text = "Impossible de charger les parties: " + ex.Message;
            }
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            if (_numRows.Value < 2 || _numCols.Value < 2)
            {
                _lblError.Text = "Le plateau doit avoir au moins 2 lignes et 2 colonnes.";
                return;
            }

            var gameService = new GameService(
                _gameRepo, new CannonRepository(_db),
                new PointRepository(_db), new ValidatedLineRepository(_db),
                new BlockedCellRepository(_db), new MoveRepository(_db),
                new SnapshotRepository(_db));

            var state = gameService.CreateNewGame(
                (int)_numRows.Value, (int)_numCols.Value,
                _txtPlayer1.Text.Trim() == "" ? "Joueur 1" : _txtPlayer1.Text.Trim(),
                _txtPlayer2.Text.Trim() == "" ? "Joueur 2" : _txtPlayer2.Text.Trim());

            LoadGamesList();

            var formGame = new FormGame(state, gameService);
            formGame.Show();
            Hide();
        }
    }
}
