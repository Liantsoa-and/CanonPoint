# Etape 10 - Persister les moves avec sequence_number

But: garantir un historique fiable pour reprise et replay.

Sous-taches:

- Definir la structure du move persiste (type + payload minimal).
- Assurer sequence_number strictement croissant par partie.
- Persister chaque action validee dans une transaction.
- Ajouter une contrainte DB d'unicite sur (game_id, sequence_number).
- Lire les moves tries par sequence_number.

Livrable:

- Historique persiste, coherent et ordonne.

Definition terminee:

- Aucune collision de sequence sur une meme partie.

Temps estime: 60-120 min.

Approche pas a pas suivie:

1. Verifier la structure persistee du move (type + payload minimal).
2. Garantir une sequence strictement croissante via calcul du prochain numero.
3. Encadrer chaque action (placement/tir) dans une transaction explicite.
4. S'appuyer sur la contrainte d'unicite DB pour prevenir les collisions de sequence.
5. Verifier la lecture ordonnee des moves par `sequence_number`.

Reponses aux sous-taches (etat actuel du code):

- Definir la structure du move persiste: OK (`Move` avec `IsShot`, `PointId`/`ShotId`, `SequenceNumber`).
- Assurer `sequence_number` strictement croissant par partie: OK (`GetNextSequenceNumberAsync`).
- Persister chaque action validee dans une transaction: OK (transactions explicites dans `AddPointAsync` et `FireShotAsync`).
- Ajouter une contrainte DB d'unicite sur `(game_id, sequence_number)`: OK (index unique EF sur `Move`).
- Lire les moves tries par `sequence_number`: OK (retour `GameStateResponseDto.Moves` ordonne par sequence).

Fichiers concernes:

- `src/CanonPoint.App/Services/GameService.cs`
- `src/CanonPoint.App/Data/AppDbContext.cs`

Validation:

- Les moves de placement et tir sont sauvegardes avec sequence.
- Les actions sont encadrees dans une transaction par action.
- L'historique rendu est ordonne par `sequence_number`.

Resultat:

- L'etape 10 est implementee et redigee.
