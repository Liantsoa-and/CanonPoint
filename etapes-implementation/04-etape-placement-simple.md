# Etape 4 - Placement simple d'un point

But: autoriser le placement legal sans scoring avance.

Sous-taches:

- Verifier que la case cible est dans la grille.
- Refuser si la case est deja occupee.
- Poser un point du joueur courant si la case est vide.
- Retourner un resultat explicite (succes/erreur + raison).
- Ecrire 3 tests manuels: hors borne, case occupee, case vide.

Livrable:

- Une action de placement fiable et comprenable.

Definition terminee:

- Les 3 cas manuels se comportent exactement comme attendu.

Temps estime: 30-60 min.

Precision metier importante:

- Le placement des points se fait sur les croisements de la grille, pas au centre des cases.
- Avec une grille de R x C cases, on a (R + 1) x (C + 1) croisements jouables.

Reponses aux sous-taches (etat actuel du code):

- Verifier que la cible est dans la grille: OK.
	- Le service valide les bornes des croisements avant placement.

- Refuser si la cible est deja occupee: OK.
	- Le service refuse si un proprietaire existe deja sur le croisement.

- Poser un point du joueur courant si vide: OK.
	- Le service pose le point uniquement si le croisement est libre.

- Retourner un resultat explicite: OK.
	- Retourne `PlacePointResult` avec `Success`, `ErrorMessage`, `Position`.

- Ecrire 3 tests manuels: OK (couverts aussi en tests automatises).
	- Hors borne -> erreur.
	- Croisement occupe -> erreur.
	- Croisement vide -> succes.

Resultat:

- L'etape 4 est implementee et redigee.
- Le placement simple est fiable, lisible, et conforme a la regle de placement sur croisements.
