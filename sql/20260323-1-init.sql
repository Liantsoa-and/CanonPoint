-- ============================================
-- SCRIPT DE RÉINITIALISATION COMPLÈTE
-- ============================================

-- 1. Se déconnecter de la base courante et revenir à postgres
\c postgres

-- 2. Supprimer la base si elle existe
DROP DATABASE IF EXISTS game_db;

-- 3. Créer une nouvelle base
CREATE DATABASE game_db;

-- 4. Se connecter à la nouvelle base
\c game_db

-- 5. Créer toutes les tables
-- Table des statuts (En cours, Terminé)
CREATE TABLE statuses (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL
);

-- Table des parties
CREATE TABLE games (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100),
    status_id INT REFERENCES statuses(id),
    grid_rows INT DEFAULT 20,
    grid_cols INT DEFAULT 20
);

-- Table des joueurs
CREATE TABLE players (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100),
    color_hex VARCHAR(7) -- Pour l'affichage UI
);

-- Table des points placés
CREATE TABLE points (
    id SERIAL PRIMARY KEY,
    game_id INT REFERENCES games(id),
    owner_id INT REFERENCES players(id),
    row INT NOT NULL,
    col INT NOT NULL,
    is_invulnerable BOOLEAN DEFAULT FALSE -- Flag pour le "mur logique"
);

-- Table des tirs de canon
CREATE TABLE shots (
    id SERIAL PRIMARY KEY,
    game_id INT REFERENCES games(id),
    player_id INT REFERENCES players(id),
    target_row INT,
    target_col INT,
    power INT CHECK (power BETWEEN 1 AND 9)
);

-- Table historique (L'Event Store)
CREATE TABLE moves (
    id SERIAL PRIMARY KEY,
    game_id INT REFERENCES games(id),
    player_id INT REFERENCES players(id),
    sequence_number INT NOT NULL, -- Votre chronologie en INT
    point_id INT REFERENCES points(id) NULL,
    shot_id INT REFERENCES shots(id) NULL,
    is_shot BOOLEAN DEFAULT FALSE,
    UNIQUE(game_id, sequence_number) -- Un seul mouvement par rang chronologique
);

-- 6. Afficher un message de confirmation
\echo '========================================='
\echo 'BASE RÉINITIALISÉE AVEC SUCCÈS !'
\echo 'Tables créées :'
\dt
\echo '========================================='