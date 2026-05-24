using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelReachedText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private RunData _runData;
    [SerializeField] private AudioData _audioData;

    private void OnEnable()
    {
        _retryButton.onClick.AddListener(OnRetryPressed);
        _menuButton.onClick.AddListener(OnMenuPressed);
        _levelReachedText.text = $"Destroyed at Level {_runData.CurrentLevel}";
    }

    private void OnDisable()
    {
        _retryButton.onClick.RemoveListener(OnRetryPressed);
        _menuButton.onClick.RemoveListener(OnMenuPressed);
    }

    private void OnRetryPressed()
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        ScreenManager.Instance.ShowScreen(GameScreen.ShipSelection);
    }

    private void OnMenuPressed()
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        ScreenManager.Instance.ShowScreen(GameScreen.MainMenu);
    }
}
