# Etape 13 - Mode replay simple

But: naviguer coup par coup sans modifier la partie.

Sous-taches:

- Charger la liste des moves ordonnee.
- Ajouter currentMoveIndex.
- Bouton precedent: annuler/revenir d'un coup.
- Bouton suivant: appliquer un coup.
- Bloquer les actions de jeu pendant replay.

Livrable:

- Replay lisible et non destructif.

Definition terminee:

- Navigation avant/arriere stable sur un historique complet.

Temps estime: 45-90 min.

Approche pas a pas suivie:

1. Ajouter un mode replay explicite dans l'UI (toggle).
2. Charger et trier les moves par `sequence_number`.
3. Ajouter `currentMoveIndex` et des controles precedent/suivant.
4. Rejouer localement les moves jusqu'a l'index courant pour reconstruire l'affichage.
5. Bloquer les actions de jeu (pose/tir/deplacement canon) tant que replay est actif.

Reponses aux sous-taches (etat actuel du code):

- Charger la liste ordonnee des moves: OK.
- Ajouter `currentMoveIndex`: OK.
- Bouton precedent: OK.
- Bouton suivant: OK.
- Bloquer les actions de jeu en replay: OK.

Fichier modifie:

- `src/CanonPoint.App/wwwroot/plateau.html`

Validation fonctionnelle:

- Le mode replay permet de naviguer coup par coup.
- Les actions de jeu sont refusees tant que le replay est actif.
- La sortie du replay revient au mode jeu normal.

Resultat:

- L'etape 13 est implementee et redigee.
