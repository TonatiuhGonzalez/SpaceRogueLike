using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCardUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _fireRateText;
    [SerializeField] private TextMeshProUGUI _rangeText;
    [SerializeField] private TextMeshProUGUI _effectsText;
    [SerializeField] private Button _selectButton;

    private WeaponData _data;
    private Action<WeaponData> _onSelected;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(OnSelectPressed);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(OnSelectPressed);
        _onSelected = null;
    }

    public void Setup(WeaponData data, Action<WeaponData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        _icon.sprite = data.Icon;
        _nameText.text = data.WeaponName;
        _damageText.text = $"DMG: {data.Damage:0}";
        _fireRateText.text = $"RATE: {data.FireRate:0.0}/s";
        _rangeText.text = $"RANGE: {data.Range:0.0}";
        _effectsText.text = data.IsVampiric ? $"Vampiric +{data.VampiricHealAmount:0} HP" : string.Empty;
    }

    private void OnSelectPressed() => _onSelected?.Invoke(_data);
}
