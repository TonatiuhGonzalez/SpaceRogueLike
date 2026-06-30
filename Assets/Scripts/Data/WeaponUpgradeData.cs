using UnityEngine;

public enum UpgradeCategory { Generic, Specific }

[CreateAssetMenu(fileName = "Upgrade_New", menuName = "Game/Weapon Upgrade Data")]
public class WeaponUpgradeData : ScriptableObject
{
    [Header("Identity")]
    public string UpgradeName;
    public string Description;
    public Sprite Icon;

    [Header("Type")]
    public UpgradeCategory Category;
    public GenericStat Stat;
    public WeaponType TargetWeaponType;
    public int UpgradeLevel;
}
