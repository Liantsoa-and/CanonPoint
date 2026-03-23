# Etape 14 - Tests unitaires des regles critiques

But: securiser les regles metier principales.

Sous-taches:

- Tester placement (borne, case occupee, case vide).
- Tester detection ligne simple et multi-ligne.
- Tester mur logique adverse.
- Tester tir sur les 4 types de cible.
- Tester non-regression du score.

Livrable:

- Une suite de tests unitaires courte mais utile.

Definition terminee:

- Les regles critiques ont au moins 1 test de succes + 1 test d'echec.

Temps estime: 60-120 min.

Precision metier figee:

- Le placement des points est autorise sur les rebords du plateau, coins inclus.
- Pour une grille de `rows x cols` cases, les croisements jouables vont de `0..rows` et `0..cols`.

Reponses aux sous-taches (etat actuel des tests):

- Tester placement (borne, case occupee, case vide): OK.
	- Inclut des tests explicites sur rebords/coins.
- Tester detection ligne simple et multi-ligne: OK.
- Tester mur logique adverse: OK.
- Tester tir sur les 4 types de cible: OK.
- Tester non-regression du score: OK.

Resultat:

- La suite couvre les regles critiques avec succes + echec, y compris la regle de placement sur rebords.
