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
