using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private ProjectileManager _projectileManager;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private AudioData _audioData;
    [SerializeField] private RunData _runData;
    [SerializeField] private RunConfig _runConfig;

    [Header("Settings")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _projectileSpawnOffset = 0.5f;

#if UNITY_EDITOR
    [Header("Debug")]
    [Tooltip("Select a weapon here in Play Mode to test its shots immediately, without waiting for a level-up offer.")]
    [SerializeField] private WeaponData _debugWeaponOverride;
    private WeaponData _lastDebugWeaponOverride;
#endif

    private const int MAX_SLOTS = 3;
    private readonly WeaponSlot[] _slots = new WeaponSlot[MAX_SLOTS];
    private float _archetypeMultiplier = 1f;
    private int _activeSlotIndex = 0;

    public event Action<WeaponSlot[]> OnSlotsChanged;

    public void SetArchetypeMultiplier(float multiplier) => _archetypeMultiplier = multiplier;

    private void Awake()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            _slots[i] = new WeaponSlot();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (_debugWeaponOverride == _lastDebugWeaponOverride) return;
        _lastDebugWeaponOverride = _debugWeaponOverride;
        if (_debugWeaponOverride == null) return;

        EquipWeapon(_debugWeaponOverride, 0);
        SetActiveSlot(0);
    }
#endif

    public void EquipWeapon(WeaponData weapon, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS) return;
        _slots[slotIndex].Equip(weapon);
        OnSlotsChanged?.Invoke(_slots);
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS) return;
        _slots[slotIndex].Clear();
        OnSlotsChanged?.Invoke(_slots);
    }

    public void ResetWeapons()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            _slots[i].Clear();
        _activeSlotIndex = 0;
        OnSlotsChanged?.Invoke(_slots);
    }

    public WeaponSlot[] GetSlots() => _slots;

    public void SetActiveSlot(int index)
    {
        if (index < 0 || index >= MAX_SLOTS) return;
        if (_slots[index].IsEmpty) return;
        _activeSlotIndex = index;
        OnSlotsChanged?.Invoke(_slots);
    }

    public int ActiveSlotIndex => _activeSlotIndex;

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            if (_slots[i].IsEmpty) return i;
        return -1;
    }

    public List<WeaponType> GetOwnedWeaponTypes()
    {
        var types = new List<WeaponType>();
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (!_slots[i].IsEmpty)
                types.Add(_slots[i].EquippedWeapon.WeaponType);
        }
        return types;
    }

    public void ApplyGenericUpgrade(GenericStat stat)
    {
        _runData.Upgrades.ApplyGenericUpgrade(stat);
    }

    public void ApplySpecificUpgrade(WeaponType type)
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (!_slots[i].IsEmpty && _slots[i].EquippedWeapon.WeaponType == type)
            {
                _slots[i].ApplySpecificUpgrade();
                return;
            }
        }
    }

    private void Update()
    {
        float fireRateMult = _runData != null
            ? _runData.Upgrades.GetGenericMultiplier(GenericStat.FireRate)
            : 1f;

        WeaponSlot activeSlot = _slots[_activeSlotIndex];
        if (activeSlot.IsEmpty) return;

        activeSlot.Tick(Time.deltaTime);
        if (activeSlot.IsReady)
            TryFire(activeSlot, fireRateMult);
    }

    private void TryFire(WeaponSlot slot, float fireRateMult)
    {
        Vector2 aimDir = _inputReader.AimInput;
        if (aimDir.sqrMagnitude < 0.01f) return;
        aimDir = aimDir.normalized;

        WeaponData weapon = slot.EquippedWeapon;
        UpgradeRegistry upgrades = _runData?.Upgrades;

        float damage = weapon.Damage *
            (upgrades?.GetGenericMultiplier(GenericStat.Damage) ?? 1f) *
            _archetypeMultiplier;
        float bulletSizeMult =
            upgrades?.GetGenericMultiplier(GenericStat.BulletSize) ?? 1f;

        Action<float> onHeal = weapon.IsVampiric
            ? (amount) => _playerHealth.Heal(amount)
            : null;

        switch (weapon.WeaponType)
        {
            case WeaponType.Fan:
                FireFan(slot, aimDir, damage, bulletSizeMult, onHeal);
                break;

            case WeaponType.Dispersion:
                FireDispersion(slot, aimDir, damage, bulletSizeMult, onHeal);
                break;

            default:
                FireSingle(slot, aimDir, damage, bulletSizeMult, onHeal);
                break;
        }

        AudioManager.Instance.PlaySFX(weapon.FireSFX ?? _audioData.ShootDefault);
        slot.StartCooldown(fireRateMult);
    }

    private Vector2 GetMuzzlePosition(Vector2 direction) =>
        (Vector2)transform.position + direction * _projectileSpawnOffset;

    private void FireSingle(WeaponSlot slot, Vector2 direction, float damage,
        float bulletSizeMult, Action<float> onHeal)
    {
        _projectileManager.Spawn(
            slot.EquippedWeapon.ProjectilePrefab,
            GetMuzzlePosition(direction),
            BuildConfig(slot, direction, damage, bulletSizeMult, onHeal));
    }

    private void FireDispersion(WeaponSlot slot, Vector2 aimDir, float damage,
        float bulletSizeMult, Action<float> onHeal)
    {
        float halfCone = slot.EffectiveConeAngle * 0.5f;
        float deviation = UnityEngine.Random.Range(-halfCone, halfCone);
        Vector2 direction = Quaternion.Euler(0f, 0f, deviation) * aimDir;

        _projectileManager.Spawn(
            slot.EquippedWeapon.ProjectilePrefab,
            GetMuzzlePosition(direction),
            BuildConfig(slot, direction, damage, bulletSizeMult, onHeal));
    }

    private void FireFan(WeaponSlot slot, Vector2 aimDir, float damage,
        float bulletSizeMult, Action<float> onHeal)
    {
        int count = slot.EffectiveBulletCount;
        float totalSpread = slot.EquippedWeapon.FanSpreadAngle;
        float step = count > 1 ? totalSpread / (count - 1) : 0f;
        float startAngle = -totalSpread * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            Vector2 dir = Quaternion.Euler(0f, 0f, angle) * aimDir;
            _projectileManager.Spawn(
                slot.EquippedWeapon.ProjectilePrefab,
                GetMuzzlePosition(dir),
                BuildConfig(slot, dir, damage, bulletSizeMult, onHeal));
        }
    }

    private ProjectileConfig BuildConfig(WeaponSlot slot, Vector2 direction,
        float damage, float bulletSizeMult, Action<float> onHeal)
    {
        WeaponData weapon = slot.EquippedWeapon;
        UpgradeRegistry upgrades = _runData?.Upgrades;
        int poisonMaxStacks = _runConfig != null ? _runConfig.PoisonMaxStacks : 8;
        float zapRadius = _runConfig != null ? _runConfig.ZapperChainSearchRadius : 15f;
        float finalBulletSizeMult = weapon.ProjectileScale * bulletSizeMult;

        return new ProjectileConfig
        {
            Direction              = direction,
            Speed                  = weapon.ProjectileSpeed,
            Damage                 = damage,
            TargetLayer            = _enemyLayer,
            BulletSizeMultiplier   = finalBulletSizeMult,
            ProjectileSprite       = weapon.ProjectileSprite,
            VampiricHealPercent    = weapon.IsVampiric ? slot.EffectiveVampiricPercent : 0f,
            OnHealPlayer           = onHeal,
            ExplosionRadius        = weapon.WeaponType == WeaponType.Area
                                        ? slot.EffectiveExplosionRadius : 0f,
            ExplosionDamagePercent = weapon.ExplosionDamagePercent,
            ChainCount             = weapon.WeaponType == WeaponType.Zapper
                                        ? slot.EffectiveChainCount : 0,
            ChainDamagePercents    = weapon.ChainDamagePercents,
            ChainSearchRadius      = zapRadius,
            ChainDelay             = weapon.ChainDelay,
            ProjectileManager      = _projectileManager,
            PoisonTickPercent      = weapon.WeaponType == WeaponType.Poison
                                        ? slot.EffectivePoisonTickPercent : 0f,
            PoisonMode             = slot.EffectivePoisonMode,
            PoisonMaxStacks        = poisonMaxStacks,
        };
    }

}
