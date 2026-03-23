# CanonPoint - Specification Projet Detaillee

## 1. Objectif de ce document

Ce document ajoute une lecture de l'etat reel du code pour separer clairement :

- ce qui est deja implemente
- ce qui reste a construire
- les ambiguities a arbitrer

Le but est d'avoir une specification unique, exploitable par un developpeur sans interpretation implicite.

---

## 2. Perimetre fonctionnel (vision cible)

### 2.1 Concept de jeu

- Jeu a 2 joueurs en tour par tour.
- Plateau carre de taille variable. [ok]
- Chaque tour, un joueur choisit une action :
	- placer un point
	- tirer avec son canon
- L'objectif est de valider des lignes de 5 points alignes.
- Chaque ligne validee donne +1 point au joueur.
- Le score ne diminue jamais.
- Les point se place sur les croisements du grid.

### 2.2 Conditions de fin

La partie se termine quand il est impossible de placer un nouveau point (plateau plein de points non supprimables dans les faits).

Remarque importante : comme le canon peut detruire des points non invulnerables, la condition de fin doit etre codee explicitement (et non deduite uniquement d'un instantane).

---

## 3. Regles metier canoniques

## 3.1 Placement

Autorise :

- poser un point sur toute case vide

Interdit :

- poser sur une case deja occupee

Effet :

- apres placement, verifier toutes les lignes potentielles de 5 incluant le nouveau point, si pour former une ligne il doit passer par un mur adverse (alignement déjà existante de 5 points du joueurs adverse), alors ce points ne formeront pas de ligne.

## 3.2 Validation d'une ligne de 5

Une ligne est valide si et seulement si :

- 5 points consecutifs du meme joueur
- alignes en horizontal, vertical, diagonale montante ou diagonale descendante
- aucune case de cette ligne n'est un point invulnerable adverse

Interpretation cle :

- les points invulnerables adverses sont des murs logiques
- ils n'empechent pas le placement sur une case vide ailleurs
- ils empechent uniquement la validation d'une ligne qui traverserait ces cases

## 3.3 Propriete d'une ligne validee

Quand une ligne est validee :

- +1 point au joueur
- les 5 points de la ligne deviennent invulnerables
- ces points restent visibles sur le plateau
- ces points ne peuvent plus etre detruits

## 3.4 Reutilisation et intersections

Autorise :

- reutiliser ses propres points dans plusieurs lignes
- croiser ses propres lignes

Interdit implicitement :

- une ligne candidate incluant au moins un point invulnerable adverse ne valide pas

## 3.5 Canon

Position des canons :

- joueur 1 a gauche
- joueur 2 a droite

Parametres de tir :

- une ligne de tir (axe vertical)
- une puissance de 1 a 9

Effet du tir sur la case touchee :

- case vide : aucun effet
- point du joueur courant : aucun effet
- point adverse invulnerable : aucun effet
- point adverse non invulnerable : point detruit

Regle de score associee :

- detruire un point adverse ne modifie pas le score

---

## 4. Cycle de jeu de reference

1. Le joueur courant choisit son action (placement ou tir).
2. L'action est validee (regles de legalite).
3. L'action est appliquee sur l'etat du plateau.
4. Si action = placement, verifier les nouvelles lignes de 5.
5. Marquer invulnerabilite et incrementer score si lignes valides.
6. Enregistrer l'action dans l'historique (sequence_number).
7. Evaluer la condition de fin de partie.
8. Changer de joueur.

---

## 5. Persistance de reference (PostgreSQL)

## 5.1 Intention metier

Le modele SQL est un event store leger :

- table games : parties
- table points : etat du plateau
- table shots : tirs
- table moves : ordre chronologique des actions

### 5.2 Invariant a conserver

- pour une partie donnee, sequence_number est strictement unique et croissant

Cet invariant rend possible :

- reprise de partie fiable
- reconstruction exacte d'un replay

---

## 6. Replay et historique

## 6.1 Attendu fonctionnel

- mode consultation non destructif
- navigation coup par coup (precedent/suivant)
- affichage du plateau correspondant a un index de sequence

## 6.2 Strategie recommandee

- charger la liste des moves ordonnee par sequence_number
- maintenir un pointeur currentMoveIndex
- appliquer ou annuler un move selon la direction de navigation
- interdire toute action de jeu en mode replay

---

## 7. Etat reel du code (constat au 22/03/2026)

## 7.1 Ce qui est deja implemente

- application WinForms demarrable (build OK net10.0-windows)
- interface moderne composee en code (form principal)
- controle visuel de canon gauche/droite avec animation verticale
- etat logique minimal : taille grille, position des canons, score
- adaptation dynamique de taille de plateau (UI)

## 7.2 Ce qui n'est pas encore implemente

- aucun modele de case/point sur le plateau jouable
- aucun placement de point utilisateur
- aucune detection de ligne de 5
- aucune logique invulnerable / mur logique
- aucune logique de tir effective sur des points
- aucune persistance PostgreSQL dans le code C#
- aucun launcher de parties (nouvelle/continuer/revoir)
- aucun mode replay operationnel

## 7.3 Conclusion de maturite

Le projet est actuellement au stade socle UI + squelette logique. Les regles metier centrales de Gomoku-Canon restent a implementer.

---

## 8. Ambiguities et decisions a trancher

Les points suivants ne bloquent pas la redaction, mais doivent etre arbitres avant implementation finale pour eviter des reworks.

## 8.1 Schema SQL : divergence entre documents

Differences observees :

- games : size (README.md) vs grid_rows/grid_cols (create_database.sql)
- players : username (README.md) vs name (create_database.sql)
- shots : row/col (README.md) vs target_row/target_col (create_database.sql)
- contraintes NOT NULL/UNIQUE non parfaitement alignees

Decision recommandee :

- conserver un schema unique cible avant coder les repositories

## 8.2 Definition exacte de "plateau plein"

Ambiguite :

- fin immediatement quand toutes les cases sont occupees a un instant t
- ou fin seulement si aucune suite de tours ne peut rouvrir des cases (avec tirs)

Decision recommandee :

- regle simple et deterministe : fin si aucune case vide a la fin du tour courant

## 8.3 Multiples lignes creees par un seul placement

Question :

- un placement qui valide 2+ lignes en meme temps donne-t-il +2, +3, etc. ?

Decision recommandee :

- oui, +1 par ligne distincte validee

## 8.4 Gestion des lignes deja comptees

Question :

- comment eviter de recompter indefiniment la meme combinaison de 5 ?

Decision recommandee :

- stocker les lignes validees (hash de cellules triees) ou imposer une regle de detection incrementale centree sur le dernier coup

---

## 9. Architecture cible recommandee

## 9.1 Couches

- Domain : regles pures (GameState, PointCell, LineValidator, ShotResolver)
- Application : orchestration de tour (PlayTurnService)
- Infrastructure : acces PostgreSQL (Npgsql + repositories)
- UI WinForms : rendu, interactions, navigation replay

## 9.2 Objets metier minimaux

- Game : id, size, currentPlayer, status, scores
- Cell : row, col, ownerId nullable, isInvulnerable
- Move : sequenceNumber, type (place|shot), payload
- Shot : playerId, row, col, power
- Line : 5 coordonnees, ownerId, validatedAtMove

## 9.3 Regles d'or de conception

- aucune regle metier dans les handlers UI
- toute regle testable sans WinForms
- une transaction DB par action de joueur

---

## 10. Plan d'implementation propose (ordre optimise)

1. Stabiliser le schema SQL unique et migrations.
2. Implementer le modele grille/case en memoire.
3. Implementer action Placement + validation lignes + scoring.
4. Implementer invulnerabilite et blocage par mur logique.
5. Implementer action Tir + destruction conditionnelle.
6. Enregistrer les actions dans moves avec sequence_number.
7. Construire launcher (nouvelle/continuer/revoir).
8. Implementer replay avant/arriere base sur moves.
9. Ajouter tests unitaires regles critiques.
10. Ajouter tests d'integration DB sur scenario complet.

---

## 11. Scenarios de validation (acceptance)

## 11.1 Ligne valide simple

- etant donne 4 points alignes du joueur A
- quand A place le 5e point
- alors score A augmente de 1
- et les 5 points deviennent invulnerables

## 11.2 Mur logique adverse

- etant donne une ligne candidate de A contenant 1 point invulnerable de B
- quand A complete l'alignement de 5
- alors aucun point n'est accorde

## 11.3 Tir sur point invulnerable

- etant donne un point adverse invulnerable en case cible
- quand le joueur tire dessus
- alors le point reste en place
- et le score ne change pas

## 11.4 Tir sur point adverse non protege

- etant donne un point adverse non invulnerable en case cible
- quand le joueur tire dessus
- alors le point est supprime
- et le score ne change pas

## 11.5 Replay deterministe

- etant donne une partie avec N moves
- quand on rejoue de 1 a N
- alors l'etat final reconstruit = etat final sauvegarde

---

## 12. Checklist de definition of done

- regles metier implementees et testees
- schema SQL coherent avec le code
- sauvegarde/reprise operationnelles
- replay precedent/suivant operationnel
- aucun couplage fort UI/regles
- build net10.0-windows sans erreurs

---

## 13. Resume executif

CanonPoint dispose deja d'une base UI solide et compile correctement. La prochaine etape n'est pas cosmetique : il faut implementer le moteur de jeu (placement, lignes, mur logique, tir), puis brancher la persistance et le replay. Ce document sert de reference unique pour derisquer cette phase.
