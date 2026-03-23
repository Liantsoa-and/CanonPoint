# Etape 5 - Detecter les alignements de 5

But: detecter les lignes candidates creees apres un placement.

Sous-taches:

- Scanner uniquement autour du dernier point pose.
- Gerer 4 axes: horizontal, vertical, diagonale montante, diagonale descendante.
- Trouver les sequences de 5 consecutives du joueur courant.
- Retourner une liste de lignes candidates (coordonnees).
- Ajouter un garde-fou contre les doublons de memes lignes.

Livrable:

- Une fonction qui renvoie des lignes candidates stables.

Definition terminee:

- Une position qui cree 2 lignes renvoie bien 2 lignes distinctes.

Temps estime: 45-90 min.
