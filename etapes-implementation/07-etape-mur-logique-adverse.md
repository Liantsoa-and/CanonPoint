# Etape 7 - Regle du mur logique adverse

But: empecher la validation d'une ligne qui traverse un invulnerable adverse.

Sous-taches:

- Lors de la validation d'une ligne candidate, lire les 5 cellules.
- Si une cellule contient un invulnerable adverse: ligne rejetee.
- Conserver le placement initial (on rejette la ligne, pas le coup de pose).
- Ajouter 2 cas de verification: avec mur adverse / sans mur adverse.

Livrable:

- Filtre metier "mur logique" applique a la validation des lignes.

Definition terminee:

- Cas mur adverse: 0 point gagne.

Temps estime: 30-60 min.

Approche pas a pas suivie:

1. Ajouter un filtre explicite "mur logique adverse" dans la validation des lignes.
2. Conserver le principe de l'etape 6: on rejette la ligne, pas le placement.
3. Ajouter 2 tests cibles: avec mur adverse / sans mur adverse.
4. Verifier que le score ne bouge pas en cas de mur.

Reponses aux sous-taches (etat actuel du code):

- Lire les 5 cellules de la ligne candidate: OK.
- Si une cellule contient un invulnerable adverse: ligne rejetee: OK.
- Conserver le placement initial: OK (aucun rollback de pose dans cette etape).
- Ajouter 2 cas de verification: OK (tests dedies).

Fichiers concernes:

- `Domain/Services/LineValidationService.cs`
- `tests/CanonPoint.App.Tests/Domain/LineValidationServiceTests.cs`

Validation:

- Cas mur adverse invulnerable: 0 point gagne, ligne rejetee.
- Cas sans mur adverse: ligne validee et score incremente.

Resultat:

- L'etape 7 est implementee et redigee.
