using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private Button _pauseButton;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanelRoot;
    [SerializeField] private GameObject _confirmExitPanelRoot;

    [Header("Panel Buttons")]
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _confirmExitButton;
    [SerializeField] private Button _cancelExitButton;

    [Header("Dependencies")]
    [SerializeField] private GameplayHUDController _gameplayHUDController;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private AudioData _audioData;

    private void OnEnable()
    {
        _pauseButton.onClick.AddListener(OpenPauseMenu);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _exitButton.onClick.AddListener(OnExitPressed);
        _confirmExitButton.onClick.AddListener(OnConfirmExitPressed);
        _cancelExitButton.onClick.AddListener(OnCancelExitPressed);
        _inputReader.OnPauseRequested += HandleBackButtonPressed;
    }

    private void OnDisable()
    {
        _pauseButton.onClick.RemoveListener(OpenPauseMenu);
        _continueButton.onClick.RemoveListener(ClosePauseMenu);
        _exitButton.onClick.RemoveListener(OnExitPressed);
        _confirmExitButton.onClick.RemoveListener(OnConfirmExitPressed);
        _cancelExitButton.onClick.RemoveListener(OnCancelExitPressed);
        _inputReader.OnPauseRequested -= HandleBackButtonPressed;
    }

    private void OpenPauseMenu()
    {
        GameManager.Instance.PauseGame();
        _gameplayHUDController.SetHUDVisible(false);
        _pausePanelRoot.SetActive(true);
        _confirmExitPanelRoot.SetActive(false);
        AudioManager.Instance.PlaySFX(_audioData.PauseOpen);
    }

    private void ClosePauseMenu()
    {
        _pausePanelRoot.SetActive(false);
        _gameplayHUDController.SetHUDVisible(true);
        GameManager.Instance.ResumeGame();
        AudioManager.Instance.PlaySFX(_audioData.PauseClose);
    }

    private void OnExitPressed()
    {
        _pausePanelRoot.SetActive(false);
        _confirmExitPanelRoot.SetActive(true);
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
    }

    private void OnCancelExitPressed()
    {
        _confirmExitPanelRoot.SetActive(false);
        _pausePanelRoot.SetActive(true);
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
    }

    private void OnConfirmExitPressed()
    {
        _gameplayHUDController.SetHUDVisible(true);
        AudioManager.Instance.PlaySFX(_audioData.ExitConfirm);
        GameManager.Instance.QuitRunToMainMenu();
    }

    private void HandleBackButtonPressed()
    {
        if (_confirmExitPanelRoot.activeSelf)
            OnCancelExitPressed();
        else if (_pausePanelRoot.activeSelf)
            ClosePauseMenu();
        else
            OpenPauseMenu();
    }
}
