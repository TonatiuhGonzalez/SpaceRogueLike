using UnityEngine;

public class EnemyExplosionPool : MonoBehaviour
{
    public static EnemyExplosionPool Instance { get; private set; }

    [SerializeField] private EnemyExplosionEffect _prefab;
    [SerializeField] private int _poolSize = 10;
    [SerializeField] private float _frameDuration = 0.06f;
    [SerializeField] private float _sizeMultiplier = 2f;

    private ObjectPool<EnemyExplosionEffect> _pool;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _pool = new ObjectPool<EnemyExplosionEffect>(_prefab, _poolSize, transform);
    }

    public void SpawnExplosion(Vector2 position, Vector2 size)
    {
        EnemyExplosionEffect effect = _pool.Get();
        effect.Initialize(position, size * _sizeMultiplier, _frameDuration, () => _pool.Return(effect));
    }
}
