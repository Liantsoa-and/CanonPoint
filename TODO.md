# Projet : Jeu de Stratégie Gomoku-Canon (WinForms & PostgreSQL)

## 🎯 Objectifs
Développer un jeu de plateau où l'on aligne 5 points, avec un système de "mur" invulnérable et un canon destructeur.

## 🛠 Fonctionnalités Clés
- [ ] **Moteur de Jeu**
    - [ ] Système de placement sur grille (WinForms).
    - [ ] Algorithme de détection de ligne de 5 (Scan 8 directions).
    - [ ] Règle du Mur : Un segment de 5 devient `is_invulnerable`.
    - [ ] Règle d'Intersection : Impossible de valider une ligne si elle croise un point invulnérable adverse.
    - [ ] Fin de partie : Détection automatique si `TotalPoints == (Rows * Cols)`.

- [ ] **Canon & Pouvoirs**
    - [ ] Tir de canon (Puissance 1-9).
    - [ ] Suppression de points adverses en base de données (si non-invulnérables).

- [ ] **Gestion des Parties & Persistance (PostgreSQL)**
    - [ ] Sauvegarde automatique de chaque mouvement via `sequence_number`.
    - [ ] Menu Launcher : Lister les parties (Continuer / Revoir).

- [ ] **Système de Replay**
    - [ ] Mode Spectateur : Navigation `[<]` et `[>]`.
    - [ ] Reconstruction de l'état du plateau à partir de la liste des `moves`.

## 🎨 Design (UI/UX)
- [ ] Thème : Monochrome (Gris, Blanc, Noir).
- [ ] Feedback visuel : Points normaux vs Points "Murés".