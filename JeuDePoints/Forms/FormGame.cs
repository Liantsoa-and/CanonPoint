using System;
using System.Collections.Generic;
using System.Text;

using JeuDePoints.Controls;
using SystemPoint = System.Drawing.Point;
using JeuDePoints.Services;
using JeuDePoints.Domain.Models;
using JeuDePoints.Data.Repositories;

namespace JeuDePoints.Forms
{
    public class FormGame : Form
    {
        private GameState _state;
        private readonly GameService _gameService;

        private BoardPanel _boardPanel;
        private CannonWidget _cannon1;
        private CannonWidget _cannon2;
        private ScorePanel _scorePanel;
        private Button _btnEnd;
        private Button _btnUndo;
        private Button _btnReset;
        private Button _btnPrev;
        private Button _btnNext;
        private Label _lblReplay;
        private int _moveNumber = 0;
        private bool _isReadOnlyTimelineMode;
        private readonly bool _canEditAtLatest;
        private readonly SnapshotRepository? _snapshotRepo;
        private readonly List<int> _replayMoveNumbers;
        private int _replayIndex = -1;

        public FormGame(GameState state, GameService gameService, SnapshotRepository snapshotRepo)
        {
            _state = state;
            _gameService = gameService;
            _snapshotRepo = snapshotRepo;
            _isReadOnlyTimelineMode = false;
            _canEditAtLatest = true;
            _replayMoveNumbers = new List<int>();
            InitializeComponents();
            ReloadTimelineMoves(snapToLatest: true);
            RefreshAll();
            UpdateReplayControls();
        }

        public FormGame(
            GameState state,
            GameService gameService,
            SnapshotRepository snapshotRepo,
            List<int> replayMoveNumbers,
            int initialMoveNumber,
            bool canResumeFromLatest)
        {
            _state = state;
            _gameService = gameService;
            _snapshotRepo = snapshotRepo;
            _canEditAtLatest = canResumeFromLatest;
            _replayMoveNumbers = replayMoveNumbers.OrderBy(x => x).ToList();
            _moveNumber = initialMoveNumber;
            _replayIndex = _replayMoveNumbers.FindIndex(x => x == initialMoveNumber);
            if (_replayIndex < 0 && _replayMoveNumbers.Count > 0)
            {
                _replayIndex = _replayMoveNumbers.Count - 1;
                _moveNumber = _replayMoveNumbers[_replayIndex];
            }

            _isReadOnlyTimelineMode = !_canEditAtLatest || !IsAtLatestSnapshot();

            InitializeComponents();
            RefreshAll();
            UpdateReplayControls();
        }

        private void InitializeComponents()
        {
            Text = "Jeu de Points";
            Size = new Size(1100, 750);
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            KeyDown += FormGame_KeyDown;

            _scorePanel = new ScorePanel();
            Controls.Add(_scorePanel);

            // Barre de boutons
            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(45, 45, 45) };
            _btnEnd = new Button { Text = "Terminer", Location = new SystemPoint(20, 10), Size = new Size(120, 30), BackColor = Color.DarkRed, ForeColor = Color.White };
            _btnUndo = new Button { Text = "Restaurer (Ctrl+Z)", Location = new SystemPoint(160, 10), Size = new Size(150, 30), BackColor = Color.DarkOrange, ForeColor = Color.White };
            _btnReset = new Button { Text = "Réinitialiser", Location = new SystemPoint(330, 10), Size = new Size(120, 30), BackColor = Color.DarkGreen, ForeColor = Color.White };
            _btnPrev = new Button { Text = "Précédent", Location = new SystemPoint(470, 10), Size = new Size(120, 30), BackColor = Color.DimGray, ForeColor = Color.White, Visible = _snapshotRepo != null };
            _btnNext = new Button { Text = "Suivant", Location = new SystemPoint(600, 10), Size = new Size(120, 30), BackColor = Color.DimGray, ForeColor = Color.White, Visible = _snapshotRepo != null };
            _lblReplay = new Label { Text = "", Location = new SystemPoint(730, 14), Size = new Size(360, 22), ForeColor = Color.Gainsboro, Visible = _snapshotRepo != null };
            _btnEnd.Click += (s, e) => EndGame();
            _btnUndo.Click += (s, e) => UndoMove();
            _btnReset.Click += (s, e) => ResetGame();
            _btnPrev.Click += (s, e) => GoToPreviousSnapshot();
            _btnNext.Click += (s, e) => GoToNextSnapshot();
            btnPanel.Controls.AddRange(new Control[] { _btnEnd, _btnUndo, _btnReset, _btnPrev, _btnNext, _lblReplay });
            Controls.Add(btnPanel);

            // Canons (ajoutés AVANT le plateau — avec Dock, WinForms place
            // les Left/Right d'abord, puis Fill remplit ce qui reste)
            _cannon1 = new CannonWidget(1) { Dock = DockStyle.Left, Width = 80 };
            _cannon2 = new CannonWidget(2) { Dock = DockStyle.Right, Width = 80 };
            _cannon1.CannonMoved += OnCannonMoved;
            _cannon2.CannonMoved += OnCannonMoved;
            Controls.Add(_cannon2);
            Controls.Add(_cannon1);

            // Plateau (ajouté EN DERNIER pour que Dock=Fill prenne l'espace restant)
            _boardPanel = new BoardPanel { Dock = DockStyle.Fill };
            _boardPanel.IntersectionClicked += OnIntersectionClicked;
            Controls.Add(_boardPanel);

            if (_isReadOnlyTimelineMode)
            {
                Text = _canEditAtLatest
                    ? "Jeu de Points - Historique"
                    : "Jeu de Points - Replay";
                SetInteractiveControlsEnabled(false);
            }
            else if (_snapshotRepo != null)
            {
                Text = "Jeu de Points";
            }
        }

        private void SetInteractiveControlsEnabled(bool enabled)
        {
            _btnEnd.Enabled = enabled;
            _btnUndo.Enabled = enabled;
            _btnReset.Enabled = enabled;
            _cannon1.Enabled = enabled;
            _cannon2.Enabled = enabled;
        }

        private void RefreshAll()
        {
            _boardPanel.UpdateState(_state);
            _cannon1.UpdatePosition(_state.Cannons[1].RowPosition);
            _cannon2.UpdatePosition(_state.Cannons[2].RowPosition);
            _scorePanel.Refresh(_state, _moveNumber);
        }

        private void OnIntersectionClicked(int row, int col)
        {
            if (_isReadOnlyTimelineMode) return;
            if (_state.IsFinished()) return;
            try
            {
                _state = _gameService.PlacePoint(_state, row, col);
                _moveNumber++;
                ReloadTimelineMoves(snapToLatest: true);
                RefreshAll();
                UpdateReplayControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Action invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnCannonMoved(int playerId, Direction direction)
        {
            if (_isReadOnlyTimelineMode) return;
            _state = _gameService.MoveCannon(_state, playerId, direction);
            RefreshAll();
        }

        private void FormGame_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_isReadOnlyTimelineMode) return;
            if (_state.IsFinished()) return;
            if (!e.Control) return;

            // Touches du rang supérieur uniquement (pas le pavé numérique)
            int power = e.KeyCode switch
            {
                Keys.D1 => 1,
                Keys.D2 => 2,
                Keys.D3 => 3,
                Keys.D4 => 4,
                Keys.D5 => 5,
                Keys.D6 => 6,
                Keys.D7 => 7,
                Keys.D8 => 8,
                Keys.D9 => 9,
                _ => 0
            };

            if (power > 0)
            {
                e.SuppressKeyPress = true;
                try
                {
                    _state = _gameService.ShootCannon(_state, power);
                    _moveNumber++;
                    ReloadTimelineMoves(snapToLatest: true);
                    RefreshAll();
                    UpdateReplayControls();

                    if (!_state.LastShotWasValid)
                    {
                        MessageBox.Show(
                            $"Tir consomme sans effet.\nCible: (ligne {_state.LastShotTargetRow}, colonne {_state.LastShotTargetCol})\nRaison: {_state.LastShotMessage}",
                            "Tir non valide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Tir invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (e.KeyCode == Keys.Z)
            {
                e.SuppressKeyPress = true;
                UndoMove();
            }
        }

        private void UndoMove()
        {
            if (_isReadOnlyTimelineMode) return;
            try
            {
                _state = _gameService.UndoLastMove(_state);
                if (_moveNumber > 0) _moveNumber--;
                ReloadTimelineMoves(snapToLatest: true);
                RefreshAll();
                UpdateReplayControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Restauration impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ResetGame()
        {
            if (_isReadOnlyTimelineMode) return;
            if (MessageBox.Show("Réinitialiser la partie ?", "Confirmation",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _state = _gameService.ResetGame(_state);
                _moveNumber = 0;
                ReloadTimelineMoves(snapToLatest: true);
                RefreshAll();
                UpdateReplayControls();
            }
        }

        private void EndGame()
        {
            if (_isReadOnlyTimelineMode) return;
            _state = _gameService.EndGame(_state);
            string winner = _state.ScoreP1 > _state.ScoreP2 ? _state.Player1Name
                : _state.ScoreP2 > _state.ScoreP1 ? _state.Player2Name
                : "Égalité";
            MessageBox.Show(
                $"Partie terminée !\n\n{_state.Player1Name} : {_state.ScoreP1} point(s)\n{_state.Player2Name} : {_state.ScoreP2} point(s)\n\nVainqueur : {winner}",
                "Fin de partie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ReloadTimelineMoves(snapToLatest: true);
            RefreshAll();
            UpdateReplayControls();
        }

        private void GoToPreviousSnapshot()
        {
            if (_snapshotRepo == null) return;
            if (_replayIndex <= 0) return;

            _replayIndex--;
            LoadSnapshotByIndex();
        }

        private void GoToNextSnapshot()
        {
            if (_snapshotRepo == null) return;
            if (_replayIndex >= _replayMoveNumbers.Count - 1) return;

            _replayIndex++;
            LoadSnapshotByIndex();
        }

        private void LoadSnapshotByIndex()
        {
            if (_snapshotRepo == null) return;
            if (_replayIndex < 0 || _replayIndex >= _replayMoveNumbers.Count) return;

            int moveNumber = _replayMoveNumbers[_replayIndex];
            var snapshot = _snapshotRepo.GetSnapshot(_state.GameId, moveNumber);
            if (snapshot == null) return;

            var replayState = snapshot.ToGameState();
            replayState.GameId = _state.GameId;
            replayState.Rows = _state.Rows;
            replayState.Columns = _state.Columns;
            replayState.Player1Name = _state.Player1Name;
            replayState.Player2Name = _state.Player2Name;
            replayState.Status = _state.Status;

            _state = replayState;
            _moveNumber = moveNumber;
            RefreshAll();
            UpdateReplayControls();
        }

        private void UpdateReplayControls()
        {
            if (_snapshotRepo == null) return;

            _isReadOnlyTimelineMode = !_canEditAtLatest || !IsAtLatestSnapshot();
            SetInteractiveControlsEnabled(!_isReadOnlyTimelineMode);

            _btnPrev.Enabled = _replayIndex > 0;
            _btnNext.Enabled = _replayIndex >= 0 && _replayIndex < _replayMoveNumbers.Count - 1;

            string modeLabel = _isReadOnlyTimelineMode
                ? "Lecture seule (ancien mouvement)"
                : (_canEditAtLatest ? "Editable (dernier mouvement)" : "Replay");
            _lblReplay.Text = $"Historique: coup {_moveNumber} / {(_replayMoveNumbers.Count == 0 ? 0 : _replayMoveNumbers.Last())} - {modeLabel}";

            if (_canEditAtLatest)
                Text = _isReadOnlyTimelineMode ? "Jeu de Points - Historique" : "Jeu de Points";
            else
                Text = "Jeu de Points - Replay";
        }

        private bool IsAtLatestSnapshot()
        {
            return _replayMoveNumbers.Count > 0 && _replayIndex == _replayMoveNumbers.Count - 1;
        }

        private void ReloadTimelineMoves(bool snapToLatest)
        {
            if (_snapshotRepo == null) return;

            var moves = _snapshotRepo.GetSnapshotMoveNumbers(_state.GameId);
            _replayMoveNumbers.Clear();
            _replayMoveNumbers.AddRange(moves.OrderBy(x => x));

            if (_replayMoveNumbers.Count == 0)
            {
                _replayIndex = -1;
                return;
            }

            if (snapToLatest)
            {
                _replayIndex = _replayMoveNumbers.Count - 1;
                _moveNumber = _replayMoveNumbers[_replayIndex];
                return;
            }

            int idx = _replayMoveNumbers.FindIndex(x => x == _moveNumber);
            _replayIndex = idx >= 0 ? idx : _replayMoveNumbers.Count - 1;
            _moveNumber = _replayMoveNumbers[_replayIndex];
        }
    }
}