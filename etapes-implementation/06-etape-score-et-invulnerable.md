# Etape 6 - Scoring et invulnerabilite de ligne validee

But: convertir les lignes candidates en gain de score + protection.

Sous-taches:

- Pour chaque ligne validee: +1 score au joueur.
- Marquer les 5 points de la ligne en invulnerable.
- Ne jamais diminuer le score.
- Stocker une trace des lignes deja comptees (hash conseille).
- Verifier qu'une meme ligne n'est pas recomptee au coup suivant.

Livrable:

- Mecanisme de score stable et non repetitif.

Definition terminee:

- Rejouer le meme pattern ne donne pas de points en boucle.

Temps estime: 45-90 min.
