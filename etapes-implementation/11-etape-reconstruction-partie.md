# Etape 11 - Reconstruction d'etat depuis l'historique

But: rebatir la partie uniquement a partir des moves.

Sous-taches:

- Charger la liste des moves d'une partie.
- Rejouer les moves dans l'ordre pour reconstruire l'etat courant.
- Verifier que l'etat reconstruit == etat sauvegarde courant.
- Exposer une methode "LoadGameStateFromHistory".

Livrable:

- Une reconstruction deterministe et re-utilisable.

Definition terminee:

- Le meme historique produit toujours le meme etat final.

Temps estime: 45-90 min.

Approche pas a pas suivie:

1. Creer un service dedie de reconstruction.
2. Initialiser un etat vierge avec `GameStateFactory`.
3. Trier l'historique par `sequence_number`.
4. Rejouer chaque move en reutilisant `PlayTurn` (meme pipeline que le jeu normal).
5. Verifier la coherence de sequence pendant replay.

Reponses aux sous-taches (etat actuel du code):

- Charger la liste des moves d'une partie: OK (entree `moves` fournie au service).
- Rejouer les moves dans l'ordre: OK (`OrderBy(SequenceNumber)` + replay `PlayTurn`).
- Verifier etat reconstruit == etat courant: OK (tests d'equivalence).
- Exposer `LoadGameStateFromHistory`: OK.

Fichiers implementes:

- `src/CanonPoint.App/Domain/Services/IGameStateReconstructionService.cs`
- `src/CanonPoint.App/Domain/Services/GameStateReconstructionService.cs`
- `src/CanonPoint.App/Program.cs` (enregistrement DI)
- Tests: `tests/CanonPoint.App.Tests/Domain/GameStateReconstructionServiceTests.cs`

Validation:

- Le replay reconstruit le meme etat final que la partie jouee.
- Le meme historique rejoue deux fois produit exactement le meme etat final.

Resultat:

- L'etape 11 est implementee et redigee.
