using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionController : MonoBehaviour
{
    [Header("Offer Cards")]
    [SerializeField] private WeaponCardUI[] _offerCards;

    [Header("Current Slot Cards")]
    [SerializeField] private WeaponCardUI[] _currentSlotCards;

    [Header("Controls")]
    [SerializeField] private Button _skipButton;

    [Header("Dependencies")]
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private WeaponData[] _allWeaponData;
    [SerializeField] private WeaponUpgradeData[] _upgradePool;
    [SerializeField] private RunData _runData;
    [SerializeField] private RunConfig _runConfig;
    [SerializeField] private AudioData _audioData;

    private WeaponSelectionLogic _logic;
    private bool _hasSelected;

    private void Awake()
    {
        _logic = new WeaponSelectionLogic(_runConfig);
    }

    private void OnEnable()
    {
        _skipButton.onClick.AddListener(OnSkipPressed);
    }

    private void OnDisable()
    {
        _skipButton.onClick.RemoveListener(OnSkipPressed);
    }

    public void Show()
    {
        _hasSelected = false;

        List<WeaponData> available = GetAvailableWeapons();
        List<WeaponType> ownedTypes = _weaponController.GetOwnedWeaponTypes();

        SelectionOffer[] offers = _logic.BuildOffers(
            _runData.CurrentLevel,
            available,
            ownedTypes,
            _upgradePool,
            _runData.Upgrades);

        for (int i = 0; i < _offerCards.Length && i < offers.Length; i++)
            _offerCards[i].Setup(offers[i], OnOfferChosen);

        WeaponSlot[] slots = _weaponController.GetSlots();
        for (int i = 0; i < _currentSlotCards.Length && i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty)
                _currentSlotCards[i].SetupCurrentWeapon(slots[i].EquippedWeapon);
            else
                _currentSlotCards[i].SetupEmpty();
        }
    }

    private List<WeaponData> GetAvailableWeapons()
    {
        var available = new List<WeaponData>();
        foreach (WeaponData weapon in _allWeaponData)
        {
            if (!_runData.OwnedWeaponTypes.Contains(weapon.WeaponType))
                available.Add(weapon);
        }
        return available;
    }

    private void OnOfferChosen(SelectionOffer offer)
    {
        if (_hasSelected || offer == null) return;
        _hasSelected = true;

        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);

        if (offer.Type == OfferType.Weapon)
        {
            int slot = _weaponController.FindFirstEmptySlot();
            if (slot < 0) return;

            _weaponController.EquipWeapon(offer.WeaponData, slot);
            _runData.AddOwnedWeapon(offer.WeaponData.WeaponType);
        }
        else
        {
            WeaponUpgradeData upgrade = offer.UpgradeData;
            if (upgrade.Category == UpgradeCategory.Generic)
                _weaponController.ApplyGenericUpgrade(upgrade.Stat);
            else
                _weaponController.ApplySpecificUpgrade(upgrade.TargetWeaponType);
        }

        EquipAndContinue();
    }

    private void OnSkipPressed()
    {
        if (_hasSelected) return;
        _hasSelected = true;

        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        EquipAndContinue();
    }

    private void EquipAndContinue()
    {
        ScreenManager.Instance.ShowScreen(GameScreen.Gameplay);
        GameManager.Instance.StartLevel();
    }
}
