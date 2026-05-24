using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_New", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string WeaponName;
    public Sprite Icon;

    [Header("Stats")]
    public float Damage = 15f;
    public float FireRate = 2f;
    public float Range = 10f;
    public float ProjectileSpeed = 12f;

    [Header("Vampiric")]
    public bool IsVampiric;
    public float VampiricHealAmount = 3f;

    [Header("References")]
    public Projectile ProjectilePrefab;
    public AudioClip FireSFX;
}
