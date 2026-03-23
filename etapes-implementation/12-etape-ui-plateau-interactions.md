# Etape 12 - Interface plateau (jeu actif)

But: brancher l'UI sur la logique sans y mettre les regles metier.

Sous-taches:

- Afficher la grille depuis GameState.
- Afficher canons gauche/droite alignes aux lignes.
- Ajouter interactions minimales: poser un point, deplacer canon, tirer.
- Afficher erreurs metier lisibles (action invalide).
- Rafraichir score, joueur courant, statut partie.

Livrable:

- Ecran plateau fonctionnel pour jouer un tour complet.

Definition terminee:

- L'UI ne contient pas de regles metier complexes.

Temps estime: 60-120 min.

Approche pas a pas suivie:

1. Garder les regles metier dans l'API/Domain, pas dans l'UI.
2. Brancher le plateau sur l'API existante (`/api/games/{id}`, `/points`, `/shots`).
3. Ajouter interactions minimales: clic de pose, deplacement canons, bouton tirer + puissance.
4. Recharger l'etat serveur apres chaque action pour garder l'affichage coherent.
5. Afficher les messages d'erreur metier lisibles dans l'UI.

Reponses aux sous-taches (etat actuel du code):

- Afficher la grille depuis GameState: OK (lecture de `gridRows/gridCols` et rendu dynamique).
- Afficher canons gauche/droite alignes aux lignes: OK.
- Interactions minimales (poser/deplacer/tirer): OK.
- Afficher erreurs metier lisibles: OK (zone message UI).
- Rafraichir score, joueur courant, statut partie: OK (rechargement apres action).

Fichiers modifies:

- `src/CanonPoint.App/wwwroot/plateau.html`

Validation fonctionnelle:

- Clic sur plateau -> `POST /api/games/{id}/points`.
- Bouton Tirer -> `POST /api/games/{id}/shots`.
- Rechargement via `GET /api/games/{id}` apres chaque action.
- Message utilisateur visible en succes/erreur.

Resultat:

- L'etape 12 est implementee et redigee.
