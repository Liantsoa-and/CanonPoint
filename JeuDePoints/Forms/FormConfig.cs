using System;
using System.Collections.Generic;
using System.Text;

using JeuDePoints.Data.Repositories;
using JeuDePoints.Data;
using JeuDePoints.Services;
using JeuDePoints.Domain.Models;
using JeuDePoints.Ui;


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
        private readonly CannonRepository _cannonRepo;
        private readonly PointRepository _pointRepo;
        private readonly ValidatedLineRepository _lineRepo;
        private readonly BlockedCellRepository _blockedRepo;
        private readonly MoveRepository _moveRepo;
        private readonly SnapshotRepository _snapshotRepo;
        private readonly GameService _gameService;

        public FormConfig()
        {
            _db = new DatabaseConnection();
            _gameRepo = new GameRepository(_db);
            _cannonRepo = new CannonRepository(_db);
            _pointRepo = new PointRepository(_db);
            _lineRepo = new ValidatedLineRepository(_db);
            _blockedRepo = new BlockedCellRepository(_db);
            _moveRepo = new MoveRepository(_db);
            _snapshotRepo = new SnapshotRepository(_db);
            _gameService = new GameService(
                _gameRepo,
                _cannonRepo,
                _pointRepo,
                _lineRepo,
                _blockedRepo,
                _moveRepo,
                _snapshotRepo);
            InitializeComponents();
            LoadGamesList();
        }

        private void InitializeComponents()
        {
            Text = "Jeu de Points — Configuration";
            Size = new Size(1080, 700);
            MinimumSize = new Size(980, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = GameTheme.WindowBackground;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = GameTheme.Surface,
                Padding = new Padding(18, 14, 18, 12)
            };

            var lblTitle = new Label
            {
                Text = "Jeu de Points",
                Font = GameTheme.TitleFont,
                ForeColor = Color.FromArgb(29, 39, 56),
                Dock = DockStyle.Top,
                Height = 38
            };

            var lblSubtitle = new Label
            {
                Text = "Configure une nouvelle partie ou reprends une session existante.",
                Font = GameTheme.UiFont,
                ForeColor = Color.FromArgb(101, 115, 138),
                Dock = DockStyle.Top,
                Height = 24
            };

            header.Controls.Add(lblSubtitle);
            header.Controls.Add(lblTitle);
            root.Controls.Add(header, 0, 0);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));

            var setupCard = new Panel { Dock = DockStyle.Fill };
            GameTheme.StyleCard(setupCard);
            var setupLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                BackColor = Color.Transparent
            };
            setupLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
            setupLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            setupLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

            var lblSetupTitle = new Label
            {
                Text = "Nouvelle partie",
                Font = GameTheme.SectionFont,
                ForeColor = Color.FromArgb(35, 48, 69),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            setupLayout.Controls.Add(lblSetupTitle, 0, 0);
            setupLayout.SetColumnSpan(lblSetupTitle, 2);

            setupLayout.Controls.Add(CreateFieldLabel("Nombre de lignes"), 0, 1);
            _numRows = new NumericUpDown { Minimum = 2, Maximum = 50, Value = 15, Dock = DockStyle.Fill, Font = GameTheme.UiFont };
            StyleInput(_numRows);
            setupLayout.Controls.Add(_numRows, 1, 1);

            setupLayout.Controls.Add(CreateFieldLabel("Nombre de colonnes"), 0, 2);
            _numCols = new NumericUpDown { Minimum = 2, Maximum = 50, Value = 20, Dock = DockStyle.Fill, Font = GameTheme.UiFont };
            StyleInput(_numCols);
            setupLayout.Controls.Add(_numCols, 1, 2);

            setupLayout.Controls.Add(CreateFieldLabel("Nom Joueur 1"), 0, 3);
            _txtPlayer1 = new TextBox { Text = "Joueur 1", Dock = DockStyle.Fill, Font = GameTheme.UiFont };
            StyleInput(_txtPlayer1);
            setupLayout.Controls.Add(_txtPlayer1, 1, 3);

            setupLayout.Controls.Add(CreateFieldLabel("Nom Joueur 2"), 0, 4);
            _txtPlayer2 = new TextBox { Text = "Joueur 2", Dock = DockStyle.Fill, Font = GameTheme.UiFont };
            StyleInput(_txtPlayer2);
            setupLayout.Controls.Add(_txtPlayer2, 1, 4);

            _lblError = new Label
            {
                ForeColor = GameTheme.Danger,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GameTheme.UiFont,
                Text = ""
            };
            setupLayout.Controls.Add(_lblError, 0, 5);
            setupLayout.SetColumnSpan(_lblError, 2);

            _btnStart = new Button
            {
                Text = "Démarrer la partie",
                Dock = DockStyle.Fill,
                Height = 42
            };
            GameTheme.StylePrimaryButton(_btnStart);
            _btnStart.Click += BtnStart_Click;
            setupLayout.Controls.Add(_btnStart, 0, 6);
            setupLayout.SetColumnSpan(_btnStart, 2);
            setupCard.Controls.Add(setupLayout);

            var listCard = new Panel { Dock = DockStyle.Fill };
            GameTheme.StyleCard(listCard);
            var listLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            listLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblGames = new Label
            {
                Text = "Parties existantes",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GameTheme.SectionFont,
                ForeColor = Color.FromArgb(35, 48, 69)
            };
            listLayout.Controls.Add(lblGames, 0, 0);

            _gridGames = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(224, 231, 243),
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 34 }
            };
            StyleGrid(_gridGames);

            _gridGames.CellContentClick += GridGames_CellContentClick;

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

            _gridGames.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Statut",
                Width = 110
            });

            _gridGames.Columns.Add(new DataGridViewButtonColumn
            {
                DataPropertyName = "ActionLabel",
                HeaderText = "Action",
                Width = 110,
                UseColumnTextForButtonValue = false,
                Name = "ActionColumn"
            });

            listLayout.Controls.Add(_gridGames, 0, 1);
            listCard.Controls.Add(listLayout);

            content.Controls.Add(setupCard, 0, 0);
            content.Controls.Add(listCard, 1, 0);
            root.Controls.Add(content, 0, 1);
            Controls.Add(root);
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GameTheme.UiFontBold,
                ForeColor = Color.FromArgb(54, 67, 88)
            };
        }

        private static void StyleInput(Control control)
        {
            control.BackColor = Color.FromArgb(248, 250, 255);
            control.ForeColor = Color.FromArgb(44, 55, 73);
            control.Margin = new Padding(2, 5, 2, 5);
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(235, 241, 251),
                ForeColor = Color.FromArgb(39, 51, 71),
                Font = GameTheme.UiFontBold,
                SelectionBackColor = Color.FromArgb(214, 226, 247),
                SelectionForeColor = Color.FromArgb(39, 51, 71)
            };

            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(46, 59, 80),
                SelectionBackColor = Color.FromArgb(219, 232, 252),
                SelectionForeColor = Color.FromArgb(34, 47, 66),
                Font = GameTheme.UiFont
            };
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

        private void GridGames_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_gridGames.Columns[e.ColumnIndex].Name != "ActionColumn") return;

            if (_gridGames.Rows[e.RowIndex].DataBoundItem is not GameListRow row)
                return;

            if (string.Equals(row.Status, "finished", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    int latestMove;
                    var state = _gameService.LoadLatestSnapshotState(row.GameId, out latestMove);
                    var moves = _snapshotRepo.GetSnapshotMoveNumbers(row.GameId);

                    var replayForm = new FormGame(
                        state,
                        _gameService,
                        _snapshotRepo,
                        moves,
                        latestMove,
                        false);

                    replayForm.Show();
                    Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Replay impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return;
            }

            try
            {
                int latestMove;
                var state = _gameService.LoadLatestSnapshotState(row.GameId, out latestMove);
                var moves = _snapshotRepo.GetSnapshotMoveNumbers(row.GameId);

                var continueForm = new FormGame(
                    state,
                    _gameService,
                    _snapshotRepo,
                    moves,
                    latestMove,
                    true);

                continueForm.Show();
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chargement impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            if (_numRows.Value < 2 || _numCols.Value < 2)
            {
                _lblError.Text = "Le plateau doit avoir au moins 2 lignes et 2 colonnes.";
                return;
            }

            var state = _gameService.CreateNewGame(
                (int)_numRows.Value, (int)_numCols.Value,
                _txtPlayer1.Text.Trim() == "" ? "Joueur 1" : _txtPlayer1.Text.Trim(),
                _txtPlayer2.Text.Trim() == "" ? "Joueur 2" : _txtPlayer2.Text.Trim());

            LoadGamesList();

            var formGame = new FormGame(state, _gameService, _snapshotRepo);
            formGame.Show();
            Hide();
        }
    }
}
