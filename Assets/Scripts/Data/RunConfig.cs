using UnityEngine;

[CreateAssetMenu(fileName = "RunConfig", menuName = "Game/Run Config")]
public class RunConfig : ScriptableObject
{
    [Header("Run Settings")]
    public int TotalLevels = 10;
    public int MaxWeaponSlots = 3;
    public int WeaponChoicesPerSelection = 2;

    [Header("Health Pack")]
    [Range(0f, 1f)]
    public float HealthPackDropChance = 0.003f;

    [Header("Enemy")]
    public float WarperSafeZoneRadius = 3f;
    public float SpawnDelayBetweenEnemies = 0.3f;
    public float MinEnemySpawnDistance = 5f;
}
