using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_New", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public EnemyType Type;

    [Header("Stats")]
    public float BaseHp = 30f;
    public float BaseSpeed = 2f;
    public float BaseFireRate = 1f;
    public float BaseDamage = 10f;
    public float BaseRange = 8f;
    public float BaseProjectileSpeed = 6f;

    [Header("Projectile")]
    public Projectile ProjectilePrefab;
}
