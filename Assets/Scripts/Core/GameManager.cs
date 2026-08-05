using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private RunConfig _runConfig;
    [SerializeField] private RunData _runData;
    [SerializeField] private AudioData _audioData;

    [Header("Scene References")]
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private WeaponSelectionController _weaponSelectionController;
    [SerializeField] private ProjectileManager _projectileManager;
    [SerializeField] private HealthPackPool _healthPackPool;
    [SerializeField] private EnemySpawner _enemySpawner;

    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;
    public event Action<ShipArchetypeData> OnRunStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = GameConstants.TARGET_FRAME_RATE;
        SetState(GameState.MainMenu);
    }

    private void OnEnable()
    {
        if (_levelManager != null)
        {
            _levelManager.OnLevelCompleted += HandleLevelCompleted;
            _levelManager.OnRunCompleted += HandleRunCompleted;
        }
        if (_playerController != null)
            _playerController.Health.OnDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (_levelManager != null)
        {
            _levelManager.OnLevelCompleted -= HandleLevelCompleted;
            _levelManager.OnRunCompleted -= HandleRunCompleted;
        }
        if (_playerController != null)
            _playerController.Health.OnDied -= HandlePlayerDied;
    }

    private void Start()
    {
        ScreenManager.Instance.ShowScreen(GameScreen.MainMenu);
    }

    public void StartNewRun(ShipArchetypeData archetype)
    {
        _runData.ResetForNewRun();
        _runData.SelectedArchetype = archetype;
        _playerController.Initialize(archetype);
        SetState(GameState.Playing);
        ScreenManager.Instance.ShowScreen(GameScreen.Gameplay);
        _levelManager.StartLevel();
        OnRunStarted?.Invoke(archetype);
    }

    private void HandleLevelCompleted()
    {
        _runData.CurrentLevel++;
        _projectileManager.ClearAllProjectiles();
        SetState(GameState.WeaponSelection);
        ScreenManager.Instance.ShowScreen(GameScreen.WeaponSelection);
        _weaponSelectionController.Show();
    }

    private void HandleRunCompleted()
    {
        SetState(GameState.Victory);
        ScreenManager.Instance.ShowScreen(GameScreen.Victory);
        _healthPackPool.ClearAllHealthPacks();
        _playerController.gameObject.SetActive(false);
    }

    private void HandlePlayerDied()
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.Dead);
        _projectileManager.ClearAllProjectiles();
        _enemySpawner.StopAllCoroutines();
        _levelManager.ClearAllEnemies();
        ScreenManager.Instance.ShowScreen(GameScreen.Death);
        AudioManager.Instance.PlaySFX(_audioData.PlayerDeath);
        _healthPackPool.ClearAllHealthPacks();
    }

    public void StartLevel()
    {
        SetState(GameState.Playing);
        _levelManager.StartLevel();
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void QuitRunToMainMenu()
    {
        if (CurrentState != GameState.Paused) return;

        Time.timeScale = 1f;
        _projectileManager.ClearAllProjectiles();
        _enemySpawner.StopAllCoroutines();
        _levelManager.ClearAllEnemies();
        _healthPackPool.ClearAllHealthPacks();
        _playerController.gameObject.SetActive(false);
        SetState(GameState.MainMenu);
        ScreenManager.Instance.ShowScreen(GameScreen.MainMenu);
    }

    private void SetState(GameState newState)
    {
        if (_playerController != null)
        {
            _playerController.SetMovementEnabled(newState == GameState.Playing);
        }

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
