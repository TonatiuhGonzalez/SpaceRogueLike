---
description: Unity C# coding standard — naming conventions, MonoBehaviour structure, ScriptableObject patterns, and Unity-specific rules for this 2D mobile project.
globs: ["Assets/Scripts/**/*.cs"]
alwaysApply: false
---

# Unity Standard — C# + Unity 2D

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes | PascalCase | `PlayerController`, `EnemySpawner` |
| Methods | PascalCase | `TakeDamage()`, `ResetPosition()` |
| Properties | PascalCase | `IsAlive`, `CurrentHealth` |
| Private fields | `_camelCase` | `_currentSpeed`, `_isGrounded` |
| Public fields | PascalCase | `MaxHealth` |
| Constants | UPPER_SNAKE_CASE | `MAX_POOL_SIZE`, `BASE_SPEED` |
| Interfaces | `I` prefix | `IDamageable`, `IPoolable` |
| Events | PascalCase, past tense | `OnPlayerDied`, `OnScoreChanged` |
| Enums | PascalCase | `GameState`, `EnemyType` |
| Enum values | PascalCase | `GameState.Playing`, `GameState.Paused` |

---

## MonoBehaviour Structure

Always in this order:

```csharp
public class PlayerController : MonoBehaviour
{
    // 1. Serialized fields (Inspector)
    [SerializeField] private PlayerData _data;
    [SerializeField] private Rigidbody2D _rb;

    // 2. Private fields
    private float _currentHealth;
    private bool _isAlive;

    // 3. Properties
    public bool IsAlive => _isAlive;
    public float HealthPercent => _currentHealth / _data.MaxHealth;

    // 4. Unity lifecycle — in order
    private void Awake()      { /* Initialize own state */ }
    private void OnEnable()   { /* Subscribe to events */ }
    private void Start()      { /* Cross-reference other components */ }
    private void Update()     { /* Per-frame logic */ }
    private void FixedUpdate(){ /* Physics */ }
    private void OnDisable()  { /* Unsubscribe from events */ }
    private void OnDestroy()  { /* Cleanup */ }

    // 5. Public methods
    public void TakeDamage(float amount) { }

    // 6. Private methods
    private void Die() { }
}
```

---

## Inspector Field Rules

```csharp
// ✅ SerializeField for Inspector exposure
[SerializeField] private float _speed = 5f;
[SerializeField] private Transform _spawnPoint;

// ✅ Header for grouping in Inspector
[Header("Movement")]
[SerializeField] private float _moveSpeed;
[SerializeField] private float _jumpForce;

[Header("Combat")]
[SerializeField] private float _attackDamage;

// ✅ Range for sliders
[Range(0f, 1f)]
[SerializeField] private float _volume;

// ❌ Never public just for Inspector
public float speed; // Wrong
```

---

## ScriptableObject Pattern

Use ScriptableObjects for all game data and configuration:

```csharp
// Definition — in Assets/Scripts/Data/
[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Stats")]
    public float MaxHealth = 100f;
    public float MoveSpeed = 5f;
    public float JumpForce = 10f;

    [Header("Combat")]
    public float AttackDamage = 20f;
    public float AttackCooldown = 0.5f;
}

// Usage — reference via Inspector
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _data;

    private void Awake()
    {
        _currentHealth = _data.MaxHealth;
    }
}
```

Instances live in `Assets/ScriptableObjects/` — not in `Assets/Scripts/`.

---

## Event Pattern

Prefer C# events or UnityEvents for loose coupling:

```csharp
// Publisher
public class PlayerHealth : MonoBehaviour
{
    public event Action<float> OnHealthChanged;
    public event Action OnDied;

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);

        if (_currentHealth <= 0)
            OnDied?.Invoke();
    }
}

// Subscriber
public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;

    private void OnEnable()
    {
        _playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        _playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float percent) { }
}
```

---

## Rules

- No `FindObjectOfType` or `GameObject.Find` in `Update` — cache references in `Awake`
- No `Camera.main` in `Update` — cache in `Awake`: `_camera = Camera.main`
- No magic numbers — use ScriptableObjects or named constants
- No business logic in MonoBehaviours — delegate to plain C# classes when possible
- Always unsubscribe from events in `OnDisable` or `OnDestroy`
- Use `[SerializeField] private` instead of `public` for Inspector fields
- Null check with `?.` operator before invoking events: `OnDied?.Invoke()`
- `Awake`: initialize own state. `Start`: reference other components.
