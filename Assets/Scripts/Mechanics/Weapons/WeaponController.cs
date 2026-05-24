using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private ProjectileManager _projectileManager;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private AudioData _audioData;

    [Header("Settings")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _aimConeAngle = 30f;

    private const int MAX_SLOTS = 3;
    private readonly WeaponSlot[] _slots = new WeaponSlot[MAX_SLOTS];
    private float _damageMultiplier = 1f;

    public event Action<WeaponSlot[]> OnSlotsChanged;

    private void Awake()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            _slots[i] = new WeaponSlot();
    }

    public void SetDamageMultiplier(float multiplier) => _damageMultiplier = multiplier;

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

    public WeaponSlot[] GetSlots() => _slots;

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            if (_slots[i].IsEmpty) return i;
        return -1;
    }

    private void Update()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            _slots[i].Tick(Time.deltaTime);
            if (_slots[i].IsReady)
                TryFire(_slots[i]);
        }
    }

    private void TryFire(WeaponSlot slot)
    {
        EnemyHealth target = FindTargetInAimDirection(slot.EquippedWeapon.Range);
        if (target == null) return;

        Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        float damage = slot.EquippedWeapon.Damage * _damageMultiplier;

        Action<float> onVampiricHeal = slot.EquippedWeapon.IsVampiric
            ? (amount) => _playerHealth.Heal(amount)
            : null;

        _projectileManager.Spawn(
            slot.EquippedWeapon.ProjectilePrefab,
            transform.position,
            direction,
            slot.EquippedWeapon.ProjectileSpeed,
            damage,
            _enemyLayer,
            slot.EquippedWeapon.IsVampiric,
            slot.EquippedWeapon.VampiricHealAmount,
            onVampiricHeal);

        AudioManager.Instance.PlaySFX(
            slot.EquippedWeapon.FireSFX ?? _audioData.ShootDefault);

        slot.StartCooldown();
    }

    private EnemyHealth FindTargetInAimDirection(float range)
    {
        Vector2 aimDir = _inputReader.AimInput;
        if (aimDir.sqrMagnitude < 0.01f) return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, _enemyLayer);

        EnemyHealth nearest = null;
        float nearestSqrDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 toEnemy = (Vector2)(hits[i].transform.position - transform.position);
            if (Vector2.Angle(aimDir, toEnemy) > _aimConeAngle) continue;

            float sqrDist = toEnemy.sqrMagnitude;
            if (sqrDist < nearestSqrDist &&
                hits[i].TryGetComponent<EnemyHealth>(out var health))
            {
                nearest = health;
                nearestSqrDist = sqrDist;
            }
        }

        return nearest;
    }
}
