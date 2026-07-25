using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private Button _menuButton;
    [SerializeField] private RunData _runData;
    [SerializeField] private AudioData _audioData;

    private void OnEnable()
    {
        _menuButton.onClick.AddListener(OnMenuPressed);
        _statsText.text = $"Run Complete!\n" +
                          $"Levels: {_runData.CurrentLevel}\n" +
                          $"Enemies destroyed: {_runData.EnemiesKilled}";
    }

    private void OnDisable()
    {
        _menuButton.onClick.RemoveListener(OnMenuPressed);
    }

    private void OnMenuPressed()
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        ScreenManager.Instance.ShowScreen(GameScreen.MainMenu);
    }
}
