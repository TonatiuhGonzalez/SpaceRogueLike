using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButtonUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private AudioChannel _channel;

    [Header("Visual States")]
    [SerializeField] private GameObject _onStateVisual;
    [SerializeField] private GameObject _offStateVisual;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
        RefreshVisual(GetCurrentState());
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        bool newState = !GetCurrentState();
        SetState(newState);
        RefreshVisual(newState);
    }

    private bool GetCurrentState()
    {
        return _channel == AudioChannel.Music
            ? AudioManager.Instance.IsMusicEnabled
            : AudioManager.Instance.IsSFXEnabled;
    }

    private void SetState(bool enabled)
    {
        if (_channel == AudioChannel.Music)
            AudioManager.Instance.SetMusicEnabled(enabled);
        else
            AudioManager.Instance.SetSFXEnabled(enabled);
    }

    private void RefreshVisual(bool enabled)
    {
        _onStateVisual.SetActive(enabled);
        _offStateVisual.SetActive(!enabled);
    }
}
