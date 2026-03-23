# Etape 8 - Action tir du canon

But: implementer le tir avec ses effets minimaux.

Sous-taches:

- Valider puissance dans [1..9].
- Determiner la case cible en fonction du canon + puissance.
- Appliquer les regles d'effet:
- case vide: rien
- point du joueur courant: rien
- point adverse invulnerable: rien
- point adverse non invulnerable: suppression
- S'assurer que le score ne change jamais sur un tir.

Livrable:

- Une action tir complete et predictable.

Definition terminee:

- Les 4 cas de cible produisent exactement l'effet attendu.

Temps estime: 45-90 min.

Approche pas a pas suivie:

1. Poser les regles minimales de tir (puissance, cible, effet).
2. Determiner la cible a partir du joueur + ligne de canon + puissance.
3. Appliquer les 4 cas d'effet metier sans toucher au score.
4. Verifier via tests les cas cibles et la contrainte score stable.

Convention de cible retenue:

- Joueur 1 tire de gauche vers droite: `targetCol = power - 1`.
- Joueur 2 tire de droite vers gauche: `targetCol = intersectionCols - power`.
- `targetRow = cannonRow`.

Reponses aux sous-taches (etat actuel du code):

- Valider puissance dans [1..9]: OK.
- Determiner la case cible en fonction du canon + puissance: OK.
- Appliquer les regles d'effet (vide, allie, adverse invulnerable, adverse non invulnerable): OK.
- S'assurer que le score ne change jamais sur un tir: OK.

Fichiers implementes:

- `Domain/Services/ShotService.cs`
- `Domain/Models/CellState.cs` (ajout `ClearOwner`)
- `Domain/Results/FireShotResult.cs` (ajout indicateur `WasPointDestroyed`)
- `Program.cs` (DI `IShotService`)
- Tests: `tests/CanonPoint.App.Tests/Domain/ShotServiceTests.cs`

Validation:

- Les 4 cas de cible produisent l'effet attendu.
- Cas puissance invalide couvre le rejet.
- Le score reste inchangé apres chaque tir.

Resultat:

- L'etape 8 est implementee et redigee.
