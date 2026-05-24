using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private InputReader _inputReader;

    public PlayerHealth Health => _health;
    public WeaponController Weapons => _weaponController;

    public void Initialize(ShipArchetypeData archetype)
    {
        _movement.Initialize(archetype.MoveSpeed);
        _health.Initialize(archetype.MaxHealth);
        _weaponController.SetDamageMultiplier(archetype.DamageMultiplier);
    }
}
