---
description: Mobile optimization standard for this Unity 2D project — performance rules, draw calls, sprite atlases, object pooling, memory management, and build settings for Android/iOS.
globs: ["Assets/Scripts/**/*.cs"]
alwaysApply: false
---

# Mobile Skill — Optimization for Android/iOS

## Performance Targets

| Metric | Target |
|--------|--------|
| Frame rate | 60 fps stable |
| Draw calls | < 50 per frame |
| Memory (runtime) | < 200 MB |
| Startup time | < 3 seconds |

---

## Draw Call Rules

- **Sprite Atlas**: group sprites used together into atlases
  - UI sprites → one atlas per screen or UI section
  - Gameplay sprites → one atlas per category (enemies, projectiles, effects)
- Enable GPU Instancing on materials used by many identical objects
- Set sprites to the same layer and sorting layer to enable batching
- Avoid mixing Sprite Renderer and UI Image on the same Canvas

---

## Object Pooling (mandatory for frequent spawns)

Any object spawned more than once per second must use a pool:

```csharp
// Manager that owns the pool
public class BulletManager : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _initialPoolSize = 20;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(_bulletPrefab, _initialPoolSize, transform);
    }

    public Bullet SpawnBullet(Vector2 position, Vector2 direction)
    {
        var bullet = _pool.Get();
        bullet.transform.position = position;
        bullet.Initialize(direction, () => _pool.Return(bullet));
        return bullet;
    }
}

// Poolable object — calls return callback when done
public class Bullet : MonoBehaviour
{
    private Action _onReturn;

    public void Initialize(Vector2 direction, Action onReturn)
    {
        _onReturn = onReturn;
        // set velocity, etc.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onReturn?.Invoke();
    }
}
```

---

## Update Loop Rules

```csharp
// ❌ Never — allocation and search every frame
private void Update()
{
    var enemy = FindObjectOfType<Enemy>();
    var cam = Camera.main;
    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
}

// ✅ Cache in Awake, use cached references
private Enemy _nearestEnemy;
private Camera _camera;
private Rigidbody2D _rb;

private void Awake()
{
    _camera = Camera.main;
    _rb = GetComponent<Rigidbody2D>();
}
```

Heavy operations that don't need to run every frame:
- Use coroutines with `WaitForSeconds`
- Use `InvokeRepeating` for fixed-interval checks
- Use events instead of polling

---

## Memory Management

- Unload unused assets when changing scenes: `Resources.UnloadUnusedAssets()`
- Avoid large `Resources/` folder — load assets via direct references or Addressables
- Texture import settings for sprites: Compression = ASTC (best quality/size for mobile)
- Max texture size: 1024×1024 for gameplay sprites, 2048×2048 for backgrounds
- Mipmaps: disabled for UI sprites, enabled for 3D or large world sprites

---

## Audio Compression (mobile)

| Audio type | Format | Load type |
|-----------|--------|-----------|
| Background music | AAC / Vorbis, 128kbps | Streaming |
| Short SFX (< 1s) | PCM (uncompressed) | Decompress On Load |
| Medium SFX (1-5s) | AAC / Vorbis, 96kbps | Compressed In Memory |

---

## Physics Rules (2D)

- Use `Rigidbody2D` Collision Detection: Continuous only for fast-moving objects (bullets)
- Use `Rigidbody2D` Sleeping Mode: Start Asleep for static or rarely moving objects
- Layer collision matrix: disable collisions between layers that never interact
- Use `OverlapCircle` / `OverlapBox` instead of continuous collision when possible

---

## Build Settings

**Android:**
- Scripting Backend: IL2CPP
- Target Architecture: ARM64 + ARMv7
- Minify: Full (Release)
- Graphics API: Vulkan + OpenGLES3

**iOS:**
- Scripting Backend: IL2CPP
- Target minimum iOS: 13.0
- Architecture: ARM64

---

## Profiling Checklist (run before any review)

- [ ] Frame rate stable at 60 fps on a mid-range device
- [ ] Draw calls < 50 in gameplay scene (Unity Profiler → Rendering)
- [ ] No GC allocations in Update loops (Profiler → Memory)
- [ ] No `FindObjectOfType` or `Camera.main` in hot paths
- [ ] All frequently spawned objects use object pools
- [ ] Sprite atlases configured for UI and gameplay sprites
- [ ] Audio compression settings correct per audio type
