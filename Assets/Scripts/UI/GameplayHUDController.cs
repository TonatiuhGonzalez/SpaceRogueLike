using UnityEngine;

public class GameplayHUDController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private HealthBarUI _healthBar;

    [Header("Weapon Slots")]
    [SerializeField] private WeaponSlotUI[] _weaponSlotUIs;

    [Header("Dependencies")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private WeaponController _weaponController;

    [Header("Pause Visibility")]
    [SerializeField] private GameObject[] _elementsToHideOnPause;

    private void OnEnable()
    {
        _playerHealth.OnHealthChanged += UpdateHealthBar;
        _weaponController.OnSlotsChanged += UpdateWeaponSlots;
        UpdateWeaponSlots(_weaponController.GetSlots());
    }

    private void OnDisable()
    {
        _playerHealth.OnHealthChanged -= UpdateHealthBar;
        _weaponController.OnSlotsChanged -= UpdateWeaponSlots;
    }

    private void Start()
    {
        UpdateHealthBar(1f);

        if (_weaponController != null)
        {
            for (int i = 0; i < _weaponSlotUIs.Length; i++)
                _weaponSlotUIs[i].SetIndex(i, _weaponController);

            var slots = _weaponController.GetSlots();
            if (slots != null)
                UpdateWeaponSlots(slots);
        }
    }

    public void SetHUDVisible(bool visible)
    {
        for (int i = 0; i < _elementsToHideOnPause.Length; i++)
        {
            if (_elementsToHideOnPause[i] != null)
                _elementsToHideOnPause[i].SetActive(visible);
        }
    }

    private void UpdateHealthBar(float percent)
    {
        _healthBar.SetHealthPercent(percent);
    }

    private void UpdateWeaponSlots(WeaponSlot[] slots)
    {
        for (int i = 0; i < _weaponSlotUIs.Length && i < slots.Length; i++)
        {
            if (slots[i] != null)
                _weaponSlotUIs[i].Refresh(slots[i]);
        }

        for (int i = 0; i < _weaponSlotUIs.Length; i++)
            _weaponSlotUIs[i].SetActive(i == _weaponController.ActiveSlotIndex);
    }
}
