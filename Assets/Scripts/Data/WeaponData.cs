using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_New", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string WeaponName;
    public Sprite Icon;

    [Header("Type")]
    public WeaponType WeaponType;

    [Header("Stats")]
    public float Damage = 15f;
    public float FireRate = 2f;
    public float Range = 10f;
    public float ProjectileSpeed = 12f;

    [Header("Dispersion")]
    public float ConeAngleDegrees = 30f;

    [Header("Fan")]
    public int BulletCount = 3;
    public float FanSpreadAngle = 30f;

    [Header("Area")]
    public float ExplosionRadius = 20f;
    public float ExplosionDamagePercent = 0.5f;

    [Header("Zapper")]
    public int ChainCount = 2;
    public float[] ChainDamagePercents;
    public float ChainDelay = 0.2f;

    [Header("Vampiric")]
    public bool IsVampiric;
    public float VampiricHealPercent = 0.10f;

    [Header("Poison")]
    public float PoisonTickPercent = 0.10f;

    [Header("References")]
    public Projectile ProjectilePrefab;
    public AudioClip FireSFX;

    [Header("Visual")]
    public Sprite ProjectileSprite;
    [SerializeField] private float _projectileScale = 1f;

    public float ProjectileScale => _projectileScale;
}
