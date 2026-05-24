using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EnemyPool _pool;
    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] private RunConfig _runConfig;
    [SerializeField] private BoxCollider2D _mapBounds;
    [SerializeField] private Transform _playerTransform;

    public event Action<EnemyHealth> OnEnemySpawned;
    public int SpawnedCount { get; private set; }

    public void StartLevel(int levelIndex)
    {
        StopAllCoroutines();
        SpawnedCount = 0;

        int clampedIndex = Mathf.Clamp(levelIndex, 0, _levelConfig.Levels.Length - 1);
        LevelEntry entry = _levelConfig.Levels[clampedIndex];
        int count = UnityEngine.Random.Range(entry.EnemyCountMin, entry.EnemyCountMax + 1);

        StartCoroutine(SpawnRoutine(entry, count));
    }

    private IEnumerator SpawnRoutine(LevelEntry entry, int count)
    {
        float angleStep = count > 0 ? 360f / count : 0f;

        for (int i = 0; i < count; i++)
        {
            EnemyType type = entry.AllowedEnemyTypes[
                UnityEngine.Random.Range(0, entry.AllowedEnemyTypes.Length)];
            Vector2 spawnPos = GetRandomBorderPosition();
            float assignedAngle = angleStep * i;

            SpawnEnemy(type, spawnPos, entry, assignedAngle);

            yield return new WaitForSeconds(_runConfig.SpawnDelayBetweenEnemies);
        }
    }

    private Vector2 GetRandomBorderPosition()
    {
        Bounds bounds = _mapBounds.bounds;
        int side = UnityEngine.Random.Range(0, 4);

        return side switch
        {
            0 => new Vector2(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), bounds.max.y),
            1 => new Vector2(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), bounds.min.y),
            2 => new Vector2(bounds.max.x, UnityEngine.Random.Range(bounds.min.y, bounds.max.y)),
            _ => new Vector2(bounds.min.x, UnityEngine.Random.Range(bounds.min.y, bounds.max.y))
        };
    }

    private void SpawnEnemy(EnemyType type, Vector2 position, LevelEntry entry, float assignedAngle)
    {
        EnemyController enemy = _pool.Get(type);
        if (enemy == null) return;

        enemy.transform.position = position;

        enemy.Initialize(
            enemy.Data, entry.AiTier,
            entry.HpMultiplier, entry.SpeedMultiplier, entry.FireRateMultiplier,
            _playerTransform, _pool, _runConfig, assignedAngle);

        OnEnemySpawned?.Invoke(enemy.Health);
        SpawnedCount++;
    }
}
