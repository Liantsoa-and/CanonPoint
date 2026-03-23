# Etape 6 - Scoring et invulnerabilite de ligne validee

But: convertir les lignes candidates en gain de score + protection.

Sous-taches:

- Pour chaque ligne validee: +1 score au joueur.
- Marquer les 5 points de la ligne en invulnerable.
- Ne jamais diminuer le score.
- Stocker une trace des lignes deja comptees (hash conseille).
- Verifier qu'une meme ligne n'est pas recomptee au coup suivant.

Livrable:

- Mecanisme de score stable et non repetitif.

Definition terminee:

- Rejouer le meme pattern ne donne pas de points en boucle.

Temps estime: 45-90 min.

Approche pas a pas suivie:

1. Conserver le scope de l'etape: score + invulnerabilite + anti-recomptage seulement.
2. Partir des lignes candidates detectees a l'etape 5.
3. Filtrer les lignes deja comptees via hash.
4. Pour chaque nouvelle ligne validee:
	- incrementer le score du joueur de +1
	- marquer les 5 points en invulnerable
	- enregistrer le hash dans `CountedLineHashes`
5. Verifier qu'une meme ligne passee une seconde fois ne modifie plus le score.

Reponses aux sous-taches (etat actuel du code):

- Pour chaque ligne validee: +1 score au joueur: OK.
- Marquer les 5 points de la ligne en invulnerable: OK.
- Ne jamais diminuer le score: OK (seulement des increments).
- Stocker une trace des lignes deja comptees: OK (`CountedLineHashes`).
- Verifier qu'une meme ligne n'est pas recomptee: OK (tests dedies).

Fichiers implementes:

- `Domain/Services/LineValidationService.cs`
- DI: enregistrement de `ILineValidationService` dans `Program.cs`
- Tests: `tests/CanonPoint.App.Tests/Domain/LineValidationServiceTests.cs`

Validation:

- Ligne unique: score +1 et 5 points invulnerables.
- Meme ligne rejouee: 0 point supplementaire.
- Deux lignes distinctes sur un meme coup: score +2.

Resultat:

- L'etape 6 est implementee et redigee.
