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
    private readonly List<EnemyHealth> _trackedEnemies = new();

    public event Action OnLevelCompleted;
    public event Action OnRunCompleted;
    public event Action<int> OnEnemyCountChanged;

    public IReadOnlyList<EnemyHealth> TrackedEnemies => _trackedEnemies;

    private void OnEnable()
    {
        _spawner.OnEnemySpawned += TrackEnemy;
    }

    private void OnDisable()
    {
        _spawner.OnEnemySpawned -= TrackEnemy;
    }

    public void StartLevel()
    {
        foreach (var enemy in _trackedEnemies)
        {
            if (enemy != null)
                enemy.OnDied -= HandleEnemyDied;
        }
        _trackedEnemies.Clear();
        _enemiesAlive = 0;

        int levelIndex = _runData.CurrentLevel - 1;
        _spawner.StartLevel(levelIndex);
    }

    private void TrackEnemy(EnemyHealth health)
    {
        health.OnDied += HandleEnemyDied;
        _trackedEnemies.Add(health);
        _enemiesAlive++;
        OnEnemyCountChanged?.Invoke(_enemiesAlive);
    }

    private void HandleEnemyDied()
    {
        _runData.EnemiesKilled++;
        _enemiesAlive = Mathf.Max(0, _enemiesAlive - 1);
        OnEnemyCountChanged?.Invoke(_enemiesAlive);

        if (_enemiesAlive == 0)
            LevelCompleted();
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in _trackedEnemies)
        {
            if (enemy != null)
            {
                enemy.OnDied -= HandleEnemyDied;
                enemy.gameObject.SetActive(false);
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
