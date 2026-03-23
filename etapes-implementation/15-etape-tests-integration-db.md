# Etape 15 - Tests d'integration PostgreSQL

But: verifier le flux complet avec persistance.

Sous-taches:

- Scenario creation partie -> placements -> tir -> lecture historique.
- Verifier sequence_number croissant en DB.
- Verifier reconstruction depuis moves.
- Tester au moins un cas d'erreur transactionnelle.

Livrable:

- Scenarios d'integration qui couvrent le parcours principal.

Definition terminee:

- Les tests prouvent la coherence metier + DB ensemble.

Temps estime: 60-120 min.

Approche pas a pas suivie:

1. Mettre en place des tests d'integration service + DB relationnelle.
2. Jouer un scenario complet (creation partie, placements, tir).
3. Verifier l'ordre des moves par `sequence_number`.
4. Rejouer les moves pour verifier la reconstruction.
5. Ajouter un test d'erreur transactionnelle avec rollback.

Reponses aux sous-taches (etat actuel):

- Scenario complet creation -> placements -> tir -> lecture historique: OK.
- Verification sequence_number croissant en DB: OK.
- Verification reconstruction depuis moves: OK.
- Cas d'erreur transactionnelle: OK (violation unicite sequence + rollback verifie).

Fichiers implementes:

- `tests/CanonPoint.App.Tests/Integration/GameServiceIntegrationTests.cs`
- `tests/CanonPoint.App.Tests/CanonPoint.App.Tests.csproj` (provider SQLite pour tests d'integration locaux)

Resultat:

- L'etape 15 est implementee et redigee.
- Les tests prouvent la coherence metier + persistance sur un SGBD relationnel en execution locale.
