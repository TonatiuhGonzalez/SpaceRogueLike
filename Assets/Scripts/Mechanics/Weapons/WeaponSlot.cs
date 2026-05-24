public class WeaponSlot
{
    public WeaponData EquippedWeapon { get; private set; }
    public bool IsEmpty => EquippedWeapon == null;
    public bool IsReady => !IsEmpty && _cooldownRemaining <= 0f;

    private float _cooldownRemaining;

    public void Equip(WeaponData weapon)
    {
        EquippedWeapon = weapon;
        _cooldownRemaining = 0f;
    }

    public void Clear()
    {
        EquippedWeapon = null;
        _cooldownRemaining = 0f;
    }

    public void Tick(float deltaTime)
    {
        if (_cooldownRemaining > 0f)
            _cooldownRemaining -= deltaTime;
    }

    public void StartCooldown()
    {
        _cooldownRemaining = EquippedWeapon != null && EquippedWeapon.FireRate > 0f
            ? 1f / EquippedWeapon.FireRate
            : 1f;
    }
}
