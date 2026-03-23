# Etape 9 - Cycle de tour et changement de joueur

But: fiabiliser l'ordre de traitement d'un coup.

Sous-taches:

- Creer une orchestration unique PlayTurn.
- Enchainer: valider action -> appliquer -> scorer si placement -> fin de partie -> joueur suivant.
- Garantir qu'une action invalide ne change pas l'etat.
- Garder une sortie unique (resultat de tour) pour UI/API.

Livrable:

- Un pipeline de tour unique, sans logique dupliquee.

Definition terminee:

- Placement et tir passent par le meme point d'entree.

Temps estime: 45-75 min.

Approche pas a pas suivie:

1. Implementer un point d'entree unique PlayTurn pour placement et tir.
2. Router selon MoveType vers placement ou tir.
3. Sur placement: detecter/valider les lignes pour appliquer scoring et invulnerabilite.
4. Sur succes: evaluer fin de partie puis changer de joueur.
5. Creer un MoveData unique de sortie pour UI/API.
6. Garantir qu'une action invalide ne modifie pas l'etat.

Reponses aux sous-taches (etat actuel du code):

- Creer une orchestration unique PlayTurn: OK.
- Enchainer valider -> appliquer -> scorer si placement -> fin de partie -> joueur suivant: OK.
- Garantir qu'une action invalide ne change pas l'etat: OK (tests dedies).
- Garder une sortie unique pour UI/API: OK (`TurnResult` + `MoveData`).

Fichiers implementes:

- `Domain/Services/TurnService.cs`
- `Domain/Models/GameState.cs` (support sequence et helpers de cycle)
- `Program.cs` (DI `ITurnService`)
- Tests: `tests/CanonPoint.App.Tests/Domain/TurnServiceTests.cs`

Validation:

- Placement et tir passent par le meme point d'entree.
- Le joueur change apres un tour valide.
- Une action invalide laisse l'etat intact.

Resultat:

- L'etape 9 est implementee et redigee.
