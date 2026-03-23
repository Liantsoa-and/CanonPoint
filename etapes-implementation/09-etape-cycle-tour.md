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
