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
| Espace | Mettre en pause / reprendre la partie |
| Entrée | Rejouer après un game over |
| F1 | Afficher / masquer les zones de frappe (debug) |
| Échap | Quitter |

## Ce qu'affiche l'écran de jeu

- En haut au centre : le score.
- En haut à gauche : le rythme actuel, c'est-à-dire la vitesse des ennemis par rapport au début de partie (100 %).
- En haut à droite : le timer de la partie (`MM:SS`).
- En bas à gauche : le journal d'évènements. Les hausses de rythme y apparaissent en doré.
- En bas à droite : les vies.
- Au sol, en doré : la portée de frappe de chaque côté du joueur.

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

### Vue d'ensemble

```
2FingersNoSkill/
├── .editorconfig                     Style de code + désactivation des règles de nommage .NET par défaut
├── .gitignore                        Fichiers exclus de git (bin/, obj/, .vs/...)
├── README.md                         Ce fichier
├── two_Fingers_No_Skill.slnx         Solution : regroupe le projet du jeu et le projet de tests
├── docs/
│   ├── game_Design.md                Document de conception (règles du jeu, écrans, choix)
│   ├── implementation_Plan.md        Plan de développement initial, tâche par tâche
│   └── screenshots/                  Captures d'écran du jeu (portfolio)
├── src/two_Fingers_No_Skill/         LE JEU
│   ├── two_Fingers_No_Skill.csproj   Projet : .NET 8, MonoGame, copie du JSON de réglages
│   ├── program.cs                    Point d'entrée
│   ├── game_Root.cs                  Classe principale MonoGame (fenêtre + boucle)
│   ├── game_Settings.json            Réglages modifiables sans recompiler
│   ├── app.manifest, Icon.ico/.bmp   Fichiers Windows fournis par le modèle MonoGame
│   ├── .config/dotnet-tools.json     Outils MonoGame (compilateur de contenu mgcb)
│   ├── config/                       Réglages : classes miroir du JSON, chargement, validation
│   ├── core/                         Cœur d'une partie : horloge, évènements, match_World
│   ├── entities/                     Les personnages : joueur, ennemi, direction
│   ├── systems/                      Règles : combat, difficulté, apparitions, score, journal...
│   ├── input/                        Lecture souris/clavier
│   ├── rendering/                    Dessin : formes, couleurs, monde, interface
│   ├── game_States/                  Les écrans du jeu + machine à états
│   └── Content/                      Ressources compilées par MonoGame (polices)
└── tests/two_Fingers_No_Skill.tests/ LES TESTS (xUnit)
```

**Le principe à retenir :** tout ce qui est dans `config/`, `core/`, `entities/` et `systems/` est de la logique pure,
sans aucun dessin ni dépendance à l'affichage. C'est cette partie qui est couverte par les tests.
`input/`, `rendering/` et `game_States/` font le lien entre cette logique, le clavier/souris et l'écran.

Le trajet d'une image (frame) :
`game_Root.Update` → lecture des entrées (`input/`) → écran actif (`game_States/`) → `match_World.update` (`core/`)
→ systèmes (`systems/`) → évènements publiés → score, stats et journal mis à jour.
Puis `game_Root.Draw` → écran actif → `world_Renderer` + `hud_Renderer` (`rendering/`).

### Racine de `src/two_Fingers_No_Skill/`

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `program.cs` | Point d'entrée de l'application | Deux lignes : crée `game_Root` et lance la boucle MonoGame. |
| `game_Root.cs` | Chef d'orchestre | Taille de la fenêtre (1280×720). `LoadContent` : chargement des réglages, des 3 polices, de l'écran titre. `Update` : Échap pour quitter, lecture des entrées filtrées par `focus_Input_Guard`, bascule F1, puis délégation à l'écran actif. `Draw` : efface l'écran et fait dessiner l'écran actif. |
| `game_Settings.json` | Équilibrage du jeu | Vies, points, portée de frappe, blocage après un raté, ralenti, taille du journal, liste des paliers de difficulté. Copié à côté de l'exécutable à chaque build. |
| `two_Fingers_No_Skill.csproj` | Configuration du projet | Cible .NET 8, paquets MonoGame, désactivation de l'avertissement CS8981 (noms tout en minuscules), copie de `game_Settings.json`. |

### `config/` : les réglages

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `game_Settings.cs` | Racine des réglages | Une propriété par clé du JSON (même nom exact). `create_Default()` : les valeurs par défaut, identiques au JSON livré (un test le vérifie). |
| `difficulty_Tier.cs` | Un palier de difficulté | Début du palier, intervalle d'apparition, vitesse des ennemis, nombre max d'ennemis, durée du ralenti. |
| `score_Rules.cs` | Barème de points | Points pour un ennemi tué, un coup raté, une vie perdue. |
| `player_Settings.cs` | Réglages du combat | Portée de frappe, distance de contact, durée du blocage après un raté. |
| `event_Log_Settings.cs` | Réglages du journal | Nombre de lignes affichées. |
| `settings_Loader.cs` | Lecture du JSON | `load_From_File` : ne plante jamais, signale l'erreur et repart sur les valeurs par défaut. `parse_Json` : lit, refuse les clés inconnues ou manquantes, trie les paliers, valide chaque valeur. |
| `settings_Validation_Exception.cs` | Erreur de réglage | Exception levée quand une valeur est hors limites (ex. vies à 0). |

### `core/` : le cœur d'une partie

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `match_World.cs` | **Une partie complète** | Le joueur, les ennemis, les vies, le game over. `update` : avance le temps, annonce les changements de palier, fait apparaître et avancer les ennemis, résout les contacts (vie perdue + ralenti). `handle_Strike` : résout une frappe (ennemi tué ou coup raté + blocage). Branche score, stats et journal sur le bus d'évènements. |
| `game_Clock.cs` | Temps de la partie | Convertit le temps réel en temps de jeu, gère le ralenti (vitesse réduite pendant une durée en temps réel). C'est ce temps qui pilote les paliers et le timer. |
| `game_Event_Type.cs` | Liste des évènements | `enemy_Killed`, `life_Lost`, `missed_Hit`, `rhythm_Increased`. |
| `game_Event.cs` | Un évènement | Type + moment de la partie + valeur optionnelle (ex. le pourcentage de rythme). |
| `game_Event_Bus.cs` | Diffusion des évènements | `publish` prévient tous les abonnés (score, stats, journal) sans qu'ils se connaissent. |
| `world_Layout.cs` | Dimensions fixes | Taille de l'écran, position du sol, taille et position du joueur et des ennemis. |

### `entities/` : les personnages

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `player.cs` | Le joueur | Position fixe au centre, direction regardée (gauche par défaut), blocage après un coup raté et son minuteur. |
| `enemy.cs` | Un ennemi | Côté d'apparition, position, vitesse. `get_Gap_To` : distance entre ses bords et ceux du joueur. `move_Toward` : avance sans jamais chevaucher le joueur. |
| `facing_Direction.cs` | Gauche / droite | Utilisé pour le regard du joueur, le côté d'une frappe et le côté d'apparition. |

### `systems/` : les règles du jeu

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `combat_System.cs` | Règles de combat | `find_Strike_Target` : l'ennemi le plus proche dans la zone de frappe du côté frappé (ou rien = coup raté). `has_Reached` : un ennemi touche-t-il le joueur ? |
| `difficulty_Manager.cs` | Paliers de difficulté | Palier actif selon le temps de jeu, et pourcentage de rythme (vitesse des ennemis ÷ vitesse du premier palier). |
| `enemy_Spawner.cs` | Apparition des ennemis | Minuteur d'apparition, côté tiré au hasard, au plus un ennemi par image, attente si l'écran est plein. |
| `score_Manager.cs` | Score | Applique le barème à chaque évènement, ne descend jamais sous le minimum. |
| `match_Stats.cs` | Statistiques | Compte les ennemis tués, coups ratés et vies perdues (affichés au game over). |
| `event_Log.cs` | Journal d'évènements | Garde les dernières lignes et fabrique leur texte français (« Ennemi tué (+20) », « Rythme : 111 % »...). |
| `event_Log_Entry.cs` | Une ligne du journal | Type d'évènement + texte affiché. |
| `time_Formatter.cs` | Format du timer | Transforme des secondes en `MM:SS`. |

### `input/` : clavier et souris

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `input_Handler.cs` | Lecture des périphériques | Compare l'image actuelle à la précédente pour ne détecter que les appuis (M1, M2, Entrée, F1, Espace), pas les maintiens. |
| `input_Snapshot.cs` | Actions d'une image | Un booléen par action ; `empty` = aucune action. |
| `focus_Input_Guard.cs` | Protection du focus | Ignore la souris tant que le clic qui a redonné le focus à la fenêtre n'est pas relâché (ce clic ne compte jamais comme une frappe). |

### `rendering/` : l'affichage

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `render_Context.cs` | Boîte à outils de dessin | Regroupe le SpriteBatch, les formes et les 3 polices ; texte centré, voile plein écran. |
| `shape_Renderer.cs` | Formes simples | Rectangles pleins et contours en pointillés, à partir d'un seul pixel blanc étiré. |
| `world_Renderer.cs` | Le terrain | Sol, portée de frappe dorée au sol, joueur (gris pendant le blocage, « bras » côté regard), ennemis, zones de debug (F1). |
| `hud_Renderer.cs` | L'interface | Score, rythme, timer, vies, journal (lignes de rythme en doré et plus grandes), avec l'animation de glissement depuis les bords. |
| `game_Colors.cs` | Palette | Toutes les couleurs du jeu. Changer une valeur ici restyle tout le jeu. |
| `easing.cs` | Courbes d'animation | `ease_Out_Cubic` : départ rapide, arrivée douce. |

### `game_States/` : les écrans

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `i_Game_State.cs` | Contrat d'un écran | Deux méthodes : `update` (logique) et `draw` (affichage). |
| `game_State_Machine.cs` | Gestion des écrans | Garde l'écran actif et lui transmet chaque image ; `change_State` pour changer d'écran. |
| `game_Services.cs` | Objets partagés | Réglages, machine à états, générateur aléatoire, mode debug. Donnés à chaque écran. |
| `ui_Texts.cs` | Textes affichés | Tous les textes français des écrans au même endroit. |
| `title_State.cs` | Écran titre | Joueur au centre, titre, consigne ; M1 lance une partie. |
| `playing_State.cs` | Partie en cours | Transmet les frappes à `match_World`, dessine le terrain et l'interface, Espace ouvre la pause, passe au game over à 0 vie. |
| `pause_State.cs` | Pause | Partie figée sous un voile sombre ; Espace reprend la même partie. |
| `game_Over_State.cs` | Game over | Partie figée sans le joueur, score, durée, ennemis tués, coups ratés ; Entrée relance. |
| `restart_Flash_State.cs` | Flash de relance | Fondu au blanc, bascule vers l'écran titre au pic, fondu retour. |

### `Content/` : les ressources

| Fichier | Rôle | Ce qu'on y trouve |
|---|---|---|
| `Content.mgcb` | Liste des ressources | Ce que l'outil MonoGame doit compiler (les 3 polices). |
| `fonts/hud_Font.spritefont` | Police standard | Consolas 18 : score, timer, rythme, journal, consignes. |
| `fonts/title_Font.spritefont` | Grande police | Consolas 40 gras : titre du jeu, « GAME OVER », « PAUSE ». |
| `fonts/highlight_Font.spritefont` | Police mise en avant | Consolas 22 gras : lignes de rythme dans le journal. |

### `tests/two_Fingers_No_Skill.tests/` : les tests

| Fichier | Ce qui est vérifié |
|---|---|
| `two_Fingers_No_Skill.tests.csproj` | Projet de tests xUnit ; référence le jeu et récupère `game_Settings.json`. |
| `settings_Loader_Tests.cs` | Valeurs par défaut = JSON livré, fichier absent ou cassé, valeurs invalides, clés mal écrites ou manquantes, tri des paliers. |
| `game_Clock_Tests.cs` | Écoulement du temps, ralenti et sa durée en temps réel. |
| `game_Event_Bus_Tests.cs` | Tous les abonnés reçoivent les évènements. |
| `score_Manager_Tests.cs` | +20, −50, plancher à 0, rythme sans points. |
| `match_Stats_Tests.cs` | Comptage de chaque type d'évènement. |
| `event_Log_Tests.cs` | Textes français, ordre, limite de lignes. |
| `difficulty_Manager_Tests.cs` | Palier selon le temps, durées de ralenti, pourcentages de rythme. |
| `enemy_Spawner_Tests.cs` | Délai d'apparition, écran plein, un ennemi max par image, deux côtés utilisés. |
| `player_Tests.cs` | Regard par défaut, fin du blocage. |
| `combat_System_Tests.cs` | Distances, arrêt au contact, cible la plus proche, portée. |
| `match_World_Tests.cs` | Partie complète : frappes, ratés, blocage, contacts, ralenti, game over, grosses images, paliers et annonce du rythme. |
| `focus_Input_Guard_Tests.cs` | Le clic de focus n'est jamais une frappe. |
| `pause_State_Tests.cs` | Ouverture de la pause, partie figée, reprise de la même partie. |
| `time_Formatter_Tests.cs` | Format `MM:SS`. |

## Convention de nommage

Premier mot en minuscule, mots suivants séparés par `_` et commençant par une majuscule :
`enemy_Spawner`, `spawn_Enemy()`, `current_Lives`, `facing_Direction.left`.
Seules exceptions : les méthodes imposées par MonoGame (`Initialize`, `LoadContent`, `Update`, `Draw`)
et les noms imposés par les outils (`Content/`, `Content.mgcb`, `README.md`...).

## Étendre le jeu

- **Nouveau type d'évènement** : une valeur dans `core/game_Event_Type.cs`, une règle dans `config/score_Rules.cs`
  (+ `score_Manager.get_Points_For`, + clé JSON), un libellé dans `systems/event_Log.cs`.
  Puis publie-le depuis `match_World` : score, stats et journal réagissent seuls via le bus d'évènements.
  Un évènement peut porter un nombre (`game_Event.value`), comme le pourcentage de `rhythm_Increased`.
- **Nouvel écran** : une classe qui implémente `game_States/i_Game_State.cs`,
  activée avec `services.state_Machine.change_State(...)`.
- **Nouveau réglage** : une propriété dans la classe de `config/` correspondante, la même clé dans le JSON,
  la valeur par défaut dans `game_Settings.create_Default()` (un test vérifie que les deux restent identiques).
