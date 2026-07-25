public enum OfferType { Weapon, Upgrade }

public sealed class SelectionOffer
{
    public OfferType Type { get; private set; }
    public WeaponData WeaponData { get; private set; }
    public WeaponUpgradeData UpgradeData { get; private set; }

    private SelectionOffer() { }

    public static SelectionOffer ForWeapon(WeaponData data) =>
        new() { Type = OfferType.Weapon, WeaponData = data };

    public static SelectionOffer ForUpgrade(WeaponUpgradeData data) =>
        new() { Type = OfferType.Upgrade, UpgradeData = data };
}
