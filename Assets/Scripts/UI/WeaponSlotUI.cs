using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private GameObject _emptyIndicator;
    [SerializeField] private Button _selectButton;

    [SerializeField] private Image _outlineFrame;

    private int _slotIndex;
    private WeaponController _weaponController;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(OnSelectPressed);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(OnSelectPressed);
    }

    public void SetActive(bool isActive)
    {
        _outlineFrame.enabled = isActive;
    }

    public void SetIndex(int index, WeaponController controller)
    {
        _slotIndex = index;
        _weaponController = controller;
    }

    public void Refresh(WeaponSlot slot)
    {
        bool hasWeapon = !slot.IsEmpty;
        _weaponIcon.gameObject.SetActive(hasWeapon);
        _emptyIndicator.SetActive(!hasWeapon);

        if (hasWeapon)
            _weaponIcon.sprite = slot.EquippedWeapon.Icon;
    }

    private void OnSelectPressed()
    {
        _weaponController.SetActiveSlot(_slotIndex);
    }
}
