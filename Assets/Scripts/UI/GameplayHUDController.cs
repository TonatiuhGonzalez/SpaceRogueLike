using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayHUDController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider _healthBar;

    [Header("Level Info")]
    [SerializeField] private TextMeshProUGUI _enemyCountText;
    [SerializeField] private TextMeshProUGUI _levelText;

    [Header("Weapon Slots")]
    [SerializeField] private WeaponSlotUI[] _weaponSlotUIs;

    [Header("Dependencies")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private RunData _runData;

    private void OnEnable()
    {
        _playerHealth.OnHealthChanged += UpdateHealthBar;
        _levelManager.OnEnemyCountChanged += UpdateEnemyCount;
        _weaponController.OnSlotsChanged += UpdateWeaponSlots;
    }

    private void OnDisable()
    {
        _playerHealth.OnHealthChanged -= UpdateHealthBar;
        _levelManager.OnEnemyCountChanged -= UpdateEnemyCount;
        _weaponController.OnSlotsChanged -= UpdateWeaponSlots;
    }

    private void Start()
    {
        UpdateHealthBar(_playerHealth.HealthPercent);
        UpdateWeaponSlots(_weaponController.GetSlots());
    }

    private void UpdateHealthBar(float percent)
    {
        _healthBar.value = percent;
    }

    private void UpdateEnemyCount(int count)
    {
        _enemyCountText.text = count == 1 ? "1 enemy left" : $"{count} enemies left";
        _levelText.text = $"Level {_runData.CurrentLevel}";
    }

    private void UpdateWeaponSlots(WeaponSlot[] slots)
    {
        for (int i = 0; i < _weaponSlotUIs.Length && i < slots.Length; i++)
            _weaponSlotUIs[i].Refresh(slots[i]);
    }
}
