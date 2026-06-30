using UnityEngine;

public class WeaponSlot
{
    public WeaponData EquippedWeapon { get; private set; }
    public bool IsEmpty => EquippedWeapon == null;
    public bool IsReady => !IsEmpty && _cooldownRemaining <= 0f;

    public int SpecificUpgradeLevel { get; private set; }

    private float _cooldownRemaining;

    private static readonly float[] CONE_ANGLES        = { 30f,  25f,  15f  };
    private static readonly int[]   BULLET_COUNTS      = {  3,    4,    6   };
    private static readonly int[]   CHAIN_COUNTS       = {  2,    3,    5   };
    private static readonly float[] EXPLOSION_RADII    = { 20f,  30f,  50f  };
    private static readonly float[] VAMPIRIC_PERCENTS  = { 0.10f, 0.15f, 0.25f };
    private static readonly float[] POISON_TICK_PERCENTS = { 0.10f, 0.15f, 0.20f };
    private static readonly PoisonMode[] POISON_MODES  = { PoisonMode.Fixed, PoisonMode.Stackable, PoisonMode.Exponential };

    private int LevelIndex => Mathf.Clamp(SpecificUpgradeLevel, 0, 2);

    public float EffectiveConeAngle         => CONE_ANGLES[LevelIndex];
    public int   EffectiveBulletCount       => BULLET_COUNTS[LevelIndex];
    public int   EffectiveChainCount        => CHAIN_COUNTS[LevelIndex];
    public float EffectiveExplosionRadius   => EXPLOSION_RADII[LevelIndex];
    public float EffectiveVampiricPercent   => VAMPIRIC_PERCENTS[LevelIndex];
    public float EffectivePoisonTickPercent => POISON_TICK_PERCENTS[LevelIndex];
    public PoisonMode EffectivePoisonMode   => POISON_MODES[LevelIndex];

    private const int MAX_SPECIFIC_LEVEL = 3;

    public void Equip(WeaponData weapon)
    {
        EquippedWeapon = weapon;
        _cooldownRemaining = 0f;
        SpecificUpgradeLevel = 0;
    }

    public void Clear()
    {
        EquippedWeapon = null;
        _cooldownRemaining = 0f;
        SpecificUpgradeLevel = 0;
    }

    public void ApplySpecificUpgrade()
    {
        if (SpecificUpgradeLevel < MAX_SPECIFIC_LEVEL)
            SpecificUpgradeLevel++;
    }

    public void Tick(float deltaTime)
    {
        if (_cooldownRemaining > 0f)
            _cooldownRemaining -= deltaTime;
    }

    public void StartCooldown(float fireRateMultiplier = 1f)
    {
        if (EquippedWeapon == null || EquippedWeapon.FireRate <= 0f)
        {
            _cooldownRemaining = 1f;
            return;
        }
        _cooldownRemaining = 1f / (EquippedWeapon.FireRate * fireRateMultiplier);
    }
}
