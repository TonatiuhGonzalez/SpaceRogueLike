using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionLogic
{
    private readonly RunConfig _config;

    public WeaponSelectionLogic(RunConfig config)
    {
        _config = config;
    }

    public SelectionOffer[] BuildOffers(
        int playerLevel,
        List<WeaponData> availableWeapons,
        List<WeaponType> ownedTypes,
        WeaponUpgradeData[] upgradePool,
        UpgradeRegistry registry)
    {
        bool upgradesOnly = ownedTypes.Count >= _config.MaxWeaponSlots;
        bool canUpgrade = playerLevel >= _config.UpgradeStartLevel;

        List<SelectionOffer> weaponOffers = BuildWeaponOffers(availableWeapons);
        List<SelectionOffer> upgradeOffers = BuildUpgradeOffers(upgradePool, ownedTypes, registry);

        if (upgradesOnly)
        {
            Shuffle(upgradeOffers);
            return TakeTwo(upgradeOffers, new List<SelectionOffer>());
        }

        if (!canUpgrade)
        {
            Shuffle(weaponOffers);
            if (weaponOffers.Count >= 2)
                return new[] { weaponOffers[0], weaponOffers[1] };

            Shuffle(upgradeOffers);
            return TakeTwo(weaponOffers, upgradeOffers);
        }

        List<SelectionOffer> combined = new(weaponOffers);
        combined.AddRange(upgradeOffers);
        Shuffle(combined);
        return TakeTwo(combined, new List<SelectionOffer>());
    }

    private static List<SelectionOffer> BuildWeaponOffers(List<WeaponData> available)
    {
        var offers = new List<SelectionOffer>(available.Count);
        foreach (WeaponData weapon in available)
            offers.Add(SelectionOffer.ForWeapon(weapon));
        return offers;
    }

    private static List<SelectionOffer> BuildUpgradeOffers(
        WeaponUpgradeData[] pool,
        List<WeaponType> ownedTypes,
        UpgradeRegistry registry)
    {
        var offers = new List<SelectionOffer>();
        foreach (WeaponUpgradeData upgrade in pool)
        {
            if (IsUpgradeAvailable(upgrade, ownedTypes, registry))
                offers.Add(SelectionOffer.ForUpgrade(upgrade));
        }
        return offers;
    }

    private static bool IsUpgradeAvailable(
        WeaponUpgradeData upgrade,
        List<WeaponType> ownedTypes,
        UpgradeRegistry registry)
    {
        if (upgrade.Category == UpgradeCategory.Generic)
            return true;

        if (!ownedTypes.Contains(upgrade.TargetWeaponType))
            return false;

        return registry.GetSpecificLevel(upgrade.TargetWeaponType) == upgrade.UpgradeLevel - 1;
    }

    private static SelectionOffer[] TakeTwo(List<SelectionOffer> primary, List<SelectionOffer> fallback)
    {
        var result = new List<SelectionOffer>(2);

        if (primary.Count >= 1) result.Add(primary[0]);
        if (primary.Count >= 2) result.Add(primary[1]);

        for (int i = 0; result.Count < 2 && i < fallback.Count; i++)
            result.Add(fallback[i]);

        while (result.Count < 2)
            result.Add(null);

        return new[] { result[0], result[1] };
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
