# Progress: Core Gameplay Loop

## Status Table

| # | Script(s) | Status | Commit |
|---|-----------|--------|--------|
| 1 | Utils: ObjectPool, StateMachine, IDamageable, SafeAreaPanel, ButtonFeedback | ✅ done | `210060a` |
| 2 | Core: GameState, EnemyTypes enums | ✅ done | `a4c87ef` |
| 3 | Core: GameManager, ScreenManager | ✅ done | `a4c87ef` |
| 4 | Data: All ScriptableObject definitions | ✅ done | `bdbd07f` |
| 5 | Audio: AudioManager | ✅ done | `8a936f8` |
| 6 | Input: InputReader | ✅ done | `dff6fca` |
| 7 | Player: PlayerController, PlayerMovement | ✅ done | `269e1b2` |
| 8 | Player: PlayerHealth (IDamageable) | ✅ done | `c5e5c07` |
| 9 | Weapons: WeaponSlot, WeaponController | ✅ done | `744e992` |
| 10 | Weapons: Projectile, ProjectileManager | ✅ done | `e918b91` |
| 11 | Enemy: EnemyHealth, EnemyMovement, EnemyShooter, EnemyController | ✅ done | `e6a0817` |
| 12 | Enemy States: Chase, Dodge, Flank, Surround, ChaserRush, Warp, SniperKeepRange | ✅ done | `e6a0817` |
| 13 | Enemy Spawn: EnemyPool, EnemySpawner | ✅ done | `29fa0a0` |
| 14 | Level: LevelManager | ✅ done | `45c22cd` |
| 15 | HealthPack: HealthPack, HealthPackPool | ✅ done | `7315fa5` |
| 16 | UI: GameplayHUDController, WeaponSlotUI | ✅ done | `c474fd5` |
| 17 | UI: MainMenuController, ShipCardUI, ShipSelectionController | ✅ done | `c6ba18b` |
| 18 | UI: WeaponCardUI, WeaponSelectionController | ✅ done | `3bacc1c` |
| 19 | UI: DeathScreenController, VictoryScreenController | ✅ done | `b7981f8` |
| 19b | Fix: HealthPackPool drop chance + GameManager.StartLevel() | ✅ done | `3cee873` |
| 20 | UI: DamageNumber, DamageNumberPool | ✅ done | `9a44d91` |
| 21 | UI: EnemyIndicatorController | ✅ done | `b7a4f76` |
| 22 | Scene assembly (Inspector) | ⏳ manual | — |

---

## Step 22 — Scene Assembly (Manual Unity Editor Work)

All scripts are written. The following steps must be done in the Unity Editor.

### 1. Install packages (Package Manager)
- `com.unity.inputsystem` — New Input System
- `com.unity.cinemachine` — Cinemachine

### 2. Layers (Project Settings → Tags and Layers)
Add these User Layers:
- `Player`
- `Enemy`
- `PlayerProjectile`
- `EnemyProjectile`
- `Pickup`

### 3. Physics 2D Collision Matrix (Project Settings → Physics 2D)
Enable only these layer pairs:
- Player ↔ Enemy
- Player ↔ EnemyProjectile
- Player ↔ Pickup
- PlayerProjectile ↔ Enemy

Disable all others (especially Enemy ↔ Enemy, EnemyProjectile ↔ Enemy, Pickup ↔ Enemy).

### 4. Input Actions Asset
- Create `Assets/Settings/SpaceRoguelikeInputActions.inputactions`
- Action Map: `Gameplay`
  - Action `Move`: Value, Vector2 → bindings: `<Gamepad>/leftStick`, `<Keyboard>/WASD`
  - Action `Aim`: Value, Vector2 → bindings: `<Gamepad>/rightStick`
- Click "Generate C# Class" → save to `Assets/Scripts/`

### 5. AudioMixer
- Create `Assets/Audio/GameMixer.mixer`
- Groups: Master → Music, Master → SFX
- Expose parameters: `MusicVolume`, `SFXVolume`

### 6. ScriptableObject Instances
Create in `Assets/ScriptableObjects/`:

```
Ships/
  Archetype_Tank.asset      (MaxHealth=150, MoveSpeed=3.5, DamageMultiplier=0.8)
  Archetype_Damage.asset    (MaxHealth=80,  MoveSpeed=5,   DamageMultiplier=1.5)
  Archetype_Speed.asset     (MaxHealth=100, MoveSpeed=7,   DamageMultiplier=1.0)

Enemies/
  Enemy_Standard.asset  (Type=Standard,  BaseHp=30,  BaseSpeed=2.5, BaseRange=8,  BaseFireRate=1.0, BaseDamage=10)
  Enemy_Gunner.asset    (Type=Gunner,    BaseHp=25,  BaseSpeed=2.0, BaseRange=5,  BaseFireRate=2.0, BaseDamage=8)
  Enemy_Sniper.asset    (Type=Sniper,    BaseHp=20,  BaseSpeed=1.5, BaseRange=15, BaseFireRate=0.5, BaseDamage=18)
  Enemy_Chaser.asset    (Type=Chaser,    BaseHp=40,  BaseSpeed=4.5, BaseRange=1,  BaseFireRate=0,   BaseDamage=25)
  Enemy_Warper.asset    (Type=Warper,    BaseHp=25,  BaseSpeed=0,   BaseRange=12, BaseFireRate=0.8, BaseDamage=12)
  Enemy_Tank.asset      (Type=Tank,      BaseHp=120, BaseSpeed=1.0, BaseRange=6,  BaseFireRate=0.5, BaseDamage=20)

Weapons/
  Weapon_StarterGun.asset  (Damage=15, FireRate=2, Range=10, ProjectileSpeed=12, IsVampiric=false)
  (add more weapons to the pool in WeaponSelectionController._weaponPool)

Config/
  RunConfig.asset   (TotalLevels=10, MaxWeaponSlots=3, WeaponChoicesPerSelection=2,
                     HealthPackDropChance=0.003, WarperSafeZoneRadius=5, SpawnDelayBetweenEnemies=0.5)
  LevelConfig.asset (10 LevelEntries — see table below)
  RunData.asset     (no defaults needed — reset on run start)

Audio/
  AudioData.asset   (assign all clips after importing audio files)
```

**LevelConfig entries (10 levels):**
| Level | EnemyCount | HpMult | SpeedMult | FRMult | AiTier | AllowedTypes |
|-------|-----------|--------|-----------|--------|--------|--------------|
| 1 | 5–6 | 1.0 | 1.0 | 1.0 | Tier1_Basic | Standard |
| 2 | 6–8 | 1.1 | 1.0 | 1.0 | Tier1_Basic | Standard, Gunner |
| 3 | 7–9 | 1.2 | 1.1 | 1.0 | Tier2_Dodge | Standard, Gunner |
| 4 | 8–10 | 1.3 | 1.1 | 1.1 | Tier2_Dodge | Standard, Gunner, Sniper |
| 5 | 8–12 | 1.4 | 1.2 | 1.1 | Tier3_Flank | Standard, Gunner, Sniper, Chaser |
| 6 | 10–13 | 1.6 | 1.2 | 1.2 | Tier3_Flank | All except Tank |
| 7 | 10–14 | 1.8 | 1.3 | 1.2 | Tier4_Surround | All |
| 8 | 12–15 | 2.0 | 1.4 | 1.3 | Tier4_Surround | All |
| 9 | 13–16 | 2.3 | 1.5 | 1.4 | Tier5_Maximum | All |
| 10 | 15–18 | 2.6 | 1.6 | 1.5 | Tier5_Maximum | All |

### 7. Prefabs

**Player prefab** (`Assets/Prefabs/Gameplay/Player.prefab`):
- Components: `Rigidbody2D` (Dynamic, Continuous, Freeze Rot Z), `CircleCollider2D`, `SpriteRenderer`, `PlayerInput` (Actions=SpaceRoguelikeInputActions, Behavior=Send Messages), `InputReader`, `PlayerController`, `PlayerMovement`, `PlayerHealth`, `WeaponController`
- Tag: `Player`, Layer: `Player`
- Wire: PlayerController._movement, ._health, ._weaponController, ._inputReader all from same GO
- WeaponController._enemyLayer = Enemy layer

**Enemy prefabs** (one per type, `Assets/Prefabs/Gameplay/Enemies/`):
- Components: `Rigidbody2D` (Dynamic, Continuous, Freeze Rot Z), `Collider2D`, `SpriteRenderer`, `EnemyController`, `EnemyHealth`, `EnemyMovement`, `EnemyShooter`
- Tag: `Enemy`, Layer: `Enemy`
- EnemyController._data = matching EnemyData SO
- EnemyShooter._playerLayer = Player layer

**Projectile prefabs** (one per weapon/enemy):
- Components: `Rigidbody2D` (Kinematic), `CircleCollider2D` (IsTrigger=true), `SpriteRenderer`, `Projectile`
- Player projectiles: Layer = PlayerProjectile
- Enemy projectiles: Layer = EnemyProjectile
- Assign `Projectile` prefabs to WeaponData.ProjectilePrefab and EnemyData.ProjectilePrefab

**HealthPack prefab** (`Assets/Prefabs/Gameplay/HealthPack.prefab`):
- Components: `CircleCollider2D` (IsTrigger=true), `SpriteRenderer`, `HealthPack`
- Tag: `Player` → **No.** HealthPack.OnTriggerEnter2D checks `other.CompareTag("Player")` so the HealthPack needs no tag. The Player collider must have Tag=Player.
- Layer: `Pickup`

**DamageNumber prefab** (`Assets/Prefabs/UI/DamageNumber.prefab`):
- Components: `RectTransform`, `TextMeshProUGUI`, `DamageNumber`
- Parent it under DamageNumberPool._poolParent (a world-space Canvas or the Pool's transform)

**EnemyIndicator prefab** (`Assets/Prefabs/UI/EnemyIndicator.prefab`):
- Components: `RectTransform`, `Image` (arrow sprite)
- Parent it under the EnemyIndicatorLayer Canvas GO

### 8. Scene Hierarchy

```
_Managers/
  GameManager          → RunConfig, RunData, AudioData, LevelManager, PlayerController, WeaponSelectionController
  ScreenManager        → all 6 panel references
  AudioManager         → AudioMixer, SFX prefab pool, music source
  LevelManager         → EnemySpawner, RunData, RunConfig, AudioData
  EnemySpawner         → LevelConfig, RunConfig, EnemyPool, ProjectileManager, PlayerController ref, MapBounds collider
  EnemyPool            → 6 enemy prefab slots (one per EnemyType)
  ProjectileManager    → (no serialized fields needed)
  HealthPackPool       → HealthPack prefab, RunConfig, AudioData
  DamageNumberPool     → DamageNumber prefab, poolSize=20

World/
  MapBackground
  MapBoundary          → BoxCollider2D (Physics boundary, Layer=Default)
  CinemachineConfinerBounds → PolygonCollider2D or BoxCollider2D (confiner shape, no RB)

Gameplay/
  Player               → Player prefab instance

CinemachineVirtualCamera
  Follow = Player.transform
  Body: Framing Transposer, Dead Zone 0.1/0.1, Lookahead 0.15
  Extension: CinemachineConfiner2D → BoundingShape2D = CinemachineConfinerBounds collider

UI (Canvas — Screen Space Overlay, Scale With Screen Size 1080×1920)
  SafeArea (SafeAreaPanel)
    MainMenuPanel        → MainMenuController
    ShipSelectionPanel   → ShipSelectionController (3 ShipCardUI + Confirm + Back)
    GameplayHUDPanel     → GameplayHUDController
      HealthBar          → Slider
      EnemyCountText     → TMP
      LevelText          → TMP
      WeaponSlots        → 3× WeaponSlotUI
      JoystickLeft       → OnScreenStick (Gameplay/Move)
      JoystickRight      → OnScreenStick (Gameplay/Aim)
    WeaponSelectionPanel → WeaponSelectionController (2 offer cards + swap panel)
    DeathPanel           → DeathScreenController
    VictoryPanel         → VictoryScreenController
  EnemyIndicatorLayer    → EnemyIndicatorController (Canvas child, Overlay)
```

### 9. Wire EnemyHealth.OnDiedAtPosition → HealthPackPool

For each enemy spawned at runtime (not in Inspector), EnemySpawner must subscribe:

In `EnemySpawner.SpawnEnemy()`, after getting the enemy from the pool, add:
```csharp
enemy.Health.OnDiedAtPosition += _healthPackPool.TrySpawnAt;
```

And unsubscribe in the return path. Add `[SerializeField] private HealthPackPool _healthPackPool;` to EnemySpawner and wire in Inspector.

### 10. Wire DamageNumberPool calls

In `Projectile.cs`, after dealing damage:
```csharp
DamageNumberPool.Instance.Spawn(transform.position, _damage, false);
if (_isVampiric)
    DamageNumberPool.Instance.Spawn(transform.position, _vampiricHealAmount, true);
```

In `HealthPack.cs`, after calling Heal:
```csharp
DamageNumberPool.Instance.Spawn(transform.position, healAmount, true);
```

---

## Session Log

### 2026-05-23
- Implemented all 21 script steps (Utils, Core, Data, Audio, Input, Player, Weapons, Enemy, Level, HealthPack, UI).
- Fixed HealthPackPool drop chance direction (`>` → `>=`).
- Added `GameManager.StartLevel()` delegating to `LevelManager.StartLevel()`.
- Wrote DamageNumber (coroutine float-up animation) and DamageNumberPool (singleton, ObjectPool<DamageNumber>).
- Wrote EnemyIndicatorController (LateUpdate, viewport clip detection, edge projection, internal pool).
- Known issues:
  - EnemySpawner does not yet subscribe `OnDiedAtPosition → HealthPackPool.TrySpawnAt` — requires adding `_healthPackPool` field and wiring in scene (see Step 22 §9).
  - DamageNumberPool.Spawn calls not yet added to Projectile.cs and HealthPack.cs — add per Step 22 §10.
  - ChaserRushState uses `Physics2D.OverlapCircleAll` (allocating) instead of NonAlloc — acceptable since it fires once per chaser lifetime.
