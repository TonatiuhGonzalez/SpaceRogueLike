using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private GameObject _emptyIndicator;

    public void Refresh(WeaponSlot slot)
    {
        bool hasWeapon = !slot.IsEmpty;
        _weaponIcon.gameObject.SetActive(hasWeapon);
        _emptyIndicator.SetActive(!hasWeapon);

        if (hasWeapon)
            _weaponIcon.sprite = slot.EquippedWeapon.Icon;
    }
}
