using UnityEngine;

[CreateAssetMenu(fileName = "OfferBadgeConfig", menuName = "Game/UI/Offer Badge Config")]
public class OfferBadgeConfig : ScriptableObject
{
    [Header("Weapon Offer")]
    [SerializeField] private BadgeVisual _weapon;

    [Header("Generic Upgrade")]
    [SerializeField] private BadgeVisual _genericUpgrade;

    [Header("Specific Upgrade")]
    [SerializeField] private BadgeVisual _specificUpgrade;

    public BadgeVisual Weapon => _weapon;
    public BadgeVisual GenericUpgrade => _genericUpgrade;
    public BadgeVisual SpecificUpgrade => _specificUpgrade;
}

[System.Serializable]
public struct BadgeVisual
{
    public Sprite Icon;
    public Color Color;
    public Vector2 Size;
}
