using UnityEngine;
using UnityEngine.UI;

public class ShipSelectionController : MonoBehaviour
{
    [SerializeField] private ShipCardUI[] _cards;
    [SerializeField] private ShipArchetypeData[] _archetypes;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private AudioData _audioData;

    private ShipArchetypeData _selected;

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(OnConfirmPressed);
        _backButton.onClick.AddListener(OnBackPressed);
        _confirmButton.interactable = false;

        for (int i = 0; i < _cards.Length && i < _archetypes.Length; i++)
            _cards[i].Setup(_archetypes[i], OnCardSelected);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(OnConfirmPressed);
        _backButton.onClick.RemoveListener(OnBackPressed);
        _selected = null;
    }

    private void OnCardSelected(ShipArchetypeData archetype)
    {
        _selected = archetype;
        _confirmButton.interactable = true;
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);

        foreach (var card in _cards)
            card.SetSelected(false);

        for (int i = 0; i < _cards.Length && i < _archetypes.Length; i++)
        {
            if (_archetypes[i] == archetype)
                _cards[i].SetSelected(true);
        }
    }

    private void OnConfirmPressed()
    {
        if (_selected == null) return;
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        GameManager.Instance.StartNewRun(_selected);
    }

    private void OnBackPressed()
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        ScreenManager.Instance.ShowScreen(GameScreen.MainMenu);
    }
}
