# Plan: Core Gameplay Loop

## 1. Overview

Sistema completo de juego para Space Roguelike: movimiento twin-stick del jugador, sistema de armas con slots (máximo 3), 6 tipos de enemigos con IA escalante por nivel, mapa grande con cámara Cinemachine, selección de arma entre niveles, y gestión de run completa (10 niveles, permadeath). Una sola escena Unity — las pantallas se muestran/ocultan via ScreenManager.

---

## 2. Reference

GDD completo: `work/planning/core-gameplay-loop/gdd.md`

Secciones clave:
- §3 Core Loop — flujo de juego
- §4 Player Actions — twin-stick input + disparo automático con rango
- §5 Enemy Types + AI Tiers — 6 tipos, 5 tiers de dificultad
- §7 Screen Flow — MainMenu → ShipSelection → Gameplay → WeaponSelection → Victory/Death

---

## 3. Architecture Decisions

| Decisión | Elección | Justificación |
|----------|----------|---------------|
| Gestión de pantallas | Una sola escena + ScreenManager | Sin tiempos de carga entre niveles; mejor para mobile |
| Input | New Input System + `OnScreenStick` | Soporte nativo para twin-stick en touch; acciones declaradas en asset |
| Cámara | Cinemachine VirtualCamera + CinemachineConfiner2D | Dead zone y confiner configurables sin código |
| Estado de run transient | `RunData` ScriptableObject con `Reset()` | Compartible entre sistemas sin acoplamiento directo |
| AI enemigos | State machine (`StateMachine` + `IState`) por `EnemyController` | Permite escalar comportamiento sin condiciones anidadas |
| Pooling | `ObjectPool<T>` genérico para proyectiles, enemigos, paquetes de vida, números de daño | Mandatorio en mobile para objetos de alta frecuencia |
| Datos de configuración | ScriptableObjects para todos los valores ajustables | Sin magic numbers; diseñador puede tweakear desde Inspector |
| Comunicación entre sistemas | Eventos C# (`event Action`) | Desacoplamiento; los sistemas no se referencian directamente |

---

## 4. Scripts to Create

### Core — `Assets/Scripts/Core/`

#### `GameManager.cs` — MonoBehaviour, singleton
Controla el estado global del juego y orquesta transiciones de run.

**Campos:**
```
[SerializeField] private RunData _runData
[SerializeField] private RunConfig _runConfig
GameState CurrentState { get; private set; }
```

**Eventos:**
```
event Action<GameState> OnStateChanged
```

**Métodos:**
```
void StartNewRun(ShipArchetypeData archetype)
void OnLevelCompleted()
void OnRunCompleted()
void OnPlayerDied()
private void SetState(GameState newState)
```

---

#### `ScreenManager.cs` — MonoBehaviour, singleton
Muestra/oculta paneles UI. Ningún panel se referencia entre sí.

**Campos:**
```
[SerializeField] private GameObject _mainMenuPanel
[SerializeField] private GameObject _shipSelectionPanel
[SerializeField] private GameObject _gameplayHUDPanel
[SerializeField] private GameObject _weaponSelectionPanel
[SerializeField] private GameObject _deathPanel
[SerializeField] private GameObject _victoryPanel
```

**Métodos:**
```
void ShowScreen(GameScreen screen)
```

---

#### `AudioManager.cs` — MonoBehaviour, singleton (ver audio_skill.md)
Pool de AudioSources para SFX; AudioSource dedicado para música.

**Campos:**
```
[SerializeField] private AudioSource _musicSource
[SerializeField] private AudioSource _sfxSourcePrefab
[SerializeField] private AudioMixer _mixer
[SerializeField] private int _sfxPoolSize = 12
```

**Métodos:**
```
void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
void PlayMusic(AudioClip clip, bool loop = true)
void StopMusic()
void SetMusicVolume(float normalized)
void SetSFXVolume(float normalized)
```

---

### Mechanics/Player — `Assets/Scripts/Mechanics/Player/`

#### `InputReader.cs` — MonoBehaviour
Envuelve `PlayerInput` del New Input System y expone eventos de movimiento y apuntado.

**Campos:**
```
[SerializeField] private PlayerInput _playerInput
Vector2 MoveInput { get; private set; }
Vector2 AimInput { get; private set; }
```

**Eventos:**
```
event Action<Vector2> OnMoveChanged
event Action<Vector2> OnAimChanged
```

**Métodos (callbacks de PlayerInput):**
```
void OnMove(InputValue value)
void OnAim(InputValue value)
```

---

#### `PlayerController.cs` — MonoBehaviour, thin
Raíz del prefab del jugador. Inicializa los subsistemas con el arquetipo seleccionado.

**Campos:**
```
[SerializeField] private PlayerMovement _movement
[SerializeField] private PlayerHealth _health
[SerializeField] private WeaponController _weaponController
[SerializeField] private InputReader _inputReader
```

**Métodos:**
```
void Initialize(ShipArchetypeData archetype)
```

---

#### `PlayerMovement.cs` — MonoBehaviour
Mueve la nave via `Rigidbody2D`. Lee `MoveInput` de `InputReader`.

**Campos:**
```
[SerializeField] private Rigidbody2D _rb
[SerializeField] private InputReader _inputReader
private float _moveSpeed
```

**Métodos:**
```
void Initialize(float moveSpeed)
private void FixedUpdate()   // aplica velocidad desde _moveInput
```

---

#### `PlayerHealth.cs` — MonoBehaviour
Gestiona HP del jugador, recibe daño, aplica curación, detecta muerte.

**Campos:**
```
private float _maxHealth
private float _currentHealth
```

**Eventos:**
```
event Action<float> OnHealthChanged    // parámetro: porcentaje 0–1
event Action OnDied
```

**Métodos:**
```
void Initialize(float maxHealth)
void TakeDamage(float amount)
void Heal(float amount)                // llamado por paquetes de vida y armas vampíricas
float HealthPercent { get; }
private void Die()
```

---

### Mechanics/Weapons — `Assets/Scripts/Mechanics/Weapons/`

#### `WeaponSlot.cs` — plain C#
Modelo de un slot de arma: referencia al arma equipada + cooldown actual.

**Campos:**
```
WeaponData EquippedWeapon { get; private set; }
bool IsOnCooldown { get; }
bool IsEmpty { get; }
```

**Métodos:**
```
void Equip(WeaponData weapon)
void Clear()
void Tick(float deltaTime)         // reduce cooldown
bool TryResetCooldown()            // retorna true y resetea si cooldown = 0
```

---

#### `WeaponController.cs` — MonoBehaviour
Gestiona hasta 3 `WeaponSlot`, detecta enemigos en la dirección de apuntado, dispara via `ProjectileManager`.

**Campos:**
```
[SerializeField] private InputReader _inputReader
[SerializeField] private ProjectileManager _projectileManager
[SerializeField] private PlayerHealth _playerHealth     // para armas vampíricas
[SerializeField] private AudioData _audioData
private WeaponSlot[] _slots = new WeaponSlot[3]
private const int MAX_SLOTS = 3
```

**Eventos:**
```
event Action<WeaponSlot[]> OnSlotsChanged    // para actualizar HUD
```

**Métodos:**
```
void EquipWeapon(WeaponData weapon, int slotIndex)
void ClearSlot(int slotIndex)
WeaponSlot[] GetSlots()
private void Update()                         // tick cooldowns + intento de disparo
private bool TryFire(WeaponSlot slot)         // detecta enemigo en rango + ángulo
private EnemyController FindTargetInAimDirection(float range)
    // Physics2D.OverlapCircle filtrado por ángulo con dirección de aim
```

---

#### `Projectile.cs` — MonoBehaviour, poolable
Proyectil que se mueve en línea recta, daña al enemigo al colisionar, se devuelve al pool.

**Campos:**
```
private Vector2 _direction
private float _speed
private float _damage
private bool _isVampiric
private float _vampireHeal
private Action _onReturn
```

**Métodos:**
```
void Initialize(Vector2 direction, float speed, float damage,
                bool isVampiric, float vampireHeal, Action onReturn)
private void Update()                           // mueve el proyectil
private void OnTriggerEnter2D(Collider2D other) // daña enemigo, regresa al pool
```

---

#### `ProjectileManager.cs` — MonoBehaviour
Mantiene un pool por `WeaponData`. Crea pools bajo demanda.

**Campos:**
```
[SerializeField] private int _defaultPoolSize = 20
private Dictionary<WeaponData, ObjectPool<Projectile>> _pools
```

**Métodos:**
```
Projectile Spawn(WeaponData weapon, Vector2 position, Vector2 direction)
void ReturnToPool(WeaponData weapon, Projectile projectile)
private ObjectPool<Projectile> GetOrCreatePool(WeaponData weapon)
```

---

### Mechanics/Enemy — `Assets/Scripts/Mechanics/Enemy/`

#### `EnemyController.cs` — MonoBehaviour, thin
Raíz del prefab de enemigo. Inicializa subsistemas y máquina de estados.

**Campos:**
```
[SerializeField] private EnemyData _data
[SerializeField] private EnemyHealth _health
[SerializeField] private EnemyMovement _movement
[SerializeField] private EnemyShooter _shooter
private StateMachine _stateMachine
private float _assignedFlankAngle   // asignado por EnemySpawner en tiers 3+
```

**Métodos:**
```
void Initialize(EnemyData data, EnemyAITier tier, float statMultiplier,
                Transform playerTransform, float assignedAngle = 0f)
private IState BuildInitialState(EnemyAITier tier)
void OnDisable()    // resetea estado para reutilización del pool
```

---

#### `EnemyHealth.cs` — MonoBehaviour
HP del enemigo, recibe daño, lanza eventos de muerte y drop de vida.

**Campos:**
```
private float _maxHealth
private float _currentHealth
private EnemyData _data
```

**Eventos:**
```
event Action OnDied
event Action<Vector2> OnDiedAtPosition    // para HealthPackPool: posición del drop
```

**Métodos:**
```
void Initialize(float maxHealth)
void TakeDamage(float amount)
private void Die()
```

---

#### `EnemyMovement.cs` — MonoBehaviour
Mueve al enemigo via `Rigidbody2D`. Las AI States llaman a `SetVelocity` o `TeleportTo`.

**Campos:**
```
[SerializeField] private Rigidbody2D _rb
private float _moveSpeed
```

**Métodos:**
```
void Initialize(float moveSpeed)
void SetVelocity(Vector2 velocity)
void TeleportTo(Vector2 position)       // para WarpState
void Stop()
Vector2 Position { get; }
```

---

#### `EnemyShooter.cs` — MonoBehaviour
Dispara proyectiles hacia el jugador según cadencia y rango del tipo.

**Campos:**
```
[SerializeField] private ProjectileManager _projectileManager
[SerializeField] private EnemyData _data
private float _cooldown
private Transform _playerTransform
```

**Métodos:**
```
void Initialize(EnemyData data, Transform playerTransform, float fireRateMultiplier)
void TryShoot()    // llamado por estado AI; verifica cooldown y rango
```

---

### Mechanics/Enemy/States — `Assets/Scripts/Mechanics/Enemy/States/`

Todas implementan `IState` (plain C#). Reciben referencias a `EnemyMovement`, `EnemyShooter`, y `Transform` del jugador en el constructor. La máquina de estados en `EnemyController` llama `Enter()`, `Update()`, `Exit()`.

#### `EnemyChaseState.cs`
Mueve directamente hacia el jugador. Llama a `TryShoot()`. Sin esquiva. Usado en Tier 1.

#### `EnemyDodgeState.cs`
Mueve hacia el jugador pero detecta proyectiles cercanos (OverlapCircle pequeño) y aplica un desvío lateral. Tier 2+. Hereda lógica de chase.

#### `EnemyFlankState.cs`
Mantiene `_assignedFlankAngle` relativo al jugador a distancia media. Tier 3+. Múltiples enemigos convergen desde ángulos distintos.

#### `EnemySurroundState.cs`
Mantiene `_assignedFlankAngle` a radio fijo alrededor del jugador. Cierra el cerco gradualmente. Tier 4+.

#### `ChaserRushState.cs`
Específico para `EnemyType.Chaser`. Carga en línea recta hacia el jugador a velocidad máxima. Al hacer `OnTriggerEnter2D` con el jugador: aplica daño de explosión y llama `EnemyHealth.Die()`.

#### `WarpState.cs`
Específico para `EnemyType.Warper`. Teletransporta a `EnemyMovement.TeleportTo()` cada `_warpInterval` segundos. La posición destino es aleatoria dentro del mapa pero a distancia mínima del jugador (definida en `RunConfig.WarperSafeZoneRadius`).

#### `SniperKeepRangeState.cs`
Específico para `EnemyType.Sniper`. Mantiene distancia `_data.BaseRange * 0.9f` del jugador retrocediendo si el jugador se acerca. Llama a `TryShoot()`.

---

### Mechanics/Enemy/Spawn — `Assets/Scripts/Mechanics/Enemy/Spawn/`

#### `EnemyPool.cs` — MonoBehaviour
Un pool por tipo de enemigo. `EnemySpawner` le pide instancias; el pool las devuelve cuando los enemigos mueren.

**Campos:**
```
[SerializeField] private EnemyController[] _enemyPrefabs   // índice = (int)EnemyType
[SerializeField] private int _initialPoolSizePerType = 8
private ObjectPool<EnemyController>[] _pools
```

**Métodos:**
```
EnemyController Get(EnemyType type)
void Return(EnemyController enemy)
```

---

#### `EnemySpawner.cs` — MonoBehaviour
Genera enemigos en los bordes del mapa según la configuración del nivel activo.

**Campos:**
```
[SerializeField] private EnemyPool _pool
[SerializeField] private RunData _runData
[SerializeField] private LevelConfig _levelConfig
[SerializeField] private BoxCollider2D _mapBounds   // para calcular posiciones de borde
[SerializeField] private Transform _playerTransform
private int _remainingToSpawn
```

**Métodos:**
```
void StartLevel(int levelIndex)
    // lee LevelConfig, calcula conteo y tipos, inicia coroutine de spawn
private IEnumerator SpawnRoutine(LevelEntry entry)
    // spawnea con delay entre enemigos
private Vector2 GetRandomBorderPosition()
    // posición aleatoria fuera del mapa en alguno de los 4 bordes
private void SpawnEnemy(EnemyType type, Vector2 position, LevelEntry entry)
    // llama EnemyPool.Get(), llama EnemyController.Initialize() con tier y ángulo
```

---

### Mechanics/Level — `Assets/Scripts/Mechanics/Level/`

#### `LevelManager.cs` — MonoBehaviour
Coordina el ciclo de vida de cada nivel: inicia el spawner, cuenta bajas, detecta nivel completo y run completa.

**Campos:**
```
[SerializeField] private EnemySpawner _spawner
[SerializeField] private RunData _runData
[SerializeField] private LevelConfig _levelConfig
private int _enemiesAlive
private int _totalEnemiesThisLevel
```

**Eventos:**
```
event Action OnLevelCompleted
event Action OnRunCompleted
```

**Métodos:**
```
void StartLevel()
    // incrementa RunData.CurrentLevel, calcula total de enemigos, suscribe a spawner
private void OnEnemyDied()
    // decrementa _enemiesAlive; si = 0 llama LevelCompleted
private void LevelCompleted()
    // si nivel == TotalLevels → OnRunCompleted; si no → OnLevelCompleted
```

---

### Mechanics/HealthPack — `Assets/Scripts/Mechanics/HealthPack/`

#### `HealthPack.cs` — MonoBehaviour, poolable
Objeto coleccionable. Se recoge cuando el jugador pasa sobre él (trigger).

**Campos:**
```
[SerializeField] private float _healAmount = 20f
private Action _onReturn
```

**Métodos:**
```
void Initialize(Vector2 position, Action onReturn)
private void OnTriggerEnter2D(Collider2D other)   // verifica tag "Player", cura, regresa al pool
```

---

#### `HealthPackPool.cs` — MonoBehaviour
Pool de HealthPacks. Se suscribe al evento `OnDiedAtPosition` de cada `EnemyHealth`.

**Campos:**
```
[SerializeField] private HealthPack _prefab
[SerializeField] private RunConfig _runConfig
private ObjectPool<HealthPack> _pool
```

**Métodos:**
```
void TrySpawnAt(Vector2 position)
    // Random.value < _runConfig.HealthPackDropChance → Get() del pool e Initialize
```

---

### UI — `Assets/Scripts/UI/`

#### `MainMenuController.cs` — MonoBehaviour
**Campos:** `[SerializeField] Button _playButton`
**OnEnable/OnDisable:** suscribe/desuscribe `_playButton.onClick`
**Acción:** llama `ScreenManager.ShowScreen(GameScreen.ShipSelection)`

---

#### `ShipSelectionController.cs` — MonoBehaviour
Muestra 3 `ShipCardUI`. Al confirmar, llama `GameManager.StartNewRun(selectedArchetype)`.

**Campos:**
```
[SerializeField] private ShipCardUI[] _cards    // 3 cartas
[SerializeField] private ShipArchetypeData[] _archetypes  // 3 SO
[SerializeField] private Button _confirmButton
private ShipArchetypeData _selected
```

**Métodos:**
```
void OnCardSelected(ShipArchetypeData archetype)
void OnConfirmPressed()
```

---

#### `ShipCardUI.cs` — MonoBehaviour
Muestra nombre, sprite, y stats de un arquetipo. Emite evento al ser seleccionado.

**Campos:**
```
[SerializeField] private Image _shipImage
[SerializeField] private TextMeshProUGUI _nameText
[SerializeField] private TextMeshProUGUI _statsText
[SerializeField] private Button _selectButton
[SerializeField] private GameObject _selectedIndicator
```

**Métodos:**
```
void Setup(ShipArchetypeData data, Action<ShipArchetypeData> onSelected)
void SetSelected(bool selected)
```

---

#### `GameplayHUDController.cs` — MonoBehaviour
Actualiza barra de vida, contador de enemigos, nivel actual, y slots de armas visibles.

**Campos:**
```
[SerializeField] private Slider _healthBar
[SerializeField] private TextMeshProUGUI _enemyCountText
[SerializeField] private TextMeshProUGUI _levelText
[SerializeField] private WeaponSlotUI[] _weaponSlotUIs    // 3 elementos
[SerializeField] private PlayerHealth _playerHealth
[SerializeField] private LevelManager _levelManager
[SerializeField] private WeaponController _weaponController
```

**OnEnable/OnDisable:** suscribe/desuscribe a `PlayerHealth.OnHealthChanged`, `LevelManager.OnEnemyCountChanged`, `WeaponController.OnSlotsChanged`

---

#### `WeaponSlotUI.cs` — MonoBehaviour
Muestra el ícono del arma en un slot del HUD. Indicador vacío si slot vacío.

**Campos:**
```
[SerializeField] private Image _weaponIcon
[SerializeField] private GameObject _emptyIndicator
```

**Métodos:** `void Refresh(WeaponSlot slot)`

---

#### `WeaponSelectionController.cs` — MonoBehaviour
Muestra 2 cartas de arma aleatorias. Si hay 3 armas activas, muestra los slots actuales para intercambio.

**Campos:**
```
[SerializeField] private WeaponCardUI[] _offerCards     // 2 cartas de oferta
[SerializeField] private WeaponCardUI[] _currentSlotCards  // 3 cartas de slots actuales
[SerializeField] private GameObject _swapPanel
[SerializeField] private WeaponController _weaponController
[SerializeField] private WeaponData[] _weaponPool       // pool de todas las armas disponibles
```

**Métodos:**
```
void Show()
    // elige 2 armas al azar del pool, configura cartas, detecta si hay slots llenos
void OnWeaponChosen(WeaponData chosen)
    // si hay slot vacío: equipa; si no: muestra _swapPanel con slots actuales
void OnSlotChosen(int slotIndex)
    // llama WeaponController.EquipWeapon(chosen, slotIndex), cierra panel
```

---

#### `WeaponCardUI.cs` — MonoBehaviour
Muestra nombre, ícono, daño, cadencia, rango y efectos de un arma.

**Campos:**
```
[SerializeField] private Image _icon
[SerializeField] private TextMeshProUGUI _nameText
[SerializeField] private TextMeshProUGUI _damageText
[SerializeField] private TextMeshProUGUI _fireRateText
[SerializeField] private TextMeshProUGUI _rangeText
[SerializeField] private TextMeshProUGUI _effectsText
[SerializeField] private Button _selectButton
```

**Métodos:** `void Setup(WeaponData data, Action<WeaponData> onSelected)`

---

#### `DeathScreenController.cs` — MonoBehaviour
**Campos:** `TextMeshProUGUI _levelReachedText`, `Button _retryButton`, `Button _menuButton`
**OnEnable:** muestra "Moriste en el nivel X" usando `RunData.CurrentLevel`
**Acciones:** Retry → `GameManager.StartNewRun(RunData.SelectedArchetype)` | Menu → `ScreenManager.ShowScreen(MainMenu)`

---

#### `VictoryScreenController.cs` — MonoBehaviour
**Campos:** `TextMeshProUGUI _statsText`, `Button _menuButton`
**OnEnable:** muestra niveles completados + enemigos eliminados desde `RunData`
**Acción:** Menu → `ScreenManager.ShowScreen(MainMenu)`

---

#### `EnemyIndicatorController.cs` — MonoBehaviour
Muestra flechas en los bordes de pantalla apuntando hacia enemigos fuera del área visible.

**Campos:**
```
[SerializeField] private RectTransform _indicatorPrefab   // flecha UI
[SerializeField] private int _maxIndicators = 8
[SerializeField] private Camera _gameCamera
private ObjectPool<RectTransform> _indicatorPool
private List<EnemyController> _trackedEnemies
```

**Métodos:**
```
private void LateUpdate()
    // para cada enemigo activo fuera de pantalla: posiciona una flecha en borde
    // usa Camera.WorldToViewportPoint para detectar si está fuera [0,1]
```

---

### Utils — `Assets/Scripts/Utils/`

#### `ObjectPool.cs` — plain C# genérico
Implementación con `Queue<T>`. Crea instancias bajo demanda si el pool está vacío.

**Métodos:** `T Get()`, `void Return(T obj)`, constructor `(T prefab, int initialSize, Transform parent)`

---

#### `StateMachine.cs` — plain C# (+ `IState` interface en el mismo archivo)
`IState`: `Enter()`, `Update()`, `Exit()`
`StateMachine`: `SetState(IState)`, `Update()`, `IState CurrentState`

---

#### `DamageNumberPool.cs` — MonoBehaviour
Pool de TextMeshProUGUI flotantes para números de daño y curación.

**Campos:**
```
[SerializeField] private DamageNumber _prefab
[SerializeField] private int _poolSize = 20
[SerializeField] private Canvas _worldCanvas
private ObjectPool<DamageNumber> _pool
```

**Métodos:** `void Spawn(Vector2 worldPos, float amount, bool isHeal)`

#### `DamageNumber.cs` — MonoBehaviour, poolable
Sube y desaparece en ~0.8s. TextMeshProUGUI con color verde (curación) o blanco (daño).

---

#### `SafeAreaPanel.cs` — MonoBehaviour
Ver ui_skill.md. Se añade al root RectTransform de todos los paneles fullscreen.

#### `ButtonFeedback.cs` — MonoBehaviour
Ver ui_skill.md. Se añade a todos los botones.

---

## 5. Scripts to Modify

Ninguno — proyecto greenfield.

---

## 6. ScriptableObjects Required

| Clase | CreateAssetMenu | Instancias a crear | Campos clave |
|-------|----------------|-------------------|--------------|
| `ShipArchetypeData` | `Game/Ship Archetype` | 3 (Tank, Damage, Speed) | `DisplayName`, `ShipSprite`, `MaxHealth`, `MoveSpeed`, `DamageMultiplier` |
| `EnemyData` | `Game/Enemy Data` | 6 (uno por tipo) | `EnemyType`, `BaseHp`, `BaseSpeed`, `BaseFireRate`, `BaseDamage`, `BaseRange` |
| `WeaponData` | `Game/Weapon Data` | ≥1 (starter weapon + pool) | `Name`, `Icon`, `Damage`, `FireRate`, `Range`, `IsVampiric`, `VampiricHealAmount`, `ProjectilePrefab`, `FireSFX` |
| `LevelConfig` | `Game/Level Config` | 1 | `LevelEntry[] Levels` (10 entradas) — cada `LevelEntry`: `EnemyCountMin/Max`, `HpMultiplier`, `SpeedMultiplier`, `FireRateMultiplier`, `AiTier`, `AllowedEnemyTypes[]` |
| `RunConfig` | `Game/Run Config` | 1 | `TotalLevels=10`, `HealthPackDropChance=0.003f`, `WarperSafeZoneRadius`, `MaxWeaponSlots=3`, `WeaponChoicesPerSelection=2` |
| `RunData` | `Game/Run Data` | 1 | `CurrentLevel`, `SelectedArchetype`, `List<WeaponData> ActiveWeapons`, `EnemiesKilled`, `void ResetForNewRun()` |
| `AudioData` | `Game/Audio Data` | 1 | Clips: `ShootDefault`, `Impact`, `Explosion`, `ShipHit`, `LowHealthAlarm`, `EnemySpawn`, `HealthPackDrop`, `HealthPickup`, `VampireAbsorb`, `LevelComplete`, `PlayerDeath`, `ButtonClick`, `GameplayMusic`, `MenuMusic` |

**`EnemyAITier` enum** (en `Assets/Scripts/Core/GameState.cs` o archivo propio):
```
Tier1_Basic, Tier2_Dodge, Tier3_Flank, Tier4_Surround, Tier5_Maximum
```

**`EnemyType` enum** (en `EnemyData.cs` o archivo propio):
```
Standard, Gunner, Sniper, Chaser, Warper, Tank
```

**`GameState` enum:** `MainMenu, ShipSelection, Playing, WeaponSelection, Victory, Dead`
**`GameScreen` enum:** `MainMenu, ShipSelection, Gameplay, WeaponSelection, Death, Victory`

---

## 7. Inspector Setup

### Input System
- Instalar paquete `com.unity.inputsystem` desde Package Manager
- Crear `Assets/Settings/SpaceRoguelikeInputActions.inputactions`
  - Action Map: `Gameplay`
  - Acción `Move`: Value, Vector2, binding: `<Gamepad>/leftStick` + `<Keyboard>/WASD`
  - Acción `Aim`: Value, Vector2, binding: `<Gamepad>/rightStick`
- Generar clase C# desde el asset (botón "Generate C# Class")
- En la Canvas de HUD: crear dos objetos UI vacíos con `OnScreenStick`:
  - Left stick → acción `Gameplay/Move`
  - Right stick → acción `Gameplay/Aim`

### Prefab: Player
- Componentes: `Rigidbody2D`, `CircleCollider2D`, `SpriteRenderer`, `PlayerInput`, `InputReader`, `PlayerController`, `PlayerMovement`, `PlayerHealth`, `WeaponController`
- `Rigidbody2D`: Body Type = Dynamic, Collision Detection = Continuous, Freeze Rotation Z = true
- `PlayerInput`: Actions = `SpaceRoguelikeInputActions`, Behavior = Send Messages
- Tag: `Player`
- Layer: `Player`
- `PlayerController._movement` → `PlayerMovement` del mismo GO
- `PlayerController._health` → `PlayerHealth` del mismo GO
- `PlayerController._weaponController` → `WeaponController` del mismo GO
- `PlayerController._inputReader` → `InputReader` del mismo GO

### Prefab: Enemy (uno por tipo)
- Componentes: `Rigidbody2D`, `Collider2D` apropiado, `SpriteRenderer`, `EnemyController`, `EnemyHealth`, `EnemyMovement`, `EnemyShooter`
- `Rigidbody2D`: Body Type = Dynamic, Collision Detection = Continuous, Freeze Rotation Z = true
- Asignar `EnemyData` correspondiente en `EnemyController._data`
- Tag: `Enemy`
- Layer: `Enemy`

### Prefab: Projectile (uno por arma o genérico)
- Componentes: `Rigidbody2D`, `CircleCollider2D` (IsTrigger = true), `SpriteRenderer`, `Projectile`
- Layer: `Projectile`
- `Rigidbody2D`: Body Type = Kinematic (moveremos via `transform.Translate` o asignando velocidad)

### Prefab: HealthPack
- Componentes: `CircleCollider2D` (IsTrigger = true), `SpriteRenderer`, `HealthPack`
- Layer: `Pickup`

### Camera — Cinemachine
- Instalar `com.unity.cinemachine` desde Package Manager
- Agregar `CinemachineVirtualCamera` a la escena
  - Follow = Transform del Player
  - Body: Framing Transposer
  - Dead Zone X/Y: 0.1
  - Lookahead Time: 0.15
- Crear un `GameObject` vacío con `PolygonCollider2D` o `BoxCollider2D` que delimita el mapa completo; agregar `CinemachineConfiner2D` a la VirtualCamera y asignar ese collider

### Scene Hierarchy (recomendada)
```
_Managers
  ├── GameManager
  ├── ScreenManager
  ├── AudioManager
  ├── LevelManager
  ├── EnemySpawner
  ├── EnemyPool
  ├── ProjectileManager
  ├── HealthPackPool
  └── DamageNumberPool

World
  ├── MapBackground (Tilemap o Sprite)
  ├── MapBoundary (BoxCollider2D — límite físico del mapa)
  └── CinemachineConfinerBounds (Collider para confiner)

Gameplay
  └── Player (prefab)

UI (Canvas — Screen Space Overlay, Scale With Screen Size 1080×1920)
  ├── SafeArea (SafeAreaPanel)
  │   ├── MainMenuPanel
  │   ├── ShipSelectionPanel
  │   ├── GameplayHUDPanel
  │   │   ├── HealthBar (Slider)
  │   │   ├── EnemyCountText (TMP)
  │   │   ├── LevelText (TMP)
  │   │   ├── WeaponSlots (3× WeaponSlotUI)
  │   │   ├── JoystickLeft (OnScreenStick)
  │   │   └── JoystickRight (OnScreenStick)
  │   ├── WeaponSelectionPanel
  │   ├── DeathPanel
  │   └── VictoryPanel
  └── EnemyIndicatorLayer (sobre todo lo demás)
```

### Layer Collision Matrix
En `Project Settings → Physics 2D → Layer Collision Matrix`:
- `Player` vs `Enemy`: ✅ (detección de daño)
- `Player` vs `EnemyProjectile`: ✅
- `Player` vs `Pickup`: ✅
- `PlayerProjectile` vs `Enemy`: ✅
- `PlayerProjectile` vs `Player`: ❌
- `Enemy` vs `Enemy`: ❌ (sin física entre enemigos)
- `EnemyProjectile` vs `Enemy`: ❌
- `Pickup` vs `Enemy`: ❌

### AudioMixer
- Crear `Assets/Audio/GameMixer.mixer`
- Grupos: Master → Music, Master → SFX
- Exponer parámetros: `MusicVolume`, `SFXVolume`
- Asignar grupos en `AudioManager`: `_musicSource` → Music group; SFX pool sources → SFX group

### ScriptableObject Instances
Crear en `Assets/ScriptableObjects/`:
```
├── Ships/
│   ├── Archetype_Tank.asset
│   ├── Archetype_Damage.asset
│   └── Archetype_Speed.asset
├── Enemies/
│   ├── Enemy_Standard.asset
│   ├── Enemy_Gunner.asset
│   ├── Enemy_Sniper.asset
│   ├── Enemy_Chaser.asset
│   ├── Enemy_Warper.asset
│   └── Enemy_Tank.asset
├── Weapons/
│   └── Weapon_StarterGun.asset
├── Config/
│   ├── LevelConfig.asset
│   ├── RunConfig.asset
│   └── RunData.asset
└── Audio/
    └── AudioData.asset
```

---

## 8. Data Flow

```
[New Input System]
  OnScreenStick (Left)  →  InputReader.OnMove()  →  PlayerMovement (FixedUpdate)
  OnScreenStick (Right) →  InputReader.OnAim()   →  WeaponController (Update)

[Combat Loop]
  WeaponController.Update()
    → FindTargetInAimDirection() [Physics2D.OverlapCircle + ángulo]
    → si target encontrado + slot cooldown = 0:
        ProjectileManager.Spawn(weapon, pos, dir)
        AudioManager.PlaySFX(fireSFX)

  Projectile.OnTriggerEnter2D(Enemy)
    → EnemyHealth.TakeDamage(damage)
    → si isVampiric: PlayerHealth.Heal(vampireHeal) + DamageNumberPool.Spawn(heal, isHeal:true)
    → DamageNumberPool.Spawn(damage, isHeal:false)
    → ProjectilePool.Return(this)

  EnemyHealth.TakeDamage() → HP ≤ 0 → EnemyHealth.Die()
    → OnDied.Invoke()         → LevelManager.OnEnemyDied()
    → OnDiedAtPosition.Invoke() → HealthPackPool.TrySpawnAt(pos)
    → AudioManager.PlaySFX(explosionSFX)
    → EnemyPool.Return(this enemy)

[Level Progression]
  LevelManager.OnEnemyDied()
    → _enemiesAlive--
    → si _enemiesAlive == 0:
        si CurrentLevel == TotalLevels → OnRunCompleted
        si no → OnLevelCompleted

  OnLevelCompleted → GameManager.OnLevelCompleted()
    → RunData.CurrentLevel++
    → ScreenManager.ShowScreen(WeaponSelection)
    → WeaponSelectionController.Show()

  WeaponSelectionController.OnSlotChosen(index)
    → WeaponController.EquipWeapon(chosen, index)
    → ScreenManager.ShowScreen(Gameplay)
    → LevelManager.StartLevel()
    → EnemySpawner.StartLevel(levelIndex)

[Player Death]
  PlayerHealth.OnDied → GameManager.OnPlayerDied()
    → ScreenManager.ShowScreen(Death)
    → AudioManager.PlaySFX(playerDeathSFX)

[Run Complete]
  GameManager.OnRunCompleted()
    → ScreenManager.ShowScreen(Victory)
```

---

## 9. Implementation Steps

| # | Paso | Scripts | Notas |
|---|------|---------|-------|
| 1 | Utilities | `ObjectPool<T>`, `StateMachine`, `IState` | Base para todos los sistemas; sin dependencias |
| 2 | Enums y Core | `GameState`, `GameScreen`, `EnemyType`, `EnemyAITier`, `GameManager`, `ScreenManager` | GameManager + ScreenManager son los primeros en escena |
| 3 | ScriptableObject definitions | `ShipArchetypeData`, `EnemyData`, `WeaponData`, `LevelConfig`, `RunConfig`, `RunData`, `AudioData` | Solo las clases; las instancias (.asset) se crean en Inspector |
| 4 | Audio | `AudioManager` | Singleton DDOL; crear prefab y asignar en escena |
| 5 | Input | `InputReader`; crear `SpaceRoguelikeInputActions.inputactions` | Generar clase C#; `PlayerInput` en prefab Player |
| 6 | Player movimiento | `PlayerController`, `PlayerMovement` | Probar movimiento en escena antes de continuar |
| 7 | Player salud | `PlayerHealth` | Probar con daño manual desde Inspector |
| 8 | Weapon slot + controller | `WeaponSlot`, `WeaponController` | Probar detección de enemigo dummy con OverlapCircle |
| 9 | Proyectil | `Projectile`, `ProjectileManager` | Pool inicial size=20; probar disparo con enemigo dummy |
| 10 | Enemy base | `EnemyController`, `EnemyHealth`, `EnemyMovement`, `EnemyShooter` | Sin AI todavía; enemigo se queda quieto |
| 11 | Enemy AI states | `EnemyChaseState`, `EnemyDodgeState`, `EnemyFlankState`, `EnemySurroundState`, `ChaserRushState`, `WarpState`, `SniperKeepRangeState` | Probar cada state de forma aislada |
| 12 | Enemy pool + spawner | `EnemyPool`, `EnemySpawner` | Probar spawn en borde con Level 1 config |
| 13 | Level manager | `LevelManager` | Conectar OnEnemyDied → conteo → OnLevelCompleted |
| 14 | Health pack | `HealthPack`, `HealthPackPool` | Probar drop desde Inspector (forzar 100% drop rate en RunConfig para test) |
| 15 | Cámara | Cinemachine setup (solo Inspector) | CinemachineVirtualCamera + Confiner2D + límites del mapa |
| 16 | HUD | `GameplayHUDController`, `WeaponSlotUI` | Conectar eventos de PlayerHealth y WeaponController |
| 17 | Menús base | `MainMenuController`, `ShipSelectionController`, `ShipCardUI`, `SafeAreaPanel`, `ButtonFeedback` | Probar flujo MainMenu → ShipSelection → StartRun |
| 18 | Selección de arma | `WeaponSelectionController`, `WeaponCardUI` | Probar flujo completo nivel completo → selección → siguiente nivel |
| 19 | Fin de run | `DeathScreenController`, `VictoryScreenController` | Probar muerte y victoria; RunData se resetea en StartNewRun |
| 20 | Números de daño | `DamageNumber`, `DamageNumberPool` | Polishing; conectar en `Projectile` y `HealthPack` |
| 21 | Indicadores de enemigo | `EnemyIndicatorController` | Polishing; conectar lista de enemigos activos desde LevelManager |
| 22 | Instancias SO + escena final | Crear todos los .asset, prefabs, asignar referencias Inspector | Checklist completo del Inspector Setup §7 |

---

## 10. Mobile Considerations

| Elemento | Riesgo | Solución |
|----------|--------|----------|
| Proyectiles (alta frecuencia) | GC allocations, draw calls | `ObjectPool<Projectile>` obligatorio; pool size inicial 20–30 |
| Enemigos (~23 en nivel 10) | Muchos Rigidbody2D activos | `ObjectPool<EnemyController>` por tipo; 8 por tipo = 48 total |
| OverlapCircle en `WeaponController.Update` | Física pesada cada frame | Cache de enemigos cercanos; `Physics2D.OverlapCircleNonAlloc` con array preallocado |
| `EnemyIndicatorController.LateUpdate` | Itera todos los enemigos | Iterar solo los `_trackedEnemies` activos; máximo 8 indicadores visibles |
| Números de daño flotantes | Instantiate en cada hit | `DamageNumberPool` con 20 instancias preallocadas |
| AI OverlapCircle en `EnemyDodgeState` | Física en cada enemigo cada frame | Usar `InvokeRepeating` cada 0.2s en vez de Update; `OverlapCircleNonAlloc` |
| Sprite atlases | Draw calls separados por sprite | Crear atlas: `EnemySprites`, `UISprites`, `ProjectileSprites` en la Sprite Atlas tool |
| `Camera.main` | Búsqueda en cache | Cachear en `Awake` en cualquier script que la use |
| Coordinar rush de Chasers (Tier 4+) | Timer compartido entre instancias | `EnemySpawner` asigna `rushGroupId` al spawnear; Chasers del mismo grupo sincronizan via `GameManager` event |

---

## 11. Compliance Checklist

| Regla | Estado | Detalle |
|-------|--------|---------|
| MonoBehaviours thin | ✅ | Toda la lógica en plain C# (WeaponSlot, StateMachine states) o delegada |
| Sin FindObjectOfType en Update | ✅ | Referencias cacheadas en Awake; singletons via Instance |
| Sin Camera.main en Update | ✅ | Cachear en EnemyIndicatorController.Awake |
| `[SerializeField] private` | ✅ | Definido en todos los campos de Inspector |
| Eventos en OnEnable/OnDisable | ✅ | GameplayHUDController, todos los UI controllers |
| Sin magic numbers | ✅ | RunConfig SO para HealthPackDropChance, WarperSafeZone, etc. |
| TextMeshPro solamente | ✅ | Todos los textos usan TextMeshProUGUI |
| SafeAreaPanel en fullscreen | ✅ | Aplicado al root SafeArea de la Canvas |
| Pooling para spawns frecuentes | ✅ | Proyectiles, enemigos, health packs, damage numbers, SFX sources |
| Canvas Scaler correcto | ✅ | Scale With Screen Size, 1080×1920, Match 0.5 |
| Sin allocations en Update | ⚠️ | OverlapCircleNonAlloc + arrays preallocados en WeaponController y EnemyDodgeState |
| New Input System | ✅ | PlayerInput + InputReader + OnScreenStick |
