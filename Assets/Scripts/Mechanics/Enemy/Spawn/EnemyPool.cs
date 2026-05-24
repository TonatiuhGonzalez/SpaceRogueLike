using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private EnemyController[] _enemyPrefabs;
    [SerializeField] private int _initialPoolSizePerType = 8;

    private ObjectPool<EnemyController>[] _pools;

    private void Awake()
    {
        _pools = new ObjectPool<EnemyController>[_enemyPrefabs.Length];
        for (int i = 0; i < _enemyPrefabs.Length; i++)
        {
            if (_enemyPrefabs[i] != null)
                _pools[i] = new ObjectPool<EnemyController>(
                    _enemyPrefabs[i], _initialPoolSizePerType, transform);
        }
    }

    public EnemyController Get(EnemyType type)
    {
        int index = (int)type;
        if (index < 0 || index >= _pools.Length || _pools[index] == null)
        {
            return null;
        }
        return _pools[index].Get();
    }

    public void Return(EnemyController enemy)
    {
        if (enemy == null) return;
        int index = (int)enemy.Data.Type;
        if (index >= 0 && index < _pools.Length && _pools[index] != null)
            _pools[index].Return(enemy);
    }
}
