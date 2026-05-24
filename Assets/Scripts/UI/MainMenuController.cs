using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private AudioData _audioData;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayPressed);
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayPressed);
    }

    private void OnPlayPressed()
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        ScreenManager.Instance.ShowScreen(GameScreen.ShipSelection);
    }
}
