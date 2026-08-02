using UnityEngine;

[CreateAssetMenu(fileName = "MinimapData", menuName = "Game/Minimap Data")]
public class MinimapData : ScriptableObject
{
    [Header("Detection")]
    public float DetectionRadius = 15f;
}
