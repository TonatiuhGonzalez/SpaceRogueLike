using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCardUI : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _effectsText;
    [SerializeField] private Image _typeBadgeImage;
    [SerializeField] private Button _selectButton;

    [Header("Weapon-only")]
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _fireRateText;
    [SerializeField] private TextMeshProUGUI _rangeText;

    [Header("Badge Icons")]
    [SerializeField] private Sprite _weaponTypeIcon;
    [SerializeField] private Sprite _genericUpgradeIcon;
    [SerializeField] private Sprite _specificUpgradeIcon;

    private SelectionOffer _offer;
    private Action<SelectionOffer> _onSelected;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(OnSelectPressed);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(OnSelectPressed);
        _onSelected = null;
        _offer = null;
    }

    public void Setup(SelectionOffer offer, Action<SelectionOffer> onSelected)
    {
        _offer = offer;
        _onSelected = onSelected;

        if (offer == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (offer.Type == OfferType.Weapon)
            SetupWeapon(offer.WeaponData);
        else
            SetupUpgrade(offer.UpgradeData);
    }

    private void SetupWeapon(WeaponData data)
    {
        if (_icon != null)       _icon.sprite = data.Icon;
        if (_nameText != null)   _nameText.text = data.WeaponName;
        if (_damageText != null) _damageText.text = $"DMG: {data.Damage:0}";
        if (_fireRateText != null) _fireRateText.text = $"RATE: {data.FireRate:0.0}/s";
        if (_rangeText != null)  _rangeText.text = $"RANGE: {data.Range:0.0}";
        if (_effectsText != null) _effectsText.text = BuildWeaponEffectText(data);
        if (_typeBadgeImage != null) _typeBadgeImage.sprite = _weaponTypeIcon;

        SetWeaponStatsVisible(true);
    }

    private void SetupUpgrade(WeaponUpgradeData data)
    {
        if (_icon != null)     _icon.sprite = data.Icon;
        if (_nameText != null) _nameText.text = data.UpgradeName;

        string desc = data.Description;
        if (data.Category == UpgradeCategory.Specific)
            desc += $"\nPara: {data.TargetWeaponType}";

        if (_effectsText != null) _effectsText.text = desc;
        if (_typeBadgeImage != null)
            _typeBadgeImage.sprite = data.Category == UpgradeCategory.Generic
                ? _genericUpgradeIcon
                : _specificUpgradeIcon;

        SetWeaponStatsVisible(false);
    }

    private void SetWeaponStatsVisible(bool visible)
    {
        if (_damageText != null)   _damageText.gameObject.SetActive(visible);
        if (_fireRateText != null) _fireRateText.gameObject.SetActive(visible);
        if (_rangeText != null)    _rangeText.gameObject.SetActive(visible);
    }

    private static string BuildWeaponEffectText(WeaponData data)
    {
        return data.WeaponType switch
        {
            WeaponType.Dispersion => $"Cono {data.ConeAngleDegrees:0}°",
            WeaponType.Fan        => $"{data.BulletCount} balas en abanico",
            WeaponType.Slow       => "Proyectil lento, alto daño",
            WeaponType.Area       => $"Explosión radio {data.ExplosionRadius:0}",
            WeaponType.Zapper     => $"Cadena {data.ChainCount} enemigos",
            WeaponType.Vampiric   => $"Cura {data.VampiricHealPercent * 100f:0}% del daño",
            WeaponType.Poison     => $"Veneno {data.PoisonTickPercent * 100f:0}% por tick",
            _                     => string.Empty,
        };
    }

    private void OnSelectPressed() => _onSelected?.Invoke(_offer);

    public void SetupCurrentWeapon(WeaponData weapon)
    {
        if (_icon != null)       _icon.sprite = weapon.Icon;
        if (_nameText != null)   _nameText.text = weapon.WeaponName;
        if (_damageText != null) _damageText.text = $"Damage: {weapon.Damage}";
        if (_fireRateText != null) _fireRateText.text = $"Fire Rate: {weapon.FireRate}";
        if (_rangeText != null)  _rangeText.text = $"Range: {weapon.Range}";
        if (_selectButton != null) _selectButton.gameObject.SetActive(false);
    }

    public void SetupEmpty()
    {
        if (_nameText != null)   _nameText.text = "Empty Slot";
        if (_icon != null)       _icon.sprite = null;
        if (_damageText != null) _damageText.text = "";
        if (_fireRateText != null) _fireRateText.text = "";
        if (_rangeText != null)  _rangeText.text = "";
        if (_selectButton != null) _selectButton.gameObject.SetActive(false);
    }
}
