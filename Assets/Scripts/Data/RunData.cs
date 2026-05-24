using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "Game/Run Data")]
public class RunData : ScriptableObject
{
    public int CurrentLevel { get; set; }
    public ShipArchetypeData SelectedArchetype { get; set; }
    public List<WeaponData> ActiveWeapons { get; private set; } = new();
    public int EnemiesKilled { get; set; }

    public void ResetForNewRun()
    {
        CurrentLevel = 1;
        SelectedArchetype = null;
        ActiveWeapons.Clear();
        EnemiesKilled = 0;
    }
}
