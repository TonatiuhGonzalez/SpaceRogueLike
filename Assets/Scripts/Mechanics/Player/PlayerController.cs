using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private InputReader _inputReader;

    [Header("Config")]
    [SerializeField] private WeaponData _starterWeapon;

    public PlayerHealth Health => _health;
    public WeaponController Weapons => _weaponController;

    private void Update()
    {
        if (_inputReader.AimInput.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_inputReader.AimInput.y, _inputReader.AimInput.x)
                          * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void SetMovementEnabled(bool enabled) => _movement.SetMovementEnabled(enabled);

    public void Initialize(ShipArchetypeData archetype)
    {
        gameObject.SetActive(true);
        transform.position = Vector3.zero;
        _inputReader.ResetInput();
        _movement.Initialize(archetype.MoveSpeed);
        _health.Initialize(archetype.MaxHealth);
        _weaponController.SetArchetypeMultiplier(archetype.DamageMultiplier);
        _weaponController.ResetWeapons();
        _weaponController.EquipWeapon(_starterWeapon, 0);
    }
}
