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

Approche pas a pas suivie:

1. Bloquer le perimetre: detection de lignes candidates uniquement (pas de score, pas de validation mur logique).
2. Scanner uniquement autour du dernier point pose.
3. Parcourir 4 axes (horizontal, vertical, diagonales).
4. Construire des fenetres de 5 cellules contenant le dernier point.
5. Filtrer: garder seulement les fenetres 100% possedees par le joueur courant.
6. Dedupliquer les lignes avec un hash canonique de coordonnees triees.

Precision metier:

- Les points et alignements sont detectes sur les croisements de la grille.

Reponses aux sous-taches (etat actuel du code):

- Scanner uniquement autour du dernier point pose: OK.
- Gerer 4 axes (horizontal/vertical/diagonales): OK.
- Trouver les sequences de 5 consecutives du joueur courant: OK.
- Retourner une liste de lignes candidates avec coordonnees: OK.
- Ajouter un garde-fou anti-doublons: OK (hash canonique + dictionnaire par hash).

Fichiers implementes:

- `Domain/Services/LineDetectionService.cs`
- Tests: `tests/CanonPoint.App.Tests/Domain/LineDetectionServiceTests.cs`

Validation:

- Cas ligne horizontale: detectee.
- Cas 2 lignes sur un seul placement (croix): 2 lignes distinctes detectees.
- Cas dernier point non possede par le joueur: aucune ligne retournee.

Resultat:

- L'etape 5 est implementee et redigee.
