Concevoir une interface de jeu avec les spécifications suivantes :

1. Plateau
Créer un plateau en grille similaire à un échiquier
Le nombre de lignes et de colonnes doit être dynamique (paramétrable depuis le formulaire qu'on vient de creer)
Chaque case doit être clairement visible (bordures visibles)
Optionnel : alternance de couleurs pour améliorer la lisibilité
2. Scrollbars latérales
Ajouter 2 scrollbars verticales uniquement :
une à gauche du plateau
une à droite du plateau
Aucune scrollbar en haut ou en bas
3. Contrôles des scrollbars
Chaque scrollbar contient :
une flèche en haut (pour monter)
une flèche en bas (pour descendre)
Ces boutons permettent de déplacer un élément le long de la scrollbar
4. Canons
Chaque scrollbar possède un canon
Le canon se déplace case par case (pas de mouvement libre)
La position du canon est alignée exactement avec les lignes du plateau
Le canon doit être visuellement distinct (forme, couleur ou icône)
Le déplacement doit être fluide visuellement (animation simple entre les cases)
5. Synchronisation avec le plateau
Le canon correspond toujours à une ligne précise du plateau
Quand le canon bouge, il reste parfaitement aligné avec les cases de la grille
6. Affichage des scores
En bas du plateau :
afficher le score de deux joueurs
format simple :
Joueur 1 : X
Joueur 2 : Y
7. Contraintes visuelles
Interface propre et lisible
Alignements précis entre :
scrollbars
canons
lignes du plateau
Animations simples pour rendre le déplacement fluide
8. Contraintes techniques
Code structuré et maintenable
Prévoir une séparation logique/affichage si possible
L’interface doit pouvoir être facilement étendue (ajout futur d’interactions comme tir ou sélection)

