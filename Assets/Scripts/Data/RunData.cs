using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "Game/Run Data")]
public class RunData : ScriptableObject
{
    public int CurrentLevel { get; set; }
    public ShipArchetypeData SelectedArchetype { get; set; }
    public List<WeaponData> ActiveWeapons { get; private set; } = new();
    public int EnemiesKilled { get; set; }

    public UpgradeRegistry Upgrades { get; private set; } = new();
    public HashSet<WeaponType> OwnedWeaponTypes { get; private set; } = new();

    public void AddOwnedWeapon(WeaponType type) => OwnedWeaponTypes.Add(type);

    public void ResetForNewRun()
    {
        CurrentLevel = 1;
        SelectedArchetype = null;
        ActiveWeapons.Clear();
        EnemiesKilled = 0;
        Upgrades.Reset();
        OwnedWeaponTypes.Clear();
    }
}
