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
        OnRunStarted?.Invoke(archetype);
        SetState(GameState.WeaponSelection);
        ScreenManager.Instance.ShowScreen(GameScreen.WeaponSelection);
        _weaponSelectionController.Show();
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
    }

    private void HandlePlayerDied()
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.Dead);
        _projectileManager.ClearAllProjectiles();
        _levelManager.ClearAllEnemies();
        ScreenManager.Instance.ShowScreen(GameScreen.Death);
        AudioManager.Instance.PlaySFX(_audioData.PlayerDeath);
    }

    public void StartLevel()
    {
        SetState(GameState.Playing);
        _levelManager.StartLevel();
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
