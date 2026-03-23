# Etape 2 - Poser l'ossature Domain

But: creer un socle propre ou la logique metier sera testable sans UI.

Sous-taches:

- Lister les objets minimaux: GameState, CellState, MoveData, LineData.
- Definir les enums utiles: PlayerSide, MoveType, GameStatus.
- Definir les invariants simples (taille grille > 0, bornes ligne/colonne).
- Ecrire les signatures des services metier (sans logique complexe).

Livrable:

- Des classes/metiers minimales compilees, sans comportement complet.

Definition terminee:

- Le code compile et la structure est claire pour les etapes suivantes.

Temps estime: 30-60 min.

Reponses attendues (version figee pour valider l'etape 2):

1) Objets Domain minimaux a creer

- GameState
	- Role: representer l'etat complet d'une partie en memoire.
	- Champs minimaux:
		- GameId
		- Rows
		- Cols
		- Status (GameStatus)
		- CurrentPlayer (PlayerSide)
		- ScorePlayer1
		- ScorePlayer2
		- LeftCanonRow
		- RightCanonRow
		- Cells (structure 2D ou dictionnaire indexe row/col)
		- CountedLineHashes (ensemble de string pour anti double-comptage)

- CellState
	- Role: representer une case du plateau.
	- Champs minimaux:
		- Row
		- Col
		- Owner (PlayerSide? nullable)
		- IsInvulnerable

- MoveData
	- Role: representer une action d'un joueur (placement ou tir).
	- Champs minimaux:
		- SequenceNumber
		- Player
		- Type (MoveType)
		- Row
		- Col
		- Power (nullable, utile pour tir)
		- CreatedAtUtc

- LineData
	- Role: representer une ligne candidate/validee de 5 points.
	- Champs minimaux:
		- Owner
		- Cells (5 coordonnees)
		- Hash

2) Enums Domain a definir

- PlayerSide
	- None = 0
	- Player1 = 1
	- Player2 = 2

- MoveType
	- PlacePoint = 1
	- FireShot = 2

- GameStatus
	- InProgress = 1
	- Finished = 2
	- Replay = 3 (optionnel mais utile des maintenant)

3) Invariants simples a poser des maintenant

- Rows > 0 et Cols > 0.
- Toute coordonnee manipulee doit verifier: 0 <= row < Rows et 0 <= col < Cols.
- LeftCanonRow et RightCanonRow doivent rester dans [0, Rows - 1].
- Une CellState ne peut pas etre invulnerable si Owner est null.
- SequenceNumber doit etre strictement > 0 pour un move persiste.

4) Signatures de services metier (sans logique complete)

- IGameStateFactory
	- CreateNew(gameId, rows, cols) -> GameState

- IPlacementService
	- TryPlacePoint(gameState, player, row, col) -> PlacePointResult

- ILineDetectionService
	- FindCandidateLines(gameState, lastRow, lastCol, player) -> IReadOnlyList<LineData>

- ILineValidationService
	- GetValidatedLines(gameState, candidateLines, player) -> IReadOnlyList<LineData>

- IShotService
	- TryFire(gameState, player, cannonRow, power) -> FireShotResult

- ITurnService
	- PlayTurn(gameState, command) -> TurnResult

Note: on attend uniquement les contrats + objets de retour minimaux (Result), pas la logique metier detaillee.

5) Ce qu'on attendait exactement de toi a cette etape

- Donner un squelette clair et compile, pas un moteur complet.
- Nommer les bons objets et leurs responsabilites.
- Poser des invariants simples pour eviter les bugs evidents plus tard.
- Definir les interfaces de services pour separer proprement Domain / Application / UI.
- Sortir de l'etape avec une base stable qui facilite l'etape 3 (etat initial) et l'etape 4 (placement).

Critere de validation pratique:

- Si tu peux expliquer en 2 minutes "qui fait quoi" entre GameState, CellState et les services, l'etape est reussie.
- Si le projet compile avec ces classes/interfaces vides ou minimales, l'etape est reussie.
