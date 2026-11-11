# Code Review — SolarPunk

## 1. Resume

**Etat general du code : Correct, avec quelques problemes a corriger.**

Le projet est un jeu de drone/farming relativement petit (~18 scripts dont ~16 actifs) avec une architecture simple et lisible. Le code est globalement comprehensible et les responsabilites sont raisonnablement reparties entre les classes. L'utilisation de LitMotion pour les animations, des events statiques pour le decouplage UI, et du ScriptableObject pour les donnees de cultures montre de bonnes intuitions de conception.

**Points positifs :**
- Bonne utilisation des evenements statiques pour decoupler le GameManager de l'UI.
- Abonnements/desabonnements corrects dans `OnEnable`/`OnDisable` dans la majorite des classes.
- Utilisation pertinente des ScriptableObjects pour les types de cultures.
- Le systeme de seed/crop/harvest est clair et facile a suivre.

**Principaux problemes a corriger :**
- `CameraFollow.Update()` cree un nouveau tween LitMotion **a chaque frame**, ce qui est un probleme de performance et de comportement.
- `Crop.Update()` ne sort pas de la methode apres avoir detruit l'objet en etat `Dead`, executant du code inutile sur un objet condamne.
- `SceneControler.FadeOut()` ignore le parametre `duration` dans le calcul de l'alpha.
- `GameManager.Load()` compare un `int` a `null`, ce qui ne fonctionne pas en C#.

---

## 2. Problemes a corriger

### Critique

#### 2.1 `CameraFollow.Update()` — Un nouveau tween cree a chaque frame

- **Fichier :** `Assets/Scripts/CameraFollow.cs`, methode `Update()`, lignes 17-20
- **Constat :** A chaque appel d'`Update()` (potentiellement 60+ fois par seconde), un nouveau tween LitMotion est cree sans annuler le precedent. Cela signifie que des dizaines de tweens actifs s'accumulent et se battent simultanement pour deplacer la camera.
- **Pourquoi :** Performance degradee (allocations massives chaque frame) et comportement imprevisible de la camera (plusieurs tweens controlent la meme propriete en meme temps).
- **Correction suggeree :** Soit annuler le tween precedent avant d'en creer un nouveau, soit ne pas utiliser de tween du tout et utiliser `Vector3.Lerp` ou `Vector3.SmoothDamp` directement dans `Update`.
- **Certitude :** Certain.
- **Exemple :**
```csharp
void Update()
{
    var targetPosition = target.position + offset;
    transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.deltaTime);
}
```
Ou avec conservation du tween et annulation :
```csharp
private MotionHandle currentMotion;

void Update()
{
    var targetPosition = target.position + offset;
    if (currentMotion.IsActive()) currentMotion.Cancel();
    currentMotion = LMotion.Create(transform.position, targetPosition, smoothTime)
        .WithEase(easeType)
        .BindToPosition(transform)
        .AddTo(gameObject);
}
```
> Note : la seconde approche reste couteuse (creation d'un tween chaque frame). `Vector3.Lerp` ou `SmoothDamp` sont bien plus adaptes a un suivi de camera continu.

---

#### 2.2 `Crop.Update()` — Code execute apres destruction en etat Dead

- **Fichier :** `Assets/Scripts/Crop.cs`, methode `Update()`, lignes 62-91
- **Constat :** Quand `currentCooldown <= 0` et `currentState == CropState.Dead`, le code appelle `Destroy(this.gameObject)` mais ne fait pas de `return`. L'execution continue : `currentCooldown` est reinitialise et `ScaleMultiply()` est appele sur un objet en cours de destruction.
- **Pourquoi :** `Destroy` est differe a la fin de la frame, donc le code suivant s'execute. Cela provoque un appel inutile a `ScaleMultiply` (qui cree un tween LitMotion) sur un objet qui va etre detruit. Risque d'erreurs ou de comportements inattendus.
- **Correction suggeree :** Ajouter un `return` apres le bloc de destruction.
- **Certitude :** Certain.
- **Exemple :**
```csharp
if (currentCooldown <= 0)
{
    if (currentState == CropState.Dead)
    {
        GameManager.Instance.ChangeScore(-scoreRemoved);
        Instantiate(destroyParticle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        return; // <-- manquant
    }
    // ...reste du code
}
```

---

#### 2.3 `SceneControler.FadeOut()` — Le parametre `duration` est ignore dans le calcul

- **Fichier :** `Assets/Scripts/SceneControler.cs`, methode `FadeOut()`, ligne 58
- **Constat :** La ligne `c.a = 1f - (t / 1);` divise `t` par `1` au lieu de `duration`. La boucle `while` utilise bien `duration` pour sa condition, mais l'alpha atteint 0 a `t = 1` seconde peu importe la duree specifiee.
- **Pourquoi :** Si `duration > 1`, l'alpha devient negatif apres 1 seconde (probablement clamp par Unity, mais le timing est faux). Si `duration < 1`, le fade ne se termine pas visuellement avant la fin de la boucle.
- **Correction suggeree :** Remplacer `1` par `duration`.
- **Certitude :** Certain.
- **Exemple :**
```csharp
c.a = 1f - (t / duration);
```

---

### Important

#### 2.4 `GameManager.Load()` — Comparaison d'un `int` avec `null`

- **Fichier :** `Assets/Scripts/GameManager.cs`, methode `Load()`, ligne 112
- **Constat :** `int highScoreSaved = PlayerPrefs.GetInt("HighScore");` retourne un `int` (type valeur). La comparaison `if (highScoreSaved == null)` n'a aucun sens en C# : un `int` ne peut jamais etre `null`. Cette condition est toujours `false`.
- **Pourquoi :** Le code fonctionne par accident (quand la cle n'existe pas, `GetInt` retourne 0, et le bloc `else` assigne 0 a `highScore`). Mais cela genere un warning du compilateur et trahit une incomprehension du type. Si un jour la logique change, ce bug dormant pourrait poser probleme.
- **Correction suggeree :** Utiliser `PlayerPrefs.HasKey()` ou simplement assigner directement.
- **Certitude :** Certain.
- **Exemple :**
```csharp
void Load()
{
    highScore = PlayerPrefs.GetInt("HighScore", 0);
}
```
> `GetInt` avec une valeur par defaut est la facon idiomatique de gerer ce cas. Toute la methode peut tenir en une ligne.

---

#### 2.5 `GameManager.ChangeScore()` — Effet de bord dans une condition

- **Fichier :** `Assets/Scripts/GameManager.cs`, methode `ChangeScore()`, ligne 46
- **Constat :** `if((score += amount) < 0)` modifie `score` a l'interieur de la condition. Si le resultat est negatif, `score` est d'abord mis a la valeur negative, puis remis a 0.
- **Pourquoi :** Le code fonctionne mais est difficile a lire. Un developpeur qui passe rapidement pourrait ne pas voir que `score` est modifie dans le `if`. C'est un pattern error-prone.
- **Correction suggeree :** Separer la modification de la condition.
- **Certitude :** Certain (probleme de lisibilite, pas de bug fonctionnel).
- **Exemple :**
```csharp
public void ChangeScore(int amount)
{
    score = Mathf.Max(0, score + amount);
    OnScoreChange?.Invoke(score);
}
```

---

#### 2.6 `ScoreText` et `HarvestText` — Tweens non annules avant d'en creer de nouveaux

- **Fichiers :** `Assets/Scripts/ScoreText.cs` ligne 32, `Assets/Scripts/HarvestText.cs` ligne 38
- **Constat :** A chaque changement de score ou de recolte, un nouveau tween LitMotion est cree sans annuler le precedent. Si des changements rapides arrivent (par exemple, recolte de plusieurs cultures en peu de temps), plusieurs tweens sont actifs simultanement sur le meme texte.
- **Pourquoi :** Les tweens se chevauchent et le texte peut osciller ou afficher des valeurs incoherentes. `CropSelectionUI` gere deja ce cas correctement avec un `MotionHandle` — le meme pattern devrait etre applique ici.
- **Correction suggeree :** Stocker le `MotionHandle` et annuler le tween actif avant d'en creer un nouveau, comme dans `CropSelectionUI`.
- **Certitude :** Probable (depend de la frequence des changements de score en jeu).

---

#### 2.7 `CropHolder` — References non nettoyees quand une culture se detruit elle-meme

- **Fichier :** `Assets/Scripts/CropHolder.cs`, champs `currentCrop` et `currentCropGameObject`
- **Constat :** Quand une `Crop` meurt (etat `Dead`), elle appelle `Destroy(this.gameObject)` depuis `Crop.Update()`. Le `CropHolder` qui reference cette culture ne nettoie jamais ses champs `currentCrop` et `currentCropGameObject`.
- **Pourquoi :** En Unity, une reference a un objet detruit retourne `true` pour `== null` grace a l'operateur surcharge. Donc `PlantSeed()` fonctionnera probablement apres la mort d'une culture (on pourra replanter). Mais c'est fragile : ce comportement est un detail d'implementation d'Unity, et des verifications comme `cropHolder.currentCrop != null` dans d'autres scripts (Harvesting, Watering) reposent sur cette subtilite. Si quelqu'un utilise le pattern `is null` (qui bypass l'operateur surcharge d'Unity), le code casse.
- **Correction suggeree :** Faire en sorte que le `CropHolder` soit notifie quand sa culture est detruite, ou que la `Crop` nettoie les references du `CropHolder` avant de se detruire.
- **Certitude :** Probable (fonctionne actuellement par accident grace au null override d'Unity).

---

#### 2.8 `DroneMovement.Update()` — Slerp frame-rate dependant

- **Fichier :** `Assets/Scripts/DroneMovement.cs`, ligne 28
- **Constat :** `Quaternion.Slerp(visualTransform.localRotation, target, tiltSmoothing)` utilise une valeur fixe (`0.05f`) comme facteur d'interpolation. Ce pattern de "lerp vers la cible" est frame-rate dependant : a 120 FPS, le drone basculera environ 2 fois plus vite qu'a 60 FPS.
- **Pourquoi :** Comportement visuellement different selon les machines. Sur un PC puissant, le tilt sera plus nerveux ; sur un PC lent, plus mou.
- **Correction suggeree :** Utiliser `Time.deltaTime` pour rendre le mouvement independant du framerate.
- **Certitude :** Certain.
- **Exemple :**
```csharp
float t = 1f - Mathf.Pow(1f - tiltSmoothing, Time.deltaTime * 60f);
visualTransform.localRotation = Quaternion.Slerp(visualTransform.localRotation, target, t);
```
> Ou plus simplement, augmenter `tiltSmoothing` et multiplier par `Time.deltaTime` : l'important est que le comportement soit lie au temps reel, pas au nombre de frames.

---

### Mineur

#### 2.9 `DropSeed.OnDisable()` — Invocation inutile de l'evenement

- **Fichier :** `Assets/Scripts/DropSeed.cs`, methode `OnDisable()`, ligne 40
- **Constat :** `OnCropChange?.Invoke(crops[currentCropId].cropSprite)` est appele dans `OnDisable`. L'evenement de changement de culture est emis quand le composant se desactive.
- **Pourquoi :** Cela semble involontaire. Les abonnes restants pourraient recevoir une notification inattendue au moment ou le composant se desactive (par exemple, lors d'un changement de scene).
- **Correction suggeree :** Supprimer cette ligne de `OnDisable` si elle n'est pas intentionnelle.
- **Certitude :** Probable (a verifier si c'est intentionnel).

---

#### 2.10 `Harvesting.cropHolder` — `[SerializeField]` trompeur

- **Fichier :** `Assets/Scripts/Harvesting.cs`, ligne 6
- **Constat :** `[SerializeField] CropHolder cropHolder` est marque comme serialise, mais la valeur est ecrasee a chaque `OnTriggerEnter`/`OnTriggerExit`.
- **Pourquoi :** Un autre developpeur pourrait penser que la valeur assignee dans l'Inspector est utilisee, alors qu'elle est immediatement remplacee au runtime. C'est trompeur.
- **Correction suggeree :** Retirer `[SerializeField]` et declarer le champ `private CropHolder cropHolder;` tout court (comme dans `Watering.cs` qui fait la meme chose correctement).
- **Certitude :** Certain.

---

#### 2.11 `SceneFade.cs` — Classe vide

- **Fichier :** `Assets/Scripts/SceneFade.cs`
- **Constat :** La classe ne contient rien d'autre que des methodes `Start()` et `Update()` vides avec les commentaires par defaut d'Unity.
- **Pourquoi :** Code mort. Ajoute du bruit dans le projet.
- **Correction suggeree :** Supprimer le fichier s'il n'est plus necessaire, ou l'implementer s'il est prevu.
- **Certitude :** Certain.

---

#### 2.12 Convention de nommage inconsistante

- **Fichiers :** `Assets/Scripts/GameManager.cs`
- **Constat :** `getHarvestObjective()` utilise le camelCase (convention Java), alors que `GetScore()` et les autres methodes utilisent le PascalCase (convention C#). De meme, `getCropScore()` dans `Crop.cs`.
- **Pourquoi :** Inconsistance mineure mais facile a corriger. Les conventions C# et Unity utilisent le PascalCase pour les methodes publiques.
- **Correction suggeree :** Renommer en `GetHarvestObjective()` et `GetCropScore()`.
- **Certitude :** Certain.

---

## 3. Ameliorations simples recommandees

1. **Encapsulation des champs publics :** Beaucoup de champs dans `Crop`, `CropHolder`, `DropSeed`, `OnButtonHover`, `HarvestText`, `WinLoseUI` sont `public` alors qu'ils ne sont utilises que par Unity pour la serialisation. Les passer en `[SerializeField] private` protege l'encapsulation et evite les modifications accidentelles depuis d'autres scripts. Champs prioritaires : `Crop.watered`, `Crop.currentState`, `CropHolder.currentCrop`, `CropHolder.currentCropGameObject`.

2. **`GameManager.highScore` devrait etre prive :** Actuellement `public`, il est lu directement par `HighScoreText`. Un simple getter suffirait, et cela eviterait qu'un autre script modifie le high score par accident.

3. **Simplifier `GameManager.Load()` :** Toute la methode peut etre remplacee par `highScore = PlayerPrefs.GetInt("HighScore", 0);`.

4. **Sauvegarder aussi en cas de Game Over :** `GameManager.GameOver()` ne verifie pas et ne sauvegarde pas le high score. Si le joueur fait un meilleur score mais perd (si ce scenario est possible), le high score n'est pas mis a jour.

5. **`GameManager.Save()` — Appeler `PlayerPrefs.Save()` :** `PlayerPrefs.SetInt` ecrit en memoire mais ne garantit pas la persistance sur disque. Ajouter `PlayerPrefs.Save()` apres le `SetInt` assure que les donnees sont ecrites, notamment en cas de crash.

6. **`CropSelectionUI.UpdateUI()` — `cropImage.transform` au lieu de `GetComponent<Transform>()` :** Ligne 35, `cropImage.GetComponent<Transform>()` est inutile car tout `MonoBehaviour` a deja une propriete `.transform` directement accessible.

---

## 4. Points a surveiller

1. **Ordre d'execution `Start()` :** `SceneControler.Start()` fait `GameManager.Instance.sceneControler = this`. Si `GameManager.Awake()` n'a pas encore ete appele (ce qui serait rare mais possible selon la configuration des scenes), `Instance` sera `null`. **A verifier** : que le GameManager est bien present dans la premiere scene chargee et que son `Awake` s'execute avant tout `Start`.

2. **Destruction des particules :** `Crop.Update()` appelle `Instantiate(destroyParticle, ...)` quand la culture meurt, mais rien dans le code visible ne detruit cet objet particule. **A verifier** : que le prefab de particule se detruit lui-meme (par exemple, via un script ou une option "Stop Action: Destroy" sur le ParticleSystem).

3. **`OnButtonHover` — `Camera.main` :** `Camera.main` fait un `FindGameObjectWithTag("MainCamera")` en interne dans les anciennes versions d'Unity. Depuis Unity 2020+, c'est cache. Si le projet cible une version recente, ce n'est pas un probleme. **A verifier** la version d'Unity utilisee.

4. **Changement de scene et evenements statiques :** Les evenements statiques (`OnScoreChange`, `OnHarvestChange`, `OnGameEnd`, `OnCropChange`) persistent entre les scenes car ils sont `static`. Si un objet s'abonne dans `OnEnable` mais est detruit par un changement de scene sans passer par `OnDisable`, l'evenement gardera une reference a un objet detruit, ce qui causera une exception au prochain `Invoke`. **A verifier** : que tous les abonnes sont bien detruits proprement (ce qui semble etre le cas avec le pattern `OnEnable`/`OnDisable`).

5. **`GameManager.StartNewGame()` ne reinvoque pas les evenements :** Apres avoir mis `score = 0` et `amountHarvested = 0`, l'UI n'est pas notifiee. Cela fonctionne si une nouvelle scene est chargee (les UI sont recreees), mais si `StartNewGame` etait appele sans recharger la scene, l'affichage serait desynchronise.

6. **`Crop` — comparaison de Vector3 avec `!=` :** Ligne 97, `transform.localScale != maxScale` compare des `Vector3` avec l'operateur `!=`, qui utilise une tolerance par defaut. Cela fonctionne generalement, mais avec des tweens LitMotion, la valeur finale pourrait avoir une precision flottante legerement differente. **A verifier** si la crop peut rester bloquee dans un etat ou elle n'atteint jamais exactement `maxScale`.

---

## 5. Ce qui est deja bien

- **Pattern evenementiel clair :** L'utilisation d'`event Action<>` statiques dans le `GameManager` et `DropSeed` pour communiquer avec l'UI est un bon pattern. L'UI s'abonne et se desabonne proprement, et le GameManager ne connait aucune classe UI. C'est exactement le bon niveau de decouplage pour un projet de cette taille.

- **Abonnements/desabonnements corrects :** La quasi-totalite des classes qui s'abonnent a des evenements le font dans `OnEnable` et se desabonnent dans `OnDisable`. C'est la bonne pratique Unity, bien appliquee.

- **Utilisation des ScriptableObjects :** `CropSO` pour definir les types de cultures (nom, sprite, prefab) est un bon choix. Cela permet d'ajouter de nouvelles cultures sans toucher au code.

- **Singleton GameManager avec protection :** Le pattern singleton avec verification `if (instance != null && instance != this)` et `DontDestroyOnLoad` est correctement implemente.

- **Separation des responsabilites du drone :** `Watering`, `Harvesting`, `DropSeed` et `DroneMovement` sont des composants separes, chacun gerant une action. C'est plus facile a maintenir que d'avoir tout dans une seule classe "DroneController".

- **Systeme de culture lisible :** Le cycle de vie d'une culture (semence -> jeune -> mature -> morte) est facile a suivre. Les etats sont clairs avec l'enum `CropState`.

---

## 6. Plan d'action conseille

Par ordre de priorite (rapport valeur/effort) :

1. **Corriger `CameraFollow.Update()`** — Remplacer le tween par un `Vector3.Lerp` ou `SmoothDamp`. C'est le probleme le plus impactant (performance + comportement incorrect).

2. **Ajouter `return` dans `Crop.Update()` apres la destruction** — Correction d'une ligne, evite l'execution de code sur un objet mort.

3. **Corriger `SceneControler.FadeOut()`** — Remplacer `t / 1` par `t / duration`. Bug simple mais visible par le joueur.

4. **Simplifier `GameManager.Load()`** — Remplacer par `highScore = PlayerPrefs.GetInt("HighScore", 0)`. Elimine un bug logique et un warning compilateur.

5. **Corriger `DroneMovement` tilt frame-rate dependant** — Integrer `Time.deltaTime` dans le Slerp.

6. **Ajouter la gestion des `MotionHandle` dans `ScoreText` et `HarvestText`** — Copier le pattern deja present dans `CropSelectionUI`.

7. **Nettoyer les champs publics** — Passer les champs serialises en `[SerializeField] private` la ou c'est pertinent, en commencant par `Crop.watered` et `Crop.currentState` qui sont manipules par d'autres scripts.

8. **Supprimer `SceneFade.cs`** et corriger les nommages (`getHarvestObjective` -> `GetHarvestObjective`).

---

## 7. Verdict

**Est-ce que le code est suffisamment propre pour continuer le developpement ?**
Oui. Le code est lisible, les responsabilites sont bien separees pour un projet de cette taille, et la base est saine. Les problemes identifies sont des corrections ponctuelles, pas des symptomes d'une architecture bancale.

**Y a-t-il des problemes qui risquent de couter cher plus tard ?**
Le probleme le plus risque est la gestion des references `CropHolder`/`Crop` (point 2.7) : si le projet grandit et que d'autres systemes interagissent avec les cultures, s'appuyer sur le null override d'Unity pour detecter les cultures mortes deviendra fragile. Ajouter un mecanisme explicite (callback ou nettoyage) quand une culture se detruit serait un bon investissement.

**Les 3 ameliorations qui valent le plus le coup :**
1. **Corriger `CameraFollow`** — gain immediat en performance et en comportement correct.
2. **Ajouter le `return` manquant dans `Crop.Update()`** — correction d'une seconde qui elimine un bug reel.
3. **Corriger le FadeOut** — bug visible par le joueur, correction triviale.

---

*Review realisee sur les 16 scripts actifs dans `Assets/Scripts/`. Les prefabs, les scenes, la configuration de l'Input System et les ScriptableObjects en place n'ont pas ete examines. Certaines remarques dependent du contexte Unity (ordre d'execution, configuration des composants) et sont signalees comme telles.*
