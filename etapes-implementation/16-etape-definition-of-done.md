# Etape 16 - Fermeture projet (Definition of Done)

But: terminer proprement sans dette cachee.

Sous-taches:

- Passer la checklist DoD complete.
- Verifier build propre en local.
- Verifier scenarios d'acceptance du document spec.
- Faire un mini guide "Comment lancer / jouer / revoir".
- Lister les prochains axes (optimisation, UX, IA, multi-parties).

Livrable:

- Projet coherent, stable, et transmissible.

Definition terminee:

- Toutes les cases DoD sont validees et tracees.

Temps estime: 30-60 min.

Approche pas a pas suivie:

1. Verifier la checklist DoD par preuves concretes (code + tests).
2. Refaire une validation globale build + tests.
3. Tracer les scenarios d'acceptance couverts.
4. Produire un mini guide d'usage pour transmission.
5. Lister les prochains axes prioritaires.

Checklist DoD (tracee):

- [x] Regles metier implementees et testees.
	- Placement, detection lignes, mur logique adverse, tir, orchestration tour.
	- Couvert par tests unitaires Domain et tests d'integration.

- [x] Schema SQL coherent avec le code.
	- Entites EF + index unique `(GameId, SequenceNumber)` en place.
	- Persistance actions dans transaction explicite.

- [x] Sauvegarde/reprise operationnelles.
	- Historique moves persiste et triable par sequence.
	- Reconstruction d'etat depuis historique implementee.

- [x] Replay precedent/suivant operationnel.
	- Replay UI sur `plateau.html` avec navigation `precedent/suivant`.
	- Actions de jeu bloquees pendant replay.

- [x] Aucun couplage fort UI/regles.
	- Regles principales en couche Domain/Services.
	- UI branchee sur API sans logique metier lourde.

- [x] Build local sans erreurs.
	- Build solution OK.
	- Suite de tests OK.

Verification scenarios d'acceptance (spec):

- Ligne valide simple: couvert (detection + scoring + invulnerabilite).
- Mur logique adverse: couvert (ligne rejetee, score inchange).
- Tir sur point invulnerable: couvert (aucun effet).
- Tir sur point adverse non protege: couvert (suppression).
- Replay deterministe: couvert (reconstruction historique deterministe).

Mini guide realise:

- Voir le fichier `GUIDE_LANCER_JOUER_REVOIR.md` a la racine du projet.

Prochains axes recommandes:

1. Aligner completement les versions EF Core pour supprimer le warning restant.
2. Exposer un endpoint replay dedie cote API (snapshot par index) pour fiabiliser le replay UI.
3. Brancher l'orchestration Domain (`ITurnService`) dans l'API pour unifier totalement les regles.
4. Ajouter des tests d'integration PostgreSQL purs (ex: Testcontainers) en complement SQLite.
5. Etendre la gestion de fin de partie et l'affichage score metier officiel (lignes validees).

Resultat:

- L'etape 16 est completee.
- La Definition of Done est validee et tracee.
