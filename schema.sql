DROP TABLE IF EXISTS game_snapshots CASCADE;
DROP TABLE IF EXISTS moves CASCADE;
DROP TABLE IF EXISTS blocked_cells CASCADE;
DROP TABLE IF EXISTS validated_lines CASCADE;
DROP TABLE IF EXISTS points CASCADE;
DROP TABLE IF EXISTS cannons CASCADE;
DROP TABLE IF EXISTS games CASCADE;

CREATE TABLE games (
    id SERIAL PRIMARY KEY,
    rows INT NOT NULL,
    columns INT NOT NULL,
    player1_name VARCHAR(100) NOT NULL DEFAULT 'Joueur 1',
    player2_name VARCHAR(100) NOT NULL DEFAULT 'Joueur 2',
    current_turn SMALLINT NOT NULL DEFAULT 1,
    score_p1 INT NOT NULL DEFAULT 0,
    score_p2 INT NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'ongoing',
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE cannons (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    player_id SMALLINT NOT NULL,
    row_position INT NOT NULL DEFAULT 0,
    UNIQUE (game_id, player_id)
);

CREATE TABLE points (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    player_id SMALLINT NOT NULL,
    row INT NOT NULL,
    col INT NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE (game_id, row, col)
);

CREATE INDEX idx_points_game_active ON points(game_id, is_active);
CREATE INDEX idx_points_game_player ON points(game_id, player_id);

CREATE TABLE validated_lines (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    player_id SMALLINT NOT NULL,
    direction VARCHAR(15) NOT NULL,
    start_row INT NOT NULL,
    start_col INT NOT NULL,
    end_row INT NOT NULL,
    end_col INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_vlines_game_player ON validated_lines(game_id, player_id);

CREATE TABLE blocked_cells (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    validated_line_id INT NOT NULL REFERENCES validated_lines(id) ON DELETE CASCADE,
    row INT NOT NULL,
    col INT NOT NULL,
    blocking_player_id SMALLINT NOT NULL
);

CREATE INDEX idx_bcells_game_pos ON blocked_cells(game_id, row, col);

CREATE TABLE moves (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    move_number INT NOT NULL,
    player_id SMALLINT NOT NULL,
    move_type VARCHAR(10) NOT NULL,
    row INT,
    col INT,
    cannon_row INT,
    power SMALLINT,
    target_row INT,
    target_col INT,
    hit BOOLEAN,
    score_p1_snapshot INT NOT NULL DEFAULT 0,
    score_p2_snapshot INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE (game_id, move_number)
);

CREATE INDEX idx_moves_game_number ON moves(game_id, move_number DESC);

CREATE TABLE game_snapshots (
    id SERIAL PRIMARY KEY,
    game_id INT NOT NULL REFERENCES games(id) ON DELETE CASCADE,
    move_number INT NOT NULL,
    snapshot_data JSONB NOT NULL,
    UNIQUE (game_id, move_number)
);

CREATE INDEX idx_snapshots_game_move ON game_snapshots(game_id, move_number DESC);

CREATE OR REPLACE FUNCTION fn_update_games_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_games_updated_at
    BEFORE UPDATE ON games
    FOR EACH ROW
    EXECUTE FUNCTION fn_update_games_timestamp();