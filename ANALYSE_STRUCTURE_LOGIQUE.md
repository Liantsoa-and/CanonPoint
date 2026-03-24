# JeuDePoints v2 - Structure et logique complete

Date: 2026-03-24

## 1. Vue d'ensemble

Le projet est organise en couches:

- UI WinForms: projet `JeuDePoints`
- Logique metier: projet `JeuDePoints.Services`
- Modeles metier: projet `JeuDePoints.Domain`
- Acces donnees PostgreSQL: projet `JeuDePoints.Data`
- Projet `JeuDePoints.Tests` present mais sans tests metier exploites

La circulation principale est:

1. UI capte une action utilisateur (clic, touche, bouton).
2. UI appelle `GameService`.
3. `GameService` valide avec `TurnManager` et services metier (`AlignmentChecker`, `CannonService`).
4. `GameService` persiste via repositories SQL (`GameRepository`, `PointRepository`, etc.).
5. `GameService` retourne un `GameState` mis a jour.
6. UI rafraichit l'affichage a partir de `GameState`.

## 2. Structure par projet

## 2.1 Projet `JeuDePoints` (UI)

### `Program.cs`
- Point d'entree WinForms.
- Initialise l'application et ouvre `FormConfig`.

### `Forms/FormConfig.cs`
Role:
- Ecran de configuration initiale.

Logique:
- Lit dimensions (lignes/colonnes) et noms joueurs.
- Verifie dimensions minimales (>= 2).
- Instancie `DatabaseConnection`.
- Instancie tous les repositories.
- Instancie `GameService` avec ces repositories.
- Lance une nouvelle partie via `CreateNewGame(...)`.
- Ouvre `FormGame` et masque la form de config.

### `Forms/FormGame.cs`
Role:
- Fenetre principale de jeu.

Composants:
- `BoardPanel`: plateau interactif.
- `CannonWidget` x2: controle deplacement canon J1/J2.
- `ScorePanel`: score, joueur courant, numero de mouvement.
- Boutons: Terminer, Restaurer, Reinitialiser.

Logique des actions:
- Clic sur intersection:
  - appelle `GameService.PlacePoint(...)`.
  - incremente le compteur local `_moveNumber` si succes.
  - rafraichit l'UI.
- Deplacement canon:
  - appelle `GameService.MoveCannon(...)`.
  - rafraichit l'UI.
- Clavier Ctrl+1..9:
  - appelle `GameService.ShootCannon(...)` avec puissance.
  - incremente `_moveNumber` si succes.
  - rafraichit l'UI.
- Clavier Ctrl+Z / bouton Restaurer:
  - appelle `GameService.UndoLastMove(...)`.
  - decremente `_moveNumber` si possible.
  - rafraichit l'UI.
- Bouton Reinitialiser:
  - confirmation utilisateur.
  - appelle `GameService.ResetGame(...)`.
  - remet `_moveNumber` a 0.
- Bouton Terminer:
  - appelle `GameService.EndGame(...)`.
  - affiche resultat final.

Gestion erreurs:
- Chaque action sensible est dans un `try/catch`.
- Les erreurs metier remontent en `MessageBox`.

### `Controls/BoardPanel.cs`
Role:
- Dessin de la grille et des objets de jeu.
- Conversion clic ecran -> coordonnees grille.

Logique d'affichage:
- Recoit `GameState` via `UpdateState`.
- Calcule `_cellSize` dynamique selon taille panel.
- Dessine dans l'ordre:
  1. Grille
  2. Cases bloquees (overlay gris transparent)
  3. Lignes validees
  4. Points actifs
  5. Canons

Logique interaction:
- `OnMouseClick` convertit la position via `ScreenToGrid`.
- Si coordonnees valides, declenche evenement `IntersectionClicked(row, col)`.

### `Controls/CannonWidget.cs`
Role:
- Controle vertical d'un canon (J1 ou J2).

Logique:
- Bouton haut envoie `Direction.Up`.
- Bouton bas envoie `Direction.Down`.
- `UpdatePosition(row)` met a jour le label du widget.

### `Controls/ScorePanel.cs`
Role:
- Affiche score, tour courant, numero du mouvement.

Logique:
- `Refresh(GameState, moveNumber)` met a jour les labels a partir de l'etat courant.

### `Form1.cs` (+ Designer/Resx)
- Form presente mais non utilisee dans le flux principal.

## 2.2 Projet `JeuDePoints.Domain` (modeles)

### `Models/GameState.cs`
Role:
- Objet central de l'etat complet d'une partie.

Contient:
- Meta partie: `GameId`, `Rows`, `Columns`, `Status`.
- Joueurs: noms, tour courant.
- Scores: `ScoreP1`, `ScoreP2`.
- Collections en memoire:
  - `Points`
  - `Cannons`
  - `ValidatedLines`
  - `BlockedCells`

Methodes:
- `IsFinished()` retourne vrai si status `finished`.
- `GetCurrentPlayerName()` selon `CurrentTurn`.

### `Models/Point.cs`
Role:
- Un point pose sur le plateau.

Logique:
- `IsActive=true` signifie visible/jouable.
- `IsActive=false` signifie detruit.
- `IsOwnedBy(playerId)` utilitaire de comparaison proprietaire.

### `Models/Cannon.cs`
Role:
- Position verticale du canon d'un joueur.

Logique:
- `MoveUp()` decrement avec borne min 0.
- `MoveDown(maxRow)` increment avec borne max.

### `Models/ValidatedLine.cs`
Role:
- Ligne de 5 points validee.

Logique:
- Garde direction + debut/fin.
- `GetCells()` reconstruit les 5 cellules selon direction:
  - horizontal
  - vertical
  - diagonal_asc
  - diagonal_desc

### `Models/BlockedCell.cs`
Role:
- Cellule rendue "protegee/bloquee" par une ligne validee.

Logique:
- Lie une cellule a `ValidatedLineId` et au `BlockingPlayerId`.

### `Models/Move.cs`
Role:
- Historique d'actions.

Logique:
- `MoveType` stocke type (`place`, `shoot`, etc.).
- Colonnes optionnelles selon le type de mouvement.
- Snapshot score stocke dans chaque move.

### `Models/ShotResult.cs`
Role:
- Resultat detaille d'un tir.

Logique:
- `IsValid` indique si l'action tir est autorisee.
- `Hit` indique impact sur point adverse actif.
- `WasBlockedByLine` indique arret anticipe sur trajectoire.
- `Trajectory` conserve les cellules parcourues.
- `InvalidReason` documente le refus.

### `Models/ValidationResult.cs`
Role:
- Objet standard de retour de validation.

Logique:
- `Ok()` et `Fail(message)` pour centraliser les validations.

### `Models/GameSnapshot.cs`
Role:
- Serialisation JSON de l'etat pour undo.

Logique:
- `FromGameState(state, moveNumber)`:
  - extrait les infos necessaires (tour, scores, canons, points, lignes, blocs).
  - serialise avec Newtonsoft.Json.
- `ToGameState()`:
  - deserialise `SnapshotData`.
  - reconstruit un `GameState` exploitable.

Remarque:
- L'undo se base sur snapshots persistes, pas uniquement sur recalcul de l'historique.

## 2.3 Projet `JeuDePoints.Services` (metier)

### `GameService.cs`
Role:
- Orchestrateur principal de toutes les actions de jeu.

Dependances:
- Repositories Data (game, cannon, point, line, blocked, move, snapshot)
- Services internes (`AlignmentChecker`, `CannonService`, `TurnManager`)

#### `CreateNewGame(rows, cols, p1, p2)`
Logique:
1. Cree la partie (`games`).
2. Initialise les canons en base.
3. Cree un `GameState` initial en memoire.
4. Place les canons J1/J2 en row 0 en memoire.
5. Cree le snapshot initial move 0.

#### `PlacePoint(state, row, col)`
Logique:
1. Recupere joueur courant.
2. Valide le placement via `TurnManager.ValidatePlacement`.
3. Ajoute/reactive le point en base (`PointRepository.AddPoint`).
4. Met a jour la collection `state.Points`.
5. Detecte nouveaux alignements via `AlignmentChecker.CheckAlignments`.
6. Pour chaque ligne nouvelle:
   - insert `validated_lines`
   - calcule cellules de la ligne
   - insert `blocked_cells`
   - met a jour memoire `state.ValidatedLines` + `state.BlockedCells`
   - incremente score du joueur courant
7. Persiste score dans `games`.
8. Cree un move `place`.
9. Passe le tour (`NextTurn`) et persiste `current_turn`.
10. Cree snapshot du move.

Effet metier:
- Une pose valide consomme le tour.
- Les alignements de 5 augmentent le score.

#### `ShootCannon(state, power)`
Logique:
1. Recupere joueur courant.
2. Verifie puissance 1..9.
3. Resout le tir via `CannonService.ResolveShot`.
4. Si tir invalide (`IsValid=false`), exception.
5. Si `Hit=true`, desactive le point touche en base + memoire.
6. Cree move `shoot` (inclut cible/hit/puissance).
7. Passe le tour et persiste.
8. Cree snapshot.

Effet metier:
- Le score ne change pas lors d'une destruction.
- Un tir valide consomme le tour, qu'il touche ou non.

#### `MoveCannon(state, playerId, direction)`
Logique:
- Deplace le canon en memoire via `Cannon.MoveUp/MoveDown`.
- Persiste la nouvelle row dans `cannons`.

#### `UndoLastMove(state)`
Logique:
1. Lit dernier move number.
2. Si 0: refuse (rien a annuler).
3. Supprime le dernier move et son snapshot associe.
4. Lit le snapshot le plus recent restant.
5. Reconstitue le `GameState` depuis snapshot.
6. Reinjecte meta non serialisees explicitement (GameId, Rows, Columns, noms joueurs).
7. Persiste tour/scores restitues dans `games`.

#### `ResetGame(state)`
Logique:
- Supprime la partie courante en base (cascade).
- Recree une nouvelle partie avec memes dimensions/noms.

#### `EndGame(state)`
Logique:
- Met `Status = "finished"` en memoire et base.

### `AlignmentChecker.cs`
Role:
- Detection des lignes de 5 consecutives apres placement.

Directions inspectees:
- horizontal `(0, +1)`
- vertical `(+1, 0)`
- diagonal_asc `(-1, +1)`
- diagonal_desc `(+1, +1)`

#### `CheckAlignments(state, lastRow, lastCol, playerId)`
Logique:
1. Prend les points actifs du joueur.
2. Pour chaque direction, appelle `CheckDirection`.
3. Si ligne detectee et nouvelle (`IsNewLine`), l'ajoute au resultat.
4. Retourne liste des lignes nouvellement validees.

#### `CheckDirection(...)`
Logique:
1. Construit un set des coordonnees du joueur.
2. Remonte en arriere pour trouver debut de sequence continue.
3. Compte la longueur continue dans la direction.
4. Si < 5: pas de ligne.
5. Si >= 5: construit une `ValidatedLine` sur 5 cellules a partir du debut.

#### `IsNewLine(line, existing)`
Logique:
- Evite de recompter une ligne deja representee.
- Regle implementee:
  - meme joueur
  - meme direction
  - au moins une cellule partagee
  => alors consideree comme deja existante.

### `CannonService.cs`
Role:
- Calcul de la cible et resolution complete d'un tir.

#### `CalculateTargetCol(power, columns, playerId)`
Logique:
1. Verifie puissance [1..9].
2. Calcule colonne 1-based avec formule:
   - `floor(power * columns / 9)`
3. Convertit en index 0-based (`-1`).
4. Clamp dans `[0, columns-1]`.
5. Si joueur 2, applique miroir horizontal.

#### `ResolveShot(state, playerId, power)`
Logique:
1. Lit la row du canon du joueur.
2. Calcule `targetCol`.
3. Construit la trajectoire (`GetTrajectory`) de bord vers cible.
4. Teste la trajectoire hors cible finale:
   - si une cellule est sur ligne validee:
     - tir reste valide
     - balle stoppee
     - `WasBlockedByLine=true`, `Hit=false`
5. Teste la cible finale:
   - si sur ligne validee:
     - tir invalide (`IsValid=false`)
6. Sinon calcule `Hit`:
   - vrai si point adverse actif sur cible.
7. Retourne `ShotResult` complet.

#### `GetTrajectory(...)`
Logique:
- J1 part de col 0 vers droite.
- J2 part de col `columns-1` vers gauche.
- Garde toutes les cellules de la ligne du canon jusqu'a cible incluse.

#### `IsOnValidatedLine(state, row, col)`
Logique:
- Verifie si une cellule appartient a au moins une ligne validee via `GetCells()`.

### `TurnManager.cs`
Role:
- Regles de validation de tour + alternance.

#### `ValidatePlacement(state, row, col, playerId)`
Refuse si:
- cellule deja occupee par un point actif
- cellule bloquee par une ligne adverse

#### `ValidateShot(state, playerId, power)`
- Verifie puissance [1..9]
- Verifie resultat de `ResolveShot` (cible protegee = invalide)

#### `NextTurn(currentTurn)`
- Bascule 1 -> 2 ou 2 -> 1.

## 2.4 Projet `JeuDePoints.Data` (acces donnees)

### `DatabaseConnection.cs`
Role:
- Fournit des connexions PostgreSQL.

Logique:
- Charge `appsettings.json` depuis le dossier d'execution.
- Lit `ConnectionStrings:Default`.
- `GetConnection()` retourne `NpgsqlConnection`.
- `TestConnection()` tente un `Open()` et retourne bool.

### `Repositories/GameRepository.cs`
- `CreateGame(...)`: insert dans `games`, retourne id.
- `GetGameById(gameId)`: lit une partie et map vers `GameState`.
- `UpdateTurn(...)`: met a jour `current_turn`.
- `UpdateScores(...)`: met a jour `score_p1` et `score_p2`.
- `SetStatus(...)`: met a jour `status`.
- `DeleteGame(...)`: supprime partie (cascade via FK).

### `Repositories/CannonRepository.cs`
- `InitCannons(gameId)`: cree les 2 canons row 0.
- `GetCannon(gameId, playerId)`: lecture canon.
- `UpdatePosition(...)`: persiste row canon.

### `Repositories/PointRepository.cs`
- `AddPoint(...)`:
  - essaie d'abord de reactiver une cellule inactive meme coordonnee.
  - sinon insert un nouveau point.
- `GetActivePoints(gameId)`: points actifs de la partie.
- `GetActivePointsByPlayer(...)`: filtre par joueur.
- `DeactivatePoint(...)`: met `is_active=false` sur la cellule.
- `IsCellOccupied(...)`: verifie occupation active.

### `Repositories/ValidatedLineRepository.cs`
- `AddValidatedLine(line)`: insert ligne validee.
- `GetValidatedLines(gameId)`: lit lignes de la partie.
- `LineAlreadyExists(...)`: verification d'existence (non utilisee dans `GameService` actuel).

### `Repositories/BlockedCellRepository.cs`
- `AddBlockedCells(...)`: insert en boucle les cellules bloquees d'une ligne.
- `IsCellBlocked(...)`: verifie blocage contre un joueur.
- `GetBlockedCells(gameId)`: lecture des cellules bloquees.

### `Repositories/MoveRepository.cs`
- `AddMove(move)`: insert action et retourne id.
- `GetLastMoveNumber(gameId)`: max move_number.
- `DeleteMove(gameId, moveNumber)`: suppression d'un move.
- `GetMoveHistory(gameId)`: historique ordonne.

### `Repositories/SnapshotRepository.cs`
- `SaveSnapshot(gameId, moveNumber, snapshot)`: insert JSONB.
- `GetSnapshot(gameId, moveNumber)`: lit snapshot precis.
- `GetLatestSnapshot(gameId)`: lit dernier snapshot.
- `DeleteSnapshot(gameId, moveNumber)`: supprime snapshot cible.

## 3. Base SQL et contraintes de coherence

Fichier: `schema.sql`

Tables:
- `games`
- `cannons`
- `points`
- `validated_lines`
- `blocked_cells`
- `moves`
- `game_snapshots`

Contrainte importante:
- `points` a `UNIQUE (game_id, row, col)`.
- Cette contrainte est coherente avec la logique de reactivation dans `PointRepository.AddPoint`.

Index:
- Index sur points actifs, moves, snapshots, etc. pour accelerer lecture metier.

Cascade:
- FK en `ON DELETE CASCADE` pour nettoyer toutes donnees d'une partie lors d'un reset/suppression.

## 4. Flux d'execution complet

## 4.1 Demarrage
1. `Program.Main` ouvre `FormConfig`.
2. Utilisateur saisit params.
3. `FormConfig` cree DB + repositories + `GameService`.
4. `GameService.CreateNewGame` cree partie + canons + snapshot initial.
5. `FormGame` affiche le plateau.

## 4.2 Tour de placement
1. Clic utilisateur sur `BoardPanel`.
2. `FormGame.OnIntersectionClicked` -> `GameService.PlacePoint`.
3. Validation de placement.
4. Ecriture point.
5. Detection alignements.
6. Si alignements: creation lignes + blocked cells + score +1 par ligne.
7. Ecriture move.
8. Changement de tour.
9. Snapshot.
10. UI rafraichie.

## 4.3 Tour de tir
1. Ctrl+1..9 dans `FormGame`.
2. `GameService.ShootCannon(power)`.
3. Resolution tir (`CannonService`).
4. Si hit: desactivation point adverse.
5. Ecriture move tir.
6. Changement de tour.
7. Snapshot.
8. UI rafraichie.

## 4.4 Undo
1. `FormGame` appelle `UndoLastMove`.
2. Suppression dernier move + snapshot associe.
3. Chargement snapshot precedent.
4. Reconstruction de l'etat.
5. Resynchronisation `games` (tour + scores).
6. UI rafraichie.

## 4.5 Fin / reset
- Fin: status passe a `finished`, affichage gagnant.
- Reset: suppression complete partie puis recreation propre.

## 5. Logiques implicites importantes (a connaitre)

1. Les cellules bloquees sont adverses:
- validation placement bloque seulement si `BlockingPlayerId == adversaire`.

2. Les lignes validees servent aussi de "barriere de tir":
- une trajectoire peut etre stoppee avant cible.
- une cible exactement sur ligne validee rend le tir invalide.

3. Le move counter UI (`_moveNumber`) est local a `FormGame`:
- il n'est pas recalcule depuis la base a chaque refresh.
- il suit les actions reussies dans la session de la form.

4. `TurnManager.ValidateShot` existe mais `GameService.ShootCannon` fait sa propre validation directe:
- logique valide mais redondance de design potentielle.

5. La detection d'une ligne >= 5 construit une ligne de longueur 5 a partir du debut de sequence:
- les extensions partageant des cellules ne sont pas recomptees.

## 6. Fichiers utilitaires / placeholders

- `Class1.cs` dans certains projets: placeholder sans logique metier.
- `JeuDePoints.Tests`: projet WinForms present, pas de batteries de tests metier actives.

## 7. Resume simple

- Le coeur du jeu est dans `GameService` + `AlignmentChecker` + `CannonService`.
- `GameState` est la source d'etat runtime.
- PostgreSQL garde l'historique complet (moves + snapshots).
- L'UI est fine: elle orchestre l'interaction et affiche l'etat retourne par les services.
- Chaque action utile (place, shoot, undo, reset, end) est deja reliee de bout en bout UI -> metier -> DB.
