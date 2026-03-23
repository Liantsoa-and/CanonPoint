# Etape 1 - Verrouiller les decisions metier

But: fixer les zones ambiguies avant de coder.

Sous-taches:

- Choisir la definition officielle de "plateau plein".
- Valider que le score donne +1 par ligne distincte creee sur un meme coup.
- Choisir la strategie anti double-comptage de lignes (hash de ligne conseille).
- Verifier les noms finaux des champs SQL (games, players, shots).
- Ecrire ces decisions dans un mini document de reference.

Livrable:

- Un document court de decisions figees.

Definition terminee:

- Plus aucune ambiguite bloquante pour coder le Domain.

Temps estime: 20-40 min.

Reponses aux sous-taches (decisions figees) :

1) Definition officielle de "plateau plein"
- Une partie est terminee si, a la fin du tour courant, aucune case vide n'est disponible pour un placement.
- La presence de tirs possibles au tour suivant ne change pas cette regle.
- Cette regle est volontairement simple, deterministe et testable.

2) Regle de score
- Un joueur gagne +1 point par ligne distincte de 5 validee lors de son placement.
- Si un seul placement cree plusieurs lignes distinctes, le score augmente de +N (N = nombre de lignes distinctes).
- Le score ne diminue jamais.

3) Strategie anti double-comptage
- Nous utilisons un hash canonique de ligne.
- Construction du hash : prendre les 5 coordonnees de la ligne, les trier, puis serialiser sous forme stable (ex: "r1:c1|r2:c2|...|r5:c5").
- Stockage : conserver un ensemble des hashes deja comptes pour la partie.
- Si le hash existe deja, la ligne n'est pas recomptee.

4) Noms finaux SQL (valide)
- games : grid_rows, grid_cols
- players : name
- shots : target_row, target_col
- Les noms actuels sont conserves comme reference unique.

Conclusion etape 1
- Les decisions metier bloquantes sont figees.
- L'etape 1 est validee et on peut passer a l'etape 2 (ossature Domain).