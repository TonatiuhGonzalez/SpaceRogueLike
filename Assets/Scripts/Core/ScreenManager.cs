using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _shipSelectionPanel;
    [SerializeField] private GameObject _gameplayHUDPanel;
    [SerializeField] private GameObject _weaponSelectionPanel;
    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _victoryPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowScreen(GameScreen screen)
    {
        _mainMenuPanel.SetActive(screen == GameScreen.MainMenu);
        _shipSelectionPanel.SetActive(screen == GameScreen.ShipSelection);
        _gameplayHUDPanel.SetActive(screen == GameScreen.Gameplay);
        _weaponSelectionPanel.SetActive(screen == GameScreen.WeaponSelection);
        _deathPanel.SetActive(screen == GameScreen.Death);
        _victoryPanel.SetActive(screen == GameScreen.Victory);
    }
}
