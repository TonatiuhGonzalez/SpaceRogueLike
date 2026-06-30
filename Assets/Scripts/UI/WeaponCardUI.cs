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
    [SerializeField] private TextMeshProUGUI _typeBadgeText;
    [SerializeField] private Image _typeBadgeImage;
    [SerializeField] private Button _selectButton;

    [Header("Weapon-only")]
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _fireRateText;
    [SerializeField] private TextMeshProUGUI _rangeText;

    [Header("Badge Colors")]
    [SerializeField] private Color _weaponBadgeColor = new(0.2f, 0.4f, 0.9f);
    [SerializeField] private Color _upgradeBadgeColor = new(0.85f, 0.65f, 0.1f);

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
        if (_typeBadgeText != null) _typeBadgeText.text = "ARMA";
        if (_typeBadgeImage != null) _typeBadgeImage.color = _weaponBadgeColor;

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
        if (_typeBadgeText != null) _typeBadgeText.text = "MEJORA";
        if (_typeBadgeImage != null) _typeBadgeImage.color = _upgradeBadgeColor;

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
}
