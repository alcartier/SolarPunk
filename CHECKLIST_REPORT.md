# Rapport de verification — Checklist Potager Solarpunk

> Analyse realisee sur l'ensemble du projet : 20 scripts C#, 2 scenes Unity, 18 prefabs, fichiers audio, animations, et structure des dossiers.
> Certains criteres ne peuvent etre verifies qu'en lancant le jeu dans l'editeur ou en build — ils sont marques **[A verifier en jeu]**.

---

## 1. Deployer un build jouable sur itch.io (WebGL ou Standalone)

| Critere | Statut | Detail |
|---------|--------|--------|
| Un lien itch.io mene a une build jouable | :x: NON | Aucune trace de deploiement ou de configuration WebGL dans le projet. |
| La build demarre sans erreur bloquante | :x: NON | Pas de build existante. |

**A faire :** Configurer le build WebGL (ou Standalone), tester, puis deployer sur itch.io.

---

## 2. Completer la page itch.io (description, controles, visuels)

| Critere | Statut | Detail |
|---------|--------|--------|
| Description du jeu publiee | :x: NON | Pas de page itch.io. |
| Controles listes | :x: NON | |
| Au moins 2 screenshots | :x: NON | |
| Credits des assets indiques | :x: NON | |

**A faire :** A completer une fois la build deployee. Les assets utilises (ForestPack, IL3DN, AssetHunts Farming, skyboxes, Simple Water Shader, UISFX, BGM) doivent etre credites.

---

## 3. Structurer la navigation entre scenes (Menu > Jeu > Fin)

| Critere | Statut | Detail |
|---------|--------|--------|
| L'ecran titre, la scene de jeu et l'ecran de fin s'enchainent correctement | :white_check_mark: OUI | 2 scenes (MainMenu, Game). L'ecran de fin est un panel UI (WinLoseUI) dans la scene Game. `SceneControler` gere les transitions avec fade. |
| L'ecran de fin apparait des que l'objectif est atteint ou que le temps est termine | :white_check_mark: OUI | `GameManager.Win()` declenche `OnGameEnd` a l'objectif. `GameTimer` declenche `GameOver()` quand le temps expire. |
| Un menu en cours de partie permet de recommencer ou de quitter | :x: NON | **Aucun menu pause n'existe.** Pas de reference a `Time.timeScale`, pas de script de pause. Le joueur ne peut pas quitter ou recommencer pendant la partie. |
| Recommencer remet le score et le potager a zero | :white_check_mark: OUI | `StartNewGame()` reinitialise score et amountHarvested, puis charge la scene Game. |

**A faire :** Implementer un menu pause (touche Echap) avec les options Reprendre / Recommencer / Menu principal.

---

## 4. Creer des Prefabs reutilisables et bien organises

| Critere                                             | Statut                 | Detail                                                                                                                                                                                                                            |
| --------------------------------------------------- | ---------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Graines, legumes et effets sont des Prefabs         | :white_check_mark: OUI | Seed.prefab, Carrot/Letuce/Corn.prefab, SmokePuffDestroy/Spawn.prefab, WaterParticles.prefab.                                                                                                                                     |
| Les Prefabs sont ranges dans `Prefabs/`             | :white_check_mark: OUI | Dossier `prefabs/` avec sous-dossiers Crops/, UI/, Utils/, VFX/. Quelques prefabs a la racine (Dirt, Seed, Drone) qui gagneraient a etre ranges dans un sous-dossier.                                                             |
| Modifier un Prefab change toutes ses instances      | :white_check_mark: OUI | Comportement standard Unity, les prefabs sont utilises correctement.                                                                                                                                                              |
| Les parametres utiles sont exposes dans l'Inspector | :white_check_mark: OUI | Vitesse du drone, durees de croissance, objectif de recolte, references audio/UI, tous exposes.                                                                                                                                   |
| Un Prefab Variant ou un Nested Prefab est utilise   | :warning: PARTIEL      | **Nested Prefab : OUI** — Drone.prefab contient WaterParticles en nested, les crops contiennent SmokePuff en nested. **Prefab Variant : NON** — les 3 crops sont des prefabs independants, pas des variantes d'un prefab de base. |

**Suggestion :** Creer un Prefab de base "CropBase" et en deriver Carrot, Letuce, Corn comme Prefab Variants. C'est le cas d'usage ideal pour cette fonctionnalite.
---------c'est FAIT

---

## 5. Instancier et detruire dynamiquement les objets

| Critere | Statut | Detail |
|---------|--------|--------|
| Les graines apparaissent sous le drone via Instantiate | :white_check_mark: OUI | `DropSeed.PlantSeed()` instancie le seedPrefab au spawnPoint. |
| Plusieurs graines peuvent exister en meme temps | :white_check_mark: OUI | Aucune limitation dans le code — chaque appel cree une nouvelle instance. |
| Les legumes recoltes ou eclates sont detruits avec Destroy | :white_check_mark: OUI | `Harvesting.HarvestCrop()` detruit via `Destroy(cropHolder.currentCropGameObject)`. `Crop.Update()` detruit en etat Dead. |
| Les objets detruits disparaissent de la Hierarchy | :white_check_mark: OUI | Comportement standard de `Destroy()`. |

---

## 6. Configurer Colliders, Triggers et Rigidbody

| Critere | Statut | Detail |
|---------|--------|--------|
| Un Trigger detecte le drone au-dessus d'un legume | :white_check_mark: OUI | `OnTriggerEnter`/`OnTriggerExit` dans Watering et Harvesting detectent les colliders tagges "Dirt". |
| Seul un legume mur est recolte | :white_check_mark: OUI | `Harvesting.HarvestCrop()` verifie `currentState == CropState.Mature`. |
| Sur 5 recoltes d'affilee, l'interaction fonctionne a chaque fois | :white_check_mark: PROBABLE | La logique est correcte (la fix OnTriggerExit est appliquee). **[A verifier en jeu]** |
| Aucune erreur dans la Console quand un legume eclate pendant que le drone passe dessus | :warning: A VERIFIER | Le `CropHolder` ne nettoie pas ses references quand la crop se detruit elle-meme. Unity gere ca via son null override, mais c'est fragile. **[A verifier en jeu]** |

---

## 7. Implementer l'Input System multi-device

| Critere | Statut | Detail |
|---------|--------|--------|
| Un asset Input Actions contient deplacement, plantation et arrosage | :white_check_mark: OUI | ActionMap "Player" avec 6 actions : Move, Plant, Water, Harvest, SwitchCrop+, SwitchCrop-. |
| Le drone se deplace dans les 4 directions | :white_check_mark: OUI | `DroneMovement.Update()` lit le Vector2 du Move action. |
| Les actions repondent au clavier et a la manette | :white_check_mark: OUI | Bindings clavier + manette configures dans l'asset. |
| Aucun script different par peripherique | :white_check_mark: OUI | Un seul jeu de scripts, pas de gestion conditionnelle par device. |
| Le composant PlayerInput n'est pas utilise | :white_check_mark: OUI | Aucune reference a `PlayerInput` dans le code. Les scripts utilisent `InputActionReference` directement. |

---

## 8. Structurer le code avec les cycles MonoBehaviour

| Critere                                                                     | Statut                 | Detail                                                                                                                                                                                                                                                                                                                       |
| --------------------------------------------------------------------------- | ---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Start, Update, OnTriggerEnter/Exit utilises a bon escient                   | :white_check_mark: OUI | Chaque callback Unity est utilise de maniere appropriee.                                                                                                                                                                                                                                                                     |
| Chaque script a une responsabilite claire                                   | :warning: QUASI        | La plupart des scripts sont bien decoupes (1 script = 1 responsabilite). `GameManager` est legerement surcharge (score + harvest + win/lose + save/load + timer duration) mais c'est acceptable pour la taille du projet. `SceneFade.cs` est vide (code mort).                                                               |
| Un legume recolte est detruit                                               | :white_check_mark: OUI |                                                                                                                                                                                                                                                                                                                              |
| Les noms de variables et scripts indiquent clairement ce qu'ils contiennent | :warning: PARTIEL      | **Problemes de nommage :** `OnGameEnd2` (nom obscur — devrait etre `OnGameCleanup`), `getHarvestObjective`/`getCropScore` (camelCase au lieu de PascalCase), `ActiveMenu` (devrait etre `ActivateMenu`), `SceneControler` (typo — `SceneController`), `backGround` (devrait etre `background`), `Letuce` (typo — `Lettuce`). |

---

## 9. Appliquer Time.deltaTime et les Coroutines

| Critere | Statut | Detail |
|---------|--------|--------|
| Chaque plantation passe par 3 etapes : graine, legume mur, legume trop mur | :white_check_mark: OUI | Enum `CropState` : Young -> Mature -> Dead. La graine (`SeedOnCollision`) est une etape separee avant la plantation. |
| La duree de chaque etape se regle depuis l'Inspector | :white_check_mark: OUI | `growthCooldown` et `matureCooldown` exposes sur le composant Crop. |
| Deux graines plantees a quelques secondes d'ecart ne murissent pas en meme temps | :white_check_mark: OUI | Chaque instance de Crop a son propre cooldown independant. |
| L'attente ne bloque pas le reste du jeu | :white_check_mark: OUI | La progression utilise `Time.deltaTime` dans `Update()`, pas de boucle bloquante. Coroutines utilisees dans `SceneControler` (fade) et `GameTimer` (countdown). |

---

## 10. Creer des AnimationClips et un Animator

| Critere | Statut | Detail |
|---------|--------|--------|
| Au moins un AnimationClip est cree | :white_check_mark: OUI | `DroneMainMenu.anim` (animation du drone dans le menu) et `DronePropeller.anim` (helices). |
| Un Animator Controller est assigne | :white_check_mark: OUI | `Drone.controller` et `DroneModel.controller`. |
| Au moins un autre element est anime (decor, drone ou interface) | :x: NON | **Seul le drone est anime.** Aucune animation sur le decor, l'interface ou les cultures. Les animations LitMotion (tweens de scale) ne sont pas des AnimationClips Unity. |
| Les animations ne perturbent ni le deplacement ni la recolte | :white_check_mark: OUI | L'Animator est sur le visuel du drone, pas sur le GameObject racine. |

**A faire :** Ajouter au moins une animation sur un autre element (par exemple : animation de l'UI de score quand il change, animation d'un element de decor, ou animation d'entree du menu).

---

## 11. Integrer des assets externes

| Critere | Statut | Detail |
|---------|--------|--------|
| Les modeles et textures sont importes sans materiau rose | :warning: A VERIFIER | Les materiaux semblent configures (12 .mat), les shaders sont en ShaderGraph URP. **[A verifier en jeu — les materiaux roses n'apparaissent qu'au runtime]** |
| Des SFX et au moins une musique sont importes | :warning: PARTIEL | **SFX : OUI** — DirtSfx, DroneFlying, harvest, Pop-Reverb, Watering, ~90 UI SFX. **BGM : le fichier existe** (`oceanking-sky-farm-219728.mp3`) et il est joue dans le menu (AudioSource sur le Drone du MainMenu), **mais pas dans la scene Game**. |
| Un ciel et un eclairage coherents avec le theme solarpunk | :warning: A VERIFIER | Des skyboxes existent (day, morning, alien, etc.) et des GlobalVolume/PostProcess sont configures. **[A verifier visuellement]** |
| Aucune erreur d'import dans la Console | :warning: A VERIFIER | **[A verifier dans l'editeur Unity]** |
| Aucun objet ne ressemble a une primitive Unity | :warning: A VERIFIER | Des modeles 3D sont importes (drone FBX, ForestPack, crop models). **[A verifier visuellement que tous les objets ont des meshes customs]** |

---

## 12. Exposer les references pertinentes via l'Inspector

| Critere | Statut | Detail |
|---------|--------|--------|
| Prefabs references depuis l'Inspector (pas de Find) | :white_check_mark: OUI | **Aucun `GameObject.Find` ni `FindObjectOfType` dans tout le projet.** Toutes les references sont via Inspector ou `GetComponent`. |
| Vitesse du drone, durees de croissance, objectif de recolte reglables | :white_check_mark: OUI | `DroneMovement.speed`, `Crop.growthCooldown`/`matureCooldown`, `GameManager.harvestObjective` — tous exposes. |
| References audio et UI exposees | :white_check_mark: OUI | AudioClips exposes dans DropSeed, Watering, Harvesting, Crop. References UI (TextMeshPro, Image, panels) exposees dans les scripts UI. |
| Les champs exposes ont des noms clairs | :warning: QUASI | La majorite des noms sont clairs. Quelques exceptions : `OnGameEnd2`, `smoothTime` (utilise a la fois pour des animations et le suivi camera — ambigu selon le script). |

---

## 13. Concevoir une UI de score fonctionnelle et lisible

| Critere | Statut | Detail |
|---------|--------|--------|
| Le score est affiche en permanence | :white_check_mark: OUI | `ScoreText` dans le Canvas de la scene Game, abonne a `OnScoreChange`. |
| Le score augmente immediatement a chaque legume recolte | :white_check_mark: OUI | `Harvesting.HarvestCrop()` appelle `GameManager.ChangeScore()` puis `Harvest()`. L'evenement notifie l'UI. Animation LitMotion pour la transition de valeur. |
| Le score est lisible sur le decor | :warning: A VERIFIER | TextMeshPro utilise. **[A verifier visuellement — contraste, taille, position]** |
| La valeur affichee est toujours juste | :white_check_mark: OUI | L'UI est directement pilotee par les evenements du GameManager. |

---

## 14. Configurer le Canvas Scaler et les ancres

| Critere | Statut | Detail |
|---------|--------|--------|
| Un Canvas Scaler est configure | :white_check_mark: OUI | Scale With Screen Size, resolution de reference 800x600. |
| Les elements d'interface sont ancres | :warning: A VERIFIER | **[A verifier dans l'editeur — les ancres ne sont pas visibles depuis le code des scenes]** |
| L'interface reste a sa place en 16:9, large et 4:3 | :warning: ATTENTION | `MatchWidthOrHeight = 0` (match width uniquement). En 4:3 l'UI pourrait etre comprimee verticalement. **Un `MatchWidthOrHeight = 0.5` serait plus robuste.** |
| Aucun element ne sort de l'ecran | :warning: A VERIFIER | **[A verifier en testant differentes resolutions dans l'editeur]** |

---

## 15. Integrer des SFX et une musique d'ambiance

| Critere | Statut | Detail |
|---------|--------|--------|
| La plantation possede son propre son | :white_check_mark: OUI | `DropSeed.cs` — `AudioSource.PlayClipAtPoint(dropSound, ...)`. |
| L'arrosage possede son propre son | :white_check_mark: OUI | `Watering.cs` — `AudioSource.PlayClipAtPoint(wateringSound, ...)`. |
| La recolte possede son propre son | :warning: OUI MAIS | `Harvesting.cs` — `AudioSource.PlayClipAtPoint(harvestSound, ..., 100f)`. **Le volume est a 100f alors que la plage est 0-1.** Ce son va tres probablement saturer. A corriger en `1f`. |
| L'eclatement possede son propre son | :white_check_mark: OUI | `Crop.cs` — `AudioSource.PlayClipAtPoint(destroySound, ...)`. |
| Une musique d'ambiance tourne en boucle | :warning: PARTIEL | **Menu principal : OUI** — AudioSource en boucle sur le Drone du MainMenu. **Scene de jeu : NON** — aucun AudioSource de musique dans la scene Game. Le fichier BGM existe mais n'est pas utilise en jeu. |
| Aucun son ne sature ni ne couvre les autres | :x: PROBLEME | Le volume `100f` sur le son de recolte va certainement saturer. **Pas d'AudioMixer** pour equilibrer les niveaux. |

**A faire :**
1. Corriger le volume de Harvesting (`100f` -> `1f`).
2. Ajouter un AudioSource avec la musique d'ambiance dans la scene Game.
3. Envisager un AudioMixer pour equilibrer SFX / Musique / UI.

---

## 16. Placer des effets particules lisibles et correctement positionnes

| Critere | Statut | Detail |
|---------|--------|--------|
| Un jet de particules est visible sous le drone pendant l'arrosage | :white_check_mark: OUI | `WaterParticles.prefab` integre en nested dans le Drone. `Watering.cs` appelle `waterParticles.Play()`. |
| L'eclatement possede ses propres particules | :white_check_mark: OUI | `SmokePuffDestroy.prefab` instancie par `Crop.Update()` a la mort. |
| Chaque effet se declenche au bon moment | :white_check_mark: OUI | Arrosage declenche a l'action, eclatement a l'expiration du cooldown Dead, spawn a l'instanciation (nested dans les crop prefabs). |

---

## 17. Organiser le projet (nomenclature, commentaires, hierarchie)

| Critere | Statut | Detail |
|---------|--------|--------|
| Les dossiers sont ranges par type | :white_check_mark: OUI | Scripts/ (avec sous-dossiers Crop, Drone, UI, Utils), prefabs/ (Crops, UI, Utils, VFX), Import/ (3DModels, BGM, SFX, Skyboxes, Textures, UI), Materials/, etc. Quelques reliquats mal ranges a la racine d'Assets (terrainlayers, Handpainted_Grass). |
| Les noms sont explicites (pas de "GameObject (3)") | :warning: PARTIEL | **Pas de "GameObject (N)"** generique. Mais : `tree-pine-small(Clone) (1)`, `(2)`, `(3)` dans MainMenu (restes de clones places manuellement). `Mure Invisible (1)`, `(2)`, `(3)` dans Game. Melange francais/anglais (`Mure Invisible` vs `BackGround`, `Environement`). |
| La hierarchie de la scene est lisible et regroupee | :warning: QUASI | Les objets sont regroupes sous des parents (`Environement`, `Dirt`, `VFX`, `Props`). Quelques clones non renommes degradent la lisibilite. |
| Les scripts sont commentes quand c'est utile | :warning: PARTIEL | Quelques commentaires utiles (`//Planter la graine`), mais aussi des commentaires par defaut Unity non supprimes (`// Start is called once...`, `// Update is called once...`) et des commentaires vides ou evidents (`//sub to event here :`). |
| Les noms respectent une norme coherente | :warning: PARTIEL | PascalCase pour la majorite des methodes, mais `getHarvestObjective()` et `getCropScore()` en camelCase. `OnGameEnd2` est un nom obscur. Typos : `SceneControler`, `Letuce`, `Environement`, `TerraainTextures`, `AWSD` (au lieu de WASD). |

**A faire :**
1. Renommer les clones dans les scenes (donner des noms descriptifs).
2. Corriger les typos (`SceneController`, `Lettuce`, `Environment`, `TerrainTextures`, `WASD`).
3. Uniformiser le PascalCase sur toutes les methodes publiques.
4. Supprimer les commentaires par defaut Unity.
5. Supprimer `SceneFade.cs` (classe vide).

---

## 18. Respecter la boucle de gameplay (planter / arroser / recolter / score)

| Critere | Statut | Detail |
|---------|--------|--------|
| Planter fait apparaitre une graine | :white_check_mark: OUI | `DropSeed.PlantSeed()` instancie un Seed qui, a la collision avec Dirt, plante via `CropHolder.PlantSeed()`. |
| Arroser fait pousser plus vite | :white_check_mark: OUI | `Crop.watered = true` applique un `wateringMultiplier` au cooldown dans `Update()`. |
| Un legume mur peut etre recolte et fait monter le score | :white_check_mark: OUI | `Harvesting` verifie `CropState.Mature`, appelle `ChangeScore()` et `Harvest()`. |
| Un legume trop mur eclate | :white_check_mark: OUI | Quand `CropState.Dead` et cooldown expire : particules + destruction + score retire. |
| Le jeu possede une fin | :white_check_mark: OUI | Victoire a l'objectif de recolte (`harvestObjective`) + defaite au timer (`GameTimer`). |

---

## 19. Sauvegarder des donnees avec PlayerPrefs

| Critere | Statut | Detail |
|---------|--------|--------|
| Au moins une donnee est ecrite avec PlayerPrefs | :white_check_mark: OUI | `PlayerPrefs.SetInt("HighScore", highScore)` dans `GameManager.Save()`. |
| La donnee est relue apres fermeture complete du jeu | :white_check_mark: OUI | `GameManager.Load()` lit le high score au `Start()`. |
| Le meilleur score ne baisse jamais | :white_check_mark: OUI | `Win()` verifie `if (score > highScore)` avant de sauvegarder. |
| Au premier lancement, le meilleur score affiche 0 | :white_check_mark: OUI | `GetInt("HighScore", 0)` retourne 0 par defaut. |

---

## 20. Livrer un jeu stable sans bug majeur

| Critere | Statut | Detail |
|---------|--------|--------|
| Une partie se joue du menu a l'ecran de fin sans blocage | :warning: A VERIFIER | La logique semble correcte mais **[necessite un test en jeu]**. |
| Recommencer plusieurs fois de suite ne cree aucune erreur | :warning: A VERIFIER | `StartNewGame()` reinitialise et recharge la scene. Le GameManager persiste via `DontDestroyOnLoad`. **[A verifier que les evenements statiques ne conservent pas de listeners morts]** |
| La Console est vide d'erreurs | :warning: A VERIFIER | Risques identifies : references CropHolder stales, volume 100f sur le harvest sound. **[A verifier dans l'editeur]** |

---

## Validation finale

| Critere | Statut |
|---------|--------|
| Tous les criteres des 20 objectifs sont coches | :x: NON | Voir le recapitulatif ci-dessous. |
| Le jeu a ete teste sur une build | :x: NON | Pas de build existante. |
| La page itch.io et le lien sont verifies | :x: NON | Pas de page itch.io. |

---

## Recapitulatif

### Ce qui est fait et valide (14/20 objectifs complets ou quasi-complets)
- :white_check_mark: **Objectif 5** — Instanciation/destruction : tout bon.
- :white_check_mark: **Objectif 7** — Input System : complet et bien fait.
- :white_check_mark: **Objectif 9** — Time.deltaTime et Coroutines : complet.
- :white_check_mark: **Objectif 13** — UI de score : fonctionnelle.
- :white_check_mark: **Objectif 16** — Particules : presentes et bien declenchees.
- :white_check_mark: **Objectif 18** — Boucle de gameplay : complete.
- :white_check_mark: **Objectif 19** — PlayerPrefs : correct.
- :white_check_mark: **Objectif 4** — Prefabs : bien organises (Nested oui, Variant manquant).
- :white_check_mark: **Objectif 6** — Colliders/Triggers : fonctionnels.
- :white_check_mark: **Objectif 12** — Inspector : bien utilise.

### Ce qui est partiellement fait (necessite des corrections ciblees)
- :warning: **Objectif 3** — Navigation : **menu pause manquant**.
- :warning: **Objectif 8** — MonoBehaviour : quelques noms a corriger, SceneFade vide.
- :warning: **Objectif 10** — Animation : **un seul element anime (le drone)**, il en faut un second.
- :warning: **Objectif 11** — Assets externes : **musique d'ambiance absente en jeu**, reste a verifier visuellement.
- :warning: **Objectif 14** — Canvas Scaler : `MatchWidthOrHeight` a ajuster.
- :warning: **Objectif 15** — SFX : **volume 100f a corriger**, **musique absente en scene Game**, pas d'AudioMixer.
- :warning: **Objectif 17** — Organisation : typos, clones non renommes, commentaires par defaut.
- :warning: **Objectif 20** — Stabilite : **a tester en jeu**.

### Ce qui n'est pas fait
- :x: **Objectif 1** — Build itch.io.
- :x: **Objectif 2** — Page itch.io.

---

## Plan d'action par priorite

### Priorite 1 — Bloquant pour la livraison
1. **Menu pause** (Objectif 3) — Creer un script PauseMenu avec `Time.timeScale = 0/1`, touche Echap, options Reprendre/Recommencer/Quitter.
2. **Build WebGL ou Standalone** (Objectif 1) — Configurer et tester.
3. **Page itch.io** (Objectif 2) — Description, controles, screenshots, credits.

### Priorite 2 — Corrections rapides
4. **Volume Harvesting** — Changer `100f` en `1f` dans `Harvesting.cs`.
5. **Musique d'ambiance en jeu** — Ajouter un AudioSource avec le BGM dans la scene Game (ou sur un objet DontDestroyOnLoad).
6. **MatchWidthOrHeight** — Passer de `0` a `0.5` sur les Canvas Scaler.
7. **Typos** — `SceneControler` -> `SceneController`, `Letuce` -> `Lettuce`, etc.
8. **Supprimer SceneFade.cs** — Code mort.
9. **Renommer les clones** dans les scenes (tree-pine-small(Clone)(1), Mure Invisible(1), etc.).

### Priorite 3 — Completude de la checklist
10. **Deuxieme element anime** (Objectif 10) — Ajouter une animation sur un element de decor ou d'UI.
11. **Prefab Variant** (Objectif 4) — Creer un CropBase.prefab et deriver les 3 crops comme variantes.
12. **Nommage** — Uniformiser PascalCase, renommer `OnGameEnd2`, `ActiveMenu`, etc.
13. **Commentaires** — Supprimer les commentaires par defaut, ajouter un commentaire utile la ou c'est necessaire.
14. **Tester en build** (Objectif 20) — Jouer plusieurs parties, verifier la Console, tester le restart.
