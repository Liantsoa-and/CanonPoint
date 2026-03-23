# Guide rapide - Lancer, jouer, revoir

## 1. Prerequis

- .NET SDK 10 installe
- PostgreSQL accessible avec la chaine de connexion configuree dans `src/CanonPoint.App/appsettings.Development.json`

## 2. Lancer le projet

Depuis la racine du repo:

```powershell
dotnet build CanonPoint.sln
dotnet run --project src/CanonPoint.App
```

URL locales usuelles:

- `http://localhost:5116/`
- `https://localhost:7181/`

## 3. Demarrer une partie

1. Ouvrir la page d'accueil (`/`).
2. Saisir joueur 1, joueur 2, taille de grille.
3. Cliquer sur `Jouer`.
4. La redirection ouvre le plateau (`/plateau.html?...`).

## 4. Jouer sur le plateau

- Poser un point: cliquer sur une case/intersection du plateau.
- Deplacer canon: fleches gauche/droite selon le joueur.
- Tirer: choisir une puissance (1..9), puis cliquer `Tirer`.
- Les messages de succes/erreur s'affichent sous les controles.

## 5. Revoir la partie (replay)

1. Cliquer `Mode replay`.
2. Naviguer avec `◀` (precedent) et `▶` (suivant).
3. Quitter avec `Quitter replay`.

Notes:

- Pendant replay, les actions de jeu (pose/tir/deplacement) sont bloquees.
- Le replay est non destructif: il n'ecrit pas de nouveaux coups.

## 6. Lancer les tests

Depuis la racine:

```powershell
dotnet test CanonPoint.sln --no-build
```

## 7. Depannage rapide

- Si la page est inaccessible: verifier que `dotnet run` est en cours.
- Si erreur DB: verifier la connexion PostgreSQL et les migrations/schema.
- Si warning EF Core de version: aligner les versions EF Core package par package.
