using UnityEngine;

[CreateAssetMenu(fileName = "Archetype_New", menuName = "Game/Ship Archetype")]
public class ShipArchetypeData : ScriptableObject
{
    [Header("Identity")]
    public string DisplayName;
    public Sprite ShipSprite;
    [TextArea(2, 4)]
    public string Description;

    [Header("Stats")]
    public float MaxHealth = 100f;
    public float MoveSpeed = 5f;
    public float DamageMultiplier = 1f;
}
