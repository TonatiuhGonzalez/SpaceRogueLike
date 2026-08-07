using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private AudioData _audioData;

    private void OnEnable()
    {
        _continueButton.interactable = true;
        _continueButton.onClick.AddListener(OnContinuePressed);
    }

    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(OnContinuePressed);
    }

    private void OnContinuePressed()
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        _continueButton.interactable = false;
        ScreenManager.Instance.ShowScreen(GameScreen.ShipSelection);
    }
}
