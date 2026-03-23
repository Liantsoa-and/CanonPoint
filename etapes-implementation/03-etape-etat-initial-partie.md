# Etape 3 - Initialiser une partie proprement

But: obtenir un etat de partie de base fiable.

Sous-taches:

- Creer la grille vide (rows x cols).
- Initialiser joueur courant (regle simple: joueur 1 commence).
- Initialiser score des 2 joueurs a 0.
- Initialiser canons gauche/droite sur une ligne de depart connue.
- Exposer une methode "CreateNewGameState".

Livrable:

- Un etat initial cree de facon deterministe.

Definition terminee:

- Deux initialisations consecutives donnent le meme etat attendu.

Temps estime: 30-45 min.

Reponses aux sous-taches (etat actuel du code):

- Creer la grille vide (rows x cols): OK.
	- Fait dans `GameState` via initialisation de toutes les cellules avec `Owner = null` et `IsInvulnerable = false`.

- Initialiser joueur courant (joueur 1 commence): OK.
	- Fait dans `GameStateFactory` avec `CurrentPlayer = PlayerSide.Player1`.

- Initialiser score des 2 joueurs a 0: OK.
	- Fait dans `GameStateFactory` avec `ScorePlayer1 = 0` et `ScorePlayer2 = 0`.

- Initialiser canons gauche/droite sur une ligne de depart connue: OK.
	- Fait dans `GameStateFactory` avec `SetCanonRows(0, 0)`.

- Exposer une methode "CreateNewGameState": OK.
	- Fait dans `IGameStateFactory` et implemente dans `GameStateFactory`.

Preuves techniques:

- Build solution: OK.
- Test de determinisme: OK (`GameStateFactoryTests`).

Resultat:

- L'etape 3 est validee.
- L'etat initial est cree de facon deterministe pour des parametres identiques.
