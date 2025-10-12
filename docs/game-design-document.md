# Game Design Document - Goblin Thief

## 🎮 Vue d'ensemble

**Thème:** "Le plus tu en as, le pire c'est"

**Genre:** Action/Aventure 2D avec mécaniques de survie

**Description:** Incarnez un voleur qui pille les trésors des tribus de gobelins. Plus vous amassez d'argent, plus votre sac se remplit et alourdit vos mouvements. Trouvez l'équilibre entre cupidité et survie!

---

## 🎯 Lien avec le thème: "Le plus tu en as, le pire c'est"

Le jeu explore brillamment le thème de la game jam à travers plusieurs mécaniques interconnectées:

### 1. **Système de Vitesse Dynamique**
Plus le joueur accumule d'argent (perles), plus il ralentit. La formule utilisée dans `PlayerComponent.cs:192`:
```csharp
speed = (Mathf.Pow(bag.GetMoney() + 3, -1) * 15) + 0.7f;
```

- **1 perle:** Vitesse maximale (~5.7 unités/sec)
- **10 perles:** Vitesse moyenne (~2.9 unités/sec)
- **40+ perles:** Vitesse très lente, mouvement par saccades

### 2. **Croissance Visuelle du Sac**
Le sac grossit logarithmiquement avec l'argent accumulé (`BagComponent.cs:28`):
```csharp
gameObject.transform.localScale = Vector3.one * (Mathf.Log(money * 2) + 0.7f);
```
Un sac plus gros rend le joueur plus visible et plus vulnérable aux gobelins.

### 3. **Délai d'Attaque Logarithmique**
Plus le joueur est riche, plus son attaque est lente (`PlayerComponent.cs:147`):
```csharp
yield return new WaitForSeconds(Mathf.Log((bag.GetMoney() + 3) * 0.3f));
```
La cupidité ralentit votre capacité à vous défendre!

### 4. **Mouvement Contraint au-delà de 40 Perles**
À 40+ perles, le joueur ne peut plus se déplacer librement et est limité à des mouvements sporadiques de 1 seconde toutes les 2 secondes (`PlayerComponent.cs:56-83`).

### 5. **Système de Vie = Argent**
La vie du joueur EST son argent. Quand un gobelin vous frappe:
- Vous perdez 1 perle
- Si vous atteignez 0 perles, c'est Game Over

Cela crée un dilemme constant: accumuler plus d'argent vous ralentit mais augmente votre survie!

---

## 🎲 Mécaniques de Gameplay

### Système de Mouvement
- **Déplacement 4-directions** (Haut, Bas, Gauche, Droite)
- **Vitesse variable** basée sur l'argent possédé
- **Caméra suivant le joueur** centrée sur la position
- **Contrainte physique:** Le joueur est ralenti par la distance entre lui et son sac

### Système de Combat
- **Attaque au corps-à-corps** avec cooldown de 2 secondes
- **Zone d'attaque** activée temporairement (BoxCollider2D)
- **Feedback visuel:** Animation de slash
- **Délai avant attaque** qui augmente avec l'argent
- **i-Frames (invincibilité temporaire):** 2 secondes après avoir été touché avec clignotement rouge

### Système de Collecte (Looting)
- **Coffres interactifs** avec détection de proximité
- **Système de progression:** Barre de chargement pour piller
- **Contenu variable:** Chaque coffre contient 5 perles
- **Interruption possible:** Le looting s'arrête si le joueur s'éloigne
- **Events:** Publication d'événements `Looting`, `Looted`, `LootingInterupted`

### Intelligence Artificielle - Gobelins
Les gobelins ont **4 états comportementaux** sophistiqués:

#### 1. **État Idle (Repos)**
- Reste sur place pendant 2 secondes
- Vitesse = 0
- Prépare le prochain mouvement de vagabondage

#### 2. **État Wandering (Vagabondage)**
- Se déplace aléatoirement entre 3-5 unités
- Reste à maximum 5 unités du point de spawn (campfire)
- Vitesse de marche: 2 unités/sec
- Évite le centre du campfire (zone de 1 unité)
- Distance minimale entre gobelins: 1.5 unités

#### 3. **État Chasing Player (Poursuite)**
- Activé quand le joueur entre dans la zone de détection
- **Point d'exclamation rouge** apparaît au-dessus du gobelin (SupriseWarning)
- Vitesse de course: 4 unités/sec
- Mémorise la dernière position connue du joueur
- Accélération et décélération fluides

#### 4. **État Going to Last Known Position**
- Après que le joueur quitte la zone
- Se dirige vers la dernière position connue
- Vitesse de marche: 2 unités/sec
- Retourne à l'état Idle une fois la position atteinte

**Systèmes d'Animation:**
- Vitesse d'animation synchronisée avec la vitesse de déplacement
- Animations fluides avec Lerp pour les transitions

### Système de Tribus
- **Spawning procédural:** 3-8 gobelins par tribu
- **Campfire central** avec animation
- **Positionnement aléatoire** autour du feu de camp (rayon de 5 unités)
- **Système de validation** pour éviter les chevauchements
- **Gestion des membres:** Nettoyage automatique des gobelins morts

### Système de Maisons
- **Détection d'entrée/sortie** du joueur
- **Fade du toit:** Transparence progressive quand le joueur entre
  - Durée de fade: 0.5 secondes
  - Alpha: 1.0 (opaque) → 0.0 (transparent)
- **Smooth Damp** pour des transitions organiques
- Permet de voir le joueur à l'intérieur

### Système de Perles (Loot Drops)
- **Object Pooling** pour optimiser les performances
- **Spawn après mort** d'un gobelin
- **Collecte automatique** au contact du joueur ou du sac
- **Feedback:** Son + augmentation du score

---

## 🔊 Système Audio

Le jeu utilise un **Event Bus** pour gérer les sons de manière découplée:

### Sons Implémentés
- **goblin-death.mp3:** Mort d'un gobelin
- **hurt.mp3:** Dégâts reçus par le joueur
- **money.mp3:** Collecte d'argent/perles
- **music.mp3:** Musique de fond (en boucle)
- **slashSound.mp3:** Attaque du joueur
- **spotted.mp3:** Gobelin détecte le joueur

### Architecture Audio
- **RecycleAudio Component:** Recyclage automatique des AudioClips
- **SoundManager Singleton:** Gestion centralisée
- **Object Pooling:** Les clips audio sont réutilisés

---

## 🏗️ Architecture Technique

### Patterns de Design Utilisés

#### 1. **Event Bus Pattern**
Système de communication découplé entre composants:
```csharp
GameEventsBus.Instance.Publish(new PlayerDamaged());
GameEventsBus.Instance.Subscribe<Looted>((l) => { MoneyUp(1); });
```

**Événements du jeu:**
- `GameStarted`: Début de partie
- `GameOver`: Fin de partie
- `Looting`: Début du pillage
- `Looted`: Pillage complété
- `LootingInterupted`: Pillage interrompu
- `MoneyGained`: Argent gagné
- `PlayerDamaged`: Joueur blessé
- `PlayerSlash`: Attaque du joueur
- `GoblinDeath`: Mort d'un gobelin
- `GoblinSurprise`: Gobelin détecte le joueur
- `ReplayMusic`: Rejouer la musique

#### 2. **Object Pooling Pattern**
Optimisation mémoire pour les perles réutilisables:
- Interface `IPoolable`
- `ObjectPoolComponent` pour la gestion
- Réduction de la charge du Garbage Collector

#### 3. **Component-Based Architecture**
Séparation des responsabilités:
- `PlayerComponent`: Logique du joueur
- `BagComponent`: Gestion de l'argent
- `PlayerSpriteComponent`: Animations
- `PlayerCollisions`: Détection de collisions

#### 4. **Single Responsibility Principle (SRP)**
Chaque classe a une responsabilité unique:
- `LootingController`: Progression du pillage
- `LootingInputHandler`: Gestion des inputs
- `LootingUIController`: Affichage de l'interface
- `PlayerDetector`: Détection du joueur

#### 5. **Observer Pattern**
Système d'événements pour les détections:
```csharp
playerDetector.OnPlayerEnter += OnPlayerEnter;
playerDetector.OnPlayerExit += OnPlayerExit;
```

---

## 📊 Paramètres de Gameplay

### Joueur
- **Vitesse initiale:** ~5.7 unités/sec (avec 1 perle)
- **Vitesse minimale:** ~0.7 unités/sec (avec beaucoup de perles)
- **Cooldown d'attaque:** 2 secondes
- **Durée des i-Frames:** 2 secondes
- **Vitesse de clignotement:** 0.2 secondes
- **Seuil de mouvement contraint:** 40 perles
- **Vitesse en mouvement contraint:** 0.7 unités/sec pendant 1 sec/2 sec

### Gobelins
- **Vitesse de marche:** 2 unités/sec
- **Vitesse de course:** 4 unités/sec
- **Temps d'idle:** 2 secondes
- **Distance de vagabondage:** 3-5 unités
- **Distance max du spawn:** 5 unités
- **Distance d'arrêt:** 0.5 unités
- **Taux d'accélération (marche):** 1
- **Taux de décélération (marche):** 1.5
- **Taux d'accélération (course):** 5
- **Taux de décélération (course):** 3

### Coffres
- **Vitesse de pillage:** 1 seconde par perle
- **Quantité par coffre:** 5 perles

### Tribus
- **Membres par tribu:** 3-8 gobelins
- **Rayon de spawn:** 5 unités
- **Distance minimale entre gobelins:** 1.5 unités
- **Distance minimale du campfire:** 1 unité

### Maisons
- **Durée de fade du toit:** 0.5 secondes
- **Alpha opaque:** 1.0
- **Alpha transparent:** 0.0

---

## 🎨 Composants Visuels

### Sprites & Animations
- **Player.aseprite / PlayerHit.aseprite:** Personnage principal
  - Animation: Idle
  - Animation: Walk
  - Animation: Idle Hit (pendant i-Frames)
  - Animation: Walk Hit (pendant i-Frames)

- **Goblin.aseprite:** Ennemis
  - Animation: Idle
  - Animation: Walk

- **Chest.aseprite:** Coffres à piller

- **Campfire.aseprite:** Feu de camp animé

- **SupriseWarning.aseprite:** Point d'exclamation pour gobelins

- **TileSet_V2.png:** Tileset pour l'environnement (131 tiles)

- **Floors.aseprite / Roof.aseprite:** Éléments de maisons

### Système de Rendu
- **Universal Render Pipeline (URP) 2D**
- **Post-processing:** Volume Profile pour effets visuels
- **Sorting Layers:** Gestion de la profondeur

---

## 🎮 Contrôles

### Input System (Unity New Input System)
- **Déplacement:** WASD ou Flèches directionnelles
- **Attaque:** Espace ou bouton d'action
- **Lâcher de l'argent:** Touche spécifique (pour réduire le poids)
- **Interaction (Looting):** Automatique au contact des coffres

---

## 🔄 Boucle de Gameplay

1. **Exploration:** Le joueur explore la carte à la recherche de coffres
2. **Détection:** Éviter les patrouilles de gobelins ou les zones de tribus
3. **Pillage:** S'approcher d'un coffre et maintenir la position pour piller
4. **Dilemme:** Continuer à accumuler ou déposer de l'argent?
5. **Combat:** Affronter les gobelins qui vous détectent (ralenti par l'argent)
6. **Fuite:** Échapper aux gobelins en colère (plus difficile avec un sac lourd)
7. **Survie:** Garder au moins 1 perle pour ne pas mourir
8. **Stratégie:** Utiliser les maisons pour se cacher (toit transparent)

---

## 🎯 Objectifs du Joueur

### Objectif Principal
Survivre le plus longtemps possible en accumulant des richesses tout en évitant les gobelins.

### Objectifs Secondaires
- Maximiser le score (nombre de perles collectées)
- Tuer des gobelins pour récupérer leurs perles
- Piller tous les coffres de la carte
- Maîtriser l'équilibre entre cupidité et mobilité

---

## 💡 Stratégies de Gameplay

### Pour les Débutants
1. Ne collectez que 10-15 perles pour rester mobile
2. Évitez les zones de tribus autant que possible
3. Utilisez les maisons comme refuges temporaires
4. Lâchez de l'argent si vous êtes poursuivi

### Pour les Joueurs Avancés
1. Mémorisez les patrouilles des gobelins
2. Optimisez les routes de pillage
3. Utilisez les attaques tactiques pour créer des perles supplémentaires
4. Maîtrisez le mouvement avec 40+ perles pour le défi ultime

---

## 🏆 Points Forts du Design

### 1. **Thème Parfaitement Intégré**
Chaque mécanique renforce le concept "Le plus tu en as, le pire c'est". Ce n'est pas un thème superficiel, mais le coeur du gameplay.

### 2. **Dilemme Constant**
Le joueur fait constamment face à des choix difficiles entre sécurité et cupidité.

### 3. **Feedback Visuel Clair**
- Le sac grossit visuellement
- Le joueur ralentit physiquement
- Les animations montrent clairement l'état du jeu

### 4. **Courbe d'Apprentissage Progressive**
- Début facile avec peu d'argent
- Difficulté croissante naturelle
- Le joueur comprend les conséquences de ses choix

### 5. **Architecture Technique Solide**
- Code modulaire et maintenable
- Patterns de design appropriés
- Commentaires et documentation SOLID

---

## 🔧 Technologies Utilisées

- **Unity 2023+**
- **Universal Render Pipeline (URP) 2D**
- **New Input System**
- **TextMesh Pro**
- **Aseprite** pour les sprites
- **C#** pour le scripting

---

## 📝 Crédits & Notes

**Game Jam:** Ludum Dare 25 (ou similaire)
**Thème:** "Le plus tu en as, le pire c'est"
**Team:** Touski

### Fichiers Clés à Examiner
- `PlayerComponent.cs` - Logique centrale du joueur
- `BagComponent.cs` - Système d'argent et progression
- `GoblinMovement.cs` - IA ennemie sophistiquée
- `GameEventsBus.cs` - Communication entre systèmes
- `LootingController.cs` - Mécanique de pillage

---

## 🚀 Améliorations Potentielles

### Gameplay
- Ajouter un système de sauvegarde de l'argent (banque/coffre sécurisé)
- Power-ups temporaires (speed boost, invincibilité)
- Différents types de trésors avec valeurs variables
- Boss gobelins avec comportements uniques
- Système de progression/upgrades permanents

### Technique
- Ajout de particules pour les effets visuels
- Système de dialogues/tutoriel
- Écran de Game Over avec statistiques
- Système de highscores
- Options audio/vidéo

### Polish
- Plus d'animations de transition
- Screenshake lors des impacts
- Trails/motion blur pour le mouvement rapide
- Environnements plus variés
- Cycle jour/nuit

---

**Note:** Cette documentation a été générée par analyse du code source du jeu. Toutes les mécaniques et valeurs proviennent directement de l'implémentation.
