using UnityEngine;

public class HealthPackPool : MonoBehaviour
{
    [SerializeField] private HealthPack _prefab;
    [SerializeField] private RunConfig _runConfig;
    [SerializeField] private int _poolSize = 10;
    [SerializeField] private AudioData _audioData;

    private ObjectPool<HealthPack> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<HealthPack>(_prefab, _poolSize, transform);
    }

    public void TrySpawnAt(Vector2 position)
    {
        if (Random.value > _runConfig.HealthPackDropChance) return;

        HealthPack pack = _pool.Get();
        pack.Initialize(position, () => _pool.Return(pack));
        AudioManager.Instance.PlaySFX(_audioData.HealthPackDrop);
    }
}
