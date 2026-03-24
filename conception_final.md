# JeuDePoints - Conception Finale (fusion)

Date: 2026-03-24
Statut: reference unique pour implementation
Stack cible: C# WinForms + PostgreSQL (Npgsql, SQL brut)
Framework cible: .NET 10 (net10.0)

## 1. Objectif

Ce document fusionne les conceptions precedentes et les reponses de validation.
Il definit une regle metier unique, un modele technique clair, et une base exploitable sans ambiguite majeure pour coder le jeu.

## 2. Regles de jeu finales

### 2.1 Principes generaux

- Jeu a 2 joueurs, tour par tour.
- Le plateau est une grille de croisements (pas des cases).
- A son tour, un joueur fait exactement une action:
  - placer un point, ou
  - tirer avec son canon.
- Couleurs UI recommandees:
  - Joueur 1: Bleu
  - Joueur 2: Rouge

### 2.2 Placement

- Un point peut etre place sur tout croisement vide.
- Un croisement occupe est interdit.
- Les points invulnerables adverses ne bloquent pas le placement ailleurs.
- Les "murs logiques" n agissent que sur la validation des lignes de 5.

### 2.3 Validation des lignes de 5

- Directions autorisees:
  - horizontale
  - verticale
  - diagonale montante
  - diagonale descendante
- Une ligne est validee si:
  - elle contient 5 points consecutifs du meme joueur,
  - et aucune des 5 positions de cette ligne ne contient un point invulnerable adverse.
- Une ligne validee rapporte +1 point.
- Les points d une ligne validee deviennent invulnerables.
- Une meme ligne ne peut jamais etre recomptee.
- Reutilisation de points autorisee pour former d autres lignes.
- Regle gomoku stricte retenue:
  - le score se compte par lignes de 5 distinctes,
  - pas de bonus special pour 6+ au dela de la validation de lignes distinctes de longueur 5.

### 2.4 Invulnerabilite

- Un point est invulnerable uniquement s il appartient a au moins une ligne deja validee.
- Un point invulnerable reste visible et reste sur le plateau.
- Un point invulnerable peut etre cible par un tir, mais le tir n a aucun effet.

### 2.5 Canon et tir

- Canon Joueur 1: bord gauche.
- Canon Joueur 2: bord droit.
- Deplacement canon: vertical uniquement.
- Resolution de tir: impact direct sur la cible (pas de simulation de trajectoire bloquee).

#### Calcul de colonne cible

Formule retenue:

colonne_visee = FLOOR((puissance * nombre_de_colonnes) / 9)

Contraintes:

- puissance entre 1 et 9
- clamp minimum a 1
- clamp maximum a nombre_de_colonnes

#### Effet du tir

A la cible (ligne canon, colonne_visee):

- croisement vide: aucun effet, action consommee
- point du joueur courant: aucun effet, action consommee
- point adverse non invulnerable: point detruit, action consommee
- point adverse invulnerable: aucun effet, action consommee

Le score ne baisse jamais apres destruction.

### 2.6 Fin de partie

- Fin officielle via bouton manuel "Terminer partie".
- A la fin, afficher gagnant (ou egalite).

### 2.7 Passage de tour (UX)

- Si aucune action utile n est possible pour le joueur courant, UI propose "Passer le tour".
- Definition "action utile" dans cette conception:
  - aucun placement possible (aucun croisement vide),
  - et aucun tir ne peut detruire un point adverse non invulnerable.
- Le passage de tour est trace comme un move de type PASS.

## 3. Conventions de modele

### 3.1 Coordonnees

Decision technique retenue pour implementation:

- interne moteur: index 0-based (row 0..R-1, col 0..C-1)
- UI: peut afficher en 1-based si besoin visuel
- DB: stocke les index internes 0-based pour eviter les conversions permanentes

### 3.2 Etat d un point

- owner_player_id: 1 ou 2
- is_active: true si present sur le plateau
- is_invulnerable: derive de l appartenance a une ligne validee

Note:
- is_invulnerable peut etre derive a la lecture via validated_lines.
- ne pas dupliquer inutilement cet etat si cela cree un risque d incoherence.

## 4. Persistance PostgreSQL

## 4.1 Principes valides

- Tout est conserve en base:
  - partie
  - mouvements
  - score
  - etat courant
  - snapshots
- Source de verite: table moves.
- table game_snapshots: acceleration de undo/rechargement, pas source principale.
- Undo: rollback via snapshot N-1.

## 4.2 Tables cibles

### games

- id (PK)
- rows
- columns
- player1_name
- player2_name
- current_turn
- score_p1
- score_p2
- status (ongoing | finished)
- created_at
- updated_at

### cannons

- id (PK)
- game_id (FK games)
- player_id (1|2)
- row_position
- UNIQUE(game_id, player_id)

### points

- id (PK)
- game_id (FK games)
- player_id
- row
- col
- is_active
- created_at
- destroyed_at nullable

Contrainte recommandee:

- UNIQUE(game_id, row, col)

Comportement associe:

- si point detruit, mettre is_active = false
- un nouveau placement au meme croisement doit reactiver/enrichir la meme ligne point (update), pas inserer un doublon

### validated_lines

- id (PK)
- game_id (FK games)
- player_id
- direction (horizontal|vertical|diag_asc|diag_desc)
- start_row
- start_col
- end_row
- end_col
- line_key (unique canonique)
- created_at

Contrainte:

- UNIQUE(game_id, line_key)

Definition de line_key:

- coordonnees des 5 croisements triees, concatenees de facon deterministe
- exemple: r1c2|r2c3|r3c4|r4c5|r5c6

### blocked_cells

- id (PK)
- game_id (FK games)
- validated_line_id (FK validated_lines)
- row
- col
- blocking_player_id

Usage:

- utile pour rendu UI rapide
- peut etre derive de validated_lines si necessaire

### moves

Table unique d historique des actions (pas de table shots separee).

- id (PK)
- game_id (FK games)
- move_number (strictement croissant par partie)
- player_id
- move_type (PLACE|SHOOT|PASS|END)
- row nullable
- col nullable
- cannon_row nullable
- power nullable
- target_row nullable
- target_col nullable
- hit nullable
- score_p1_snapshot
- score_p2_snapshot
- created_at

Contrainte:

- UNIQUE(game_id, move_number)

### game_snapshots

- id (PK)
- game_id (FK games)
- move_number
- snapshot_data (JSONB)
- created_at

Contrainte:

- UNIQUE(game_id, move_number)

## 4.3 JSON snapshot minimal

{
  "current_turn": 1,
  "score_p1": 0,
  "score_p2": 0,
  "cannons": {
    "1": { "row_position": 0 },
    "2": { "row_position": 0 }
  },
  "points": [
    { "player_id": 1, "row": 3, "col": 5, "is_active": true }
  ],
  "validated_lines": [
    {
      "player_id": 1,
      "direction": "horizontal",
      "start_row": 3,
      "start_col": 1,
      "end_row": 3,
      "end_col": 5,
      "line_key": "r3c1|r3c2|r3c3|r3c4|r3c5"
    }
  ]
}

## 5. Flux applicatif standard

### 5.1 Tour PLACE

1. verifier croisement vide
2. appliquer placement (insert/update points)
3. detecter nouvelles lignes candidates autour du point place
4. filtrer lignes bloquees par invulnerables adverses
5. inserer lignes valides non deja comptees (line_key)
6. mettre a jour score
7. inserer move PLACE
8. inserer snapshot
9. changer de joueur

### 5.2 Tour SHOOT

1. lire cannon_row du joueur
2. calculer target_col via puissance
3. evaluer cible
4. si point adverse actif non invulnerable: desactiver point
5. inserer move SHOOT
6. inserer snapshot
7. changer de joueur

### 5.3 Undo

1. lire dernier move_number N
2. charger snapshot N-1
3. restaurer etat complet
4. ne pas supprimer move N (et snapshot N si present), accessible via bouton suivant

## 6. Architecture logicielle cible

- UI WinForms:
  - rendu plateau, interactions souris/clavier, animations, messages
- Domain:
  - entites, value objects, regles pures (testables sans UI)
- Services:
  - orchestration d un tour (place/shoot/pass/end), scoring, validation
- Infrastructure:
  - repositories SQL brut Npgsql

Regle d or:

- aucune regle metier dans les event handlers UI

## 7. Contrats UX

- Placement au clic immediat sur croisement valide.
- Tir via Ctrl+1..Ctrl+9 (rang superieur clavier, pas pave numerique).
- Message clair si action sans effet.
- Proposition de "Passer le tour" seulement si aucune action utile possible.
- Affichage permanent:
  - score p1/p2
  - joueur actif
  - numero de move

## 8. Invariants techniques

- move_number unique et croissant par game_id.
- score_p1 et score_p2 monotones non decroissants.
- une ligne validee unique par line_key.
- aucun couplage direct UI -> SQL sans passer service/repository.
- une transaction DB par action joueur.

## 9. Criteres d acceptance

1. 5 points alignes valident une ligne et donnent +1.
2. Une ligne contenant un point invulnerable adverse ne se valide pas.
3. Tir sur point adverse invulnerable: aucun effet, tour consomme.
4. Tir sur point adverse non invulnerable: point detruit, score inchange.
5. Un point detruit peut etre repose plus tard sur le meme croisement.
6. Une meme ligne de 5 n est jamais recomptee.
7. Undo restaure strictement l etat snapshot N-1.
8. Fin manuelle annonce gagnant ou egalite.
9. Build et execution en net10.0 sans erreur bloquante.

## 10. Decisions fermees recap

- placement sur tout croisement vide: OUI
- mur logique bloque validation, pas placement: OUI
- invulnerable cible possible, tir sans effet: OUI
- tir impact direct sur cible: OUI
- fin manuelle par bouton: OUI
- historique unique dans moves: OUI
- source de verite = moves, snapshots pour acceleration: OUI
- acces DB = Npgsql SQL brut: OUI
- framework = net10.0: OUI
- proposition passer tour si aucune action utile possible: OUI

---

Fin de document - conception finale validee pour implementation.
