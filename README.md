# 2 FINGERS NO SKILL

Petite copie volontaire de *One Finger Death Punch* en C#/.NET 8 avec MonoGame.
Les règles et l'architecture sont décrites dans [docs/game_Design.md](docs/game_Design.md).

## Prérequis

- **SDK .NET** 8 ou plus récent (le projet cible `net8.0`, le runtime .NET 8 doit être installé).
- Les outils MonoGame (compilateur de contenu `mgcb`) sont restaurés automatiquement au premier build.
  Pour les restaurer à la main : `dotnet tool restore` depuis `src/two_Fingers_No_Skill/`.

## Lancer

```bash
dotnet run --project src/two_Fingers_No_Skill
```

## Tests

```bash
dotnet test
```

Les tests couvrent toute la logique de jeu (`config/`, `core/`, `entities/`, `systems/`).
L'affichage et les entrées se vérifient en jouant.

## Commandes

| Touche | Action |
|---|---|
| Clic gauche (M1) | Frapper à gauche / commencer une partie |
| Clic droit (M2) | Frapper à droite |
| Entrée | Rejouer après un game over |
| F1 | Afficher / masquer les zones de frappe (debug) |
| Échap | Quitter |

## Régler le jeu : `game_Settings.json`

Le fichier `src/two_Fingers_No_Skill/game_Settings.json` est copié à côté de l'exécutable et lu au démarrage.
Pas besoin de recompiler : modifie la copie dans `bin/Debug/net8.0/` pour tester, ou l'original puis relance.
Si le fichier est absent ou invalide, le jeu affiche l'erreur dans la console et utilise les valeurs par défaut.

| Clé | Rôle |
|---|---|
| `starting_Lives` | Vies au début d'une partie (≥ 1) |
| `score_Rules.enemy_Killed` | Points par ennemi tué |
| `score_Rules.missed_Hit` | Points par coup raté (négatif = pénalité) |
| `score_Rules.life_Lost` | Points par vie perdue |
| `minimum_Score` | Le score ne descend jamais sous cette valeur |
| `player.attack_Range` | Largeur de la zone de frappe de chaque côté, en pixels, depuis le bord du joueur |
| `player.contact_Distance` | Distance à laquelle un ennemi frappe le joueur, en pixels |
| `player.miss_Lockout_Seconds` | Durée du blocage après un coup raté |
| `slow_Motion_Time_Scale` | Vitesse du jeu pendant le ralenti (0.05 = 5 %) |
| `event_Log.max_Visible_Events` | Nombre de lignes du journal |
| `difficulty_Tiers` | Liste des paliers de difficulté (voir ci-dessous) |

Chaque palier :

| Clé | Rôle |
|---|---|
| `start_Time_Seconds` | Temps de jeu à partir duquel le palier s'applique (le premier doit être à 0) |
| `spawn_Interval_Seconds` | Délai entre deux apparitions d'ennemis |
| `enemy_Speed` | Vitesse des ennemis apparus pendant ce palier (pixels/s) |
| `max_Enemies_On_Screen` | Nombre maximum d'ennemis vivants en même temps |
| `slow_Motion_Duration_Seconds` | Durée du ralenti quand le joueur est frappé |

**Ajouter un palier** : ajoute un objet dans `difficulty_Tiers` avec son `start_Time_Seconds`.
L'ordre dans la liste n'a pas d'importance, les paliers sont triés au chargement.

## Organisation du code

```
src/two_Fingers_No_Skill/
├── config/       Réglages (classes miroir du JSON) et chargement/validation
├── core/         Horloge, évènements, dimensions de l'écran, match_World (logique d'une partie)
├── entities/     player, enemy, facing_Direction
├── systems/      Score, statistiques, journal, difficulté, apparitions, combat
├── input/        Lecture souris/clavier ("vient d'être appuyé")
├── rendering/    Couleurs, formes, interface (HUD), dessin du monde
├── game_States/  Écrans : titre, partie, game over, flash + machine à états
└── Content/      Polices (pipeline MonoGame)
```

La logique de jeu (`match_World` et ce qu'il utilise) ne dépend pas de MonoGame : c'est elle qui est testée.
Les écrans (`game_States/`) font le lien entre les entrées, la logique et l'affichage.

## Convention de nommage

Premier mot en minuscule, mots suivants séparés par `_` et commençant par une majuscule :
`enemy_Spawner`, `spawn_Enemy()`, `current_Lives`, `facing_Direction.left`.
Seules exceptions : les méthodes imposées par MonoGame (`Initialize`, `LoadContent`, `Update`, `Draw`)
et les noms imposés par les outils (`Content/`, `Content.mgcb`, `README.md`...).

## Étendre le jeu

- **Nouveau type d'évènement** : une valeur dans `core/game_Event_Type.cs`, une règle dans `config/score_Rules.cs`
  (+ `score_Manager.get_Points_For`, + clé JSON), un libellé dans `systems/event_Log.cs`.
  Puis publie-le depuis `match_World` : score, stats et journal réagissent seuls via le bus d'évènements.
- **Nouvel écran** : une classe qui implémente `game_States/i_Game_State.cs`,
  activée avec `services.state_Machine.change_State(...)`.
- **Nouveau réglage** : une propriété dans la classe de `config/` correspondante, la même clé dans le JSON,
  la valeur par défaut dans `game_Settings.create_Default()` (un test vérifie que les deux restent identiques).
