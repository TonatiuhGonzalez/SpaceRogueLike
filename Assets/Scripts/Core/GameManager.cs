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
        SetState(GameState.Playing);
        ScreenManager.Instance.ShowScreen(GameScreen.Gameplay);
        _levelManager.StartLevel();
        OnRunStarted?.Invoke(archetype);
    }

    private void HandleLevelCompleted()
    {
        _runData.CurrentLevel++;
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
        ScreenManager.Instance.ShowScreen(GameScreen.Death);
        AudioManager.Instance.PlaySFX(_audioData.PlayerDeath);
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
