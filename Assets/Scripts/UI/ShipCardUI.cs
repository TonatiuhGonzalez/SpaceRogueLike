using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipCardUI : MonoBehaviour
{
    [SerializeField] private Image _shipImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private GameObject _selectedIndicator;

    private ShipArchetypeData _data;
    private Action<ShipArchetypeData> _onSelected;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(OnSelectPressed);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(OnSelectPressed);
    }

    public void Setup(ShipArchetypeData data, Action<ShipArchetypeData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        _shipImage.sprite = data.ShipSprite;
        _nameText.text = data.DisplayName;
        _statsText.text = $"HP: {data.MaxHealth}\nSpeed: {data.MoveSpeed}\nDMG: ×{data.DamageMultiplier:0.0}";
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _selectedIndicator.SetActive(selected);
    }

    private void OnSelectPressed() => _onSelected?.Invoke(_data);
}
