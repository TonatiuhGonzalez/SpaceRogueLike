using System;
using UnityEngine;

[Serializable]
public class LevelEntry
{
    [Header("Enemies")]
    public int EnemyCountMin = 5;
    public int EnemyCountMax = 8;
    public EnemyType[] AllowedEnemyTypes = { EnemyType.Standard };

    [Header("Scaling")]
    public float HpMultiplier = 1f;
    public float SpeedMultiplier = 1f;
    public float FireRateMultiplier = 1f;

    [Header("AI")]
    public EnemyAITier AiTier = EnemyAITier.Tier1_Basic;
}

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Levels (index 0 = Level 1)")]
    public LevelEntry[] Levels = new LevelEntry[10];
}
