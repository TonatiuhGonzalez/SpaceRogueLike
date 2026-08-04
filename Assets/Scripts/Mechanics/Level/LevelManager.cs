using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EnemySpawner _spawner;
    [SerializeField] private RunData _runData;
    [SerializeField] private RunConfig _runConfig;
    [SerializeField] private AudioData _audioData;

    private int _enemiesAlive;
    private bool _spawningComplete;
    private readonly List<EnemyHealth> _trackedEnemies = new();

    public event Action OnLevelCompleted;
    public event Action OnRunCompleted;
    public event Action<int> OnEnemyCountChanged;

    public IReadOnlyList<EnemyHealth> TrackedEnemies => _trackedEnemies;

    private void OnEnable()
    {
        _spawner.OnEnemySpawned += TrackEnemy;
        _spawner.OnSpawningComplete += HandleSpawningComplete;
    }

    private void OnDisable()
    {
        _spawner.OnEnemySpawned -= TrackEnemy;
        _spawner.OnSpawningComplete -= HandleSpawningComplete;
    }

    public void StartLevel()
    {
        ClearAllEnemies();
        _spawningComplete = false;

        int levelIndex = _runData.CurrentLevel - 1;
        _spawner.StartLevel(levelIndex);
    }

    private void TrackEnemy(EnemyHealth health)
    {
        health.OnDied -= HandleEnemyDied;
        health.OnDied += HandleEnemyDied;

        if (!_trackedEnemies.Contains(health))
            _trackedEnemies.Add(health);

        _enemiesAlive++;
        OnEnemyCountChanged?.Invoke(_enemiesAlive);
    }

    private void HandleEnemyDied()
    {
        _runData.EnemiesKilled++;
        _enemiesAlive = Mathf.Max(0, _enemiesAlive - 1);
        OnEnemyCountChanged?.Invoke(_enemiesAlive);

        if (_enemiesAlive == 0 && _spawningComplete)
            LevelCompleted();
    }

    private void HandleSpawningComplete()
    {
        _spawningComplete = true;
        if (_enemiesAlive == 0)
            LevelCompleted();
    }

    public void ClearAllEnemies()
    {
        var enemiesToKill = new List<EnemyHealth>(_trackedEnemies);
        foreach (var enemy in enemiesToKill)
        {
            if (enemy != null)
            {
                enemy.OnDied -= HandleEnemyDied;
                if (enemy.TryGetComponent<EnemyController>(out var controller))
                    controller.Despawn();
            }
        }
        _trackedEnemies.Clear();
        _enemiesAlive = 0;
    }

    private void LevelCompleted()
    {
        AudioManager.Instance.PlaySFX(_audioData.LevelComplete);

        if (_runData.CurrentLevel >= _runConfig.TotalLevels)
            OnRunCompleted?.Invoke();
        else
            OnLevelCompleted?.Invoke();
    }
}
