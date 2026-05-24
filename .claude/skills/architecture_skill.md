---
description: Architecture decisions for Unity 2D — when to use MonoBehaviour vs ScriptableObject vs plain C#, GameManager pattern, object pooling, state machines, and service locator.
globs: ["Assets/Scripts/**/*.cs"]
alwaysApply: false
---

# Architecture Skill — Unity 2D

## Decision Tree: What Class Type to Use

```
Does it need to exist in a scene or attach to a GameObject?
  └─ YES → MonoBehaviour
  └─ NO  → Does it hold game data or configuration?
            └─ YES → ScriptableObject
            └─ NO  → Does it need Unity lifecycle (Update, coroutines)?
                      └─ YES → MonoBehaviour
                      └─ NO  → Plain C# class
```

---

## MonoBehaviour

Use when:
- The class needs to exist in a scene (player, enemy, UI controller)
- It needs Unity lifecycle methods (Awake, Update, OnCollision)
- It needs to be attached to a GameObject in the Inspector

Keep thin:
- No hardcoded data — reference a ScriptableObject instead
- No complex logic inline — delegate to plain C# classes
- No cross-system dependencies — use events

---

## ScriptableObject

Use when:
- Storing game data that designers need to tweak (stats, configs, level data)
- Sharing data between multiple objects without coupling
- Implementing the "event channel" pattern

```csharp
// Event channel — decouples publisher from subscriber completely
[CreateAssetMenu(menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private readonly List<GameEventListener> _listeners = new();

    public void Raise()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
            _listeners[i].OnEventRaised();
    }

    public void Register(GameEventListener listener) => _listeners.Add(listener);
    public void Unregister(GameEventListener listener) => _listeners.Remove(listener);
}
```

---

## Plain C# Class

Use when:
- Pure logic with no Unity dependency (math, parsing, state machine logic)
- Models or data structures (inventory item, save data)
- Services that MonoBehaviours call into

```csharp
// Pure logic — no MonoBehaviour needed
public class ScoreCalculator
{
    private readonly ScoreData _data;

    public ScoreCalculator(ScoreData data)
    {
        _data = data;
    }

    public int Calculate(int baseScore, float multiplier, int combo)
    {
        return Mathf.RoundToInt(baseScore * multiplier * (1 + combo * _data.ComboBonus));
    }
}
```

---

## GameManager Pattern

Single entry point for global game state. Use sparingly.

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfig _config;

    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}

public enum GameState { MainMenu, Playing, Paused, GameOver }
```

Rules:
- GameManager holds state and fires events — it does NOT implement game logic
- Other systems subscribe to `OnStateChanged` — they don't poll `CurrentState` in Update
- One GameManager only — do not create one per scene

---

## Object Pool Pattern

For anything spawned frequently: bullets, enemies, particles, score popups.

```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool = new();

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    public T Get()
    {
        var obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    private T CreateNew()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        return obj;
    }
}
```

---

## State Machine Pattern

For entities with clearly defined states (player, enemy AI, game flow).

```csharp
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class StateMachine
{
    private IState _currentState;

    public void SetState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void Update() => _currentState?.Update();
}

// Usage
public class EnemyController : MonoBehaviour
{
    private StateMachine _stateMachine;
    private EnemyData _data;

    private void Awake()
    {
        _stateMachine = new StateMachine();
        _stateMachine.SetState(new IdleState(this, _data));
    }

    private void Update() => _stateMachine.Update();
}
```

---

## Folder Structure

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── SceneLoader.cs
│   └── GameState.cs (enum)
├── Mechanics/
│   ├── Player/
│   │   ├── PlayerController.cs       ← thin MonoBehaviour
│   │   ├── PlayerMovement.cs         ← plain C# logic
│   │   └── States/
│   │       ├── IdleState.cs
│   │       └── RunState.cs
│   └── Enemy/
├── UI/
├── Data/                             ← ScriptableObject definitions
│   ├── PlayerData.cs
│   └── EnemyData.cs
└── Utils/
    ├── ObjectPool.cs
    ├── StateMachine.cs
    └── Extensions.cs
```
