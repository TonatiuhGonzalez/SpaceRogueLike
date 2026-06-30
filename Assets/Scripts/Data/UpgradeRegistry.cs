using System.Collections.Generic;

public class UpgradeRegistry
{
    private readonly Dictionary<GenericStat, int> _genericLevels = new();
    private readonly Dictionary<WeaponType, int> _specificLevels = new();

    private const float GENERIC_BONUS_PER_LEVEL = 0.30f;
    private const int MAX_SPECIFIC_LEVEL = 3;

    public float GetGenericMultiplier(GenericStat stat) =>
        1f + GetGenericLevel(stat) * GENERIC_BONUS_PER_LEVEL;

    public int GetGenericLevel(GenericStat stat) =>
        _genericLevels.TryGetValue(stat, out int lvl) ? lvl : 0;

    public int GetSpecificLevel(WeaponType type) =>
        _specificLevels.TryGetValue(type, out int lvl) ? lvl : 0;

    public void ApplyGenericUpgrade(GenericStat stat)
    {
        _genericLevels[stat] = GetGenericLevel(stat) + 1;
    }

    public void ApplySpecificUpgrade(WeaponType type)
    {
        int current = GetSpecificLevel(type);
        if (current < MAX_SPECIFIC_LEVEL)
            _specificLevels[type] = current + 1;
    }

    public void Reset()
    {
        _genericLevels.Clear();
        _specificLevels.Clear();
    }
}
