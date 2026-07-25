using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionController : MonoBehaviour
{
    [Header("Offer Cards")]
    [SerializeField] private WeaponCardUI[] _offerCards;

    [Header("Swap Panel")]
    [SerializeField] private GameObject _swapPanel;
    [SerializeField] private WeaponCardUI[] _currentSlotCards;

    [Header("Dependencies")]
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private WeaponData[] _weaponPool;
    [SerializeField] private AudioData _audioData;

    private WeaponData _pendingWeapon;

    public void Show()
    {
        _swapPanel.SetActive(false);
        _pendingWeapon = null;

        WeaponData[] offers = PickRandomOffers();
        for (int i = 0; i < _offerCards.Length && i < offers.Length; i++)
            _offerCards[i].Setup(offers[i], OnWeaponChosen);
    }

    private WeaponData[] PickRandomOffers()
    {
        int count = Mathf.Min(_offerCards.Length, _weaponPool.Length);
        WeaponData[] shuffled = (WeaponData[])_weaponPool.Clone();

        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        WeaponData[] result = new WeaponData[count];
        System.Array.Copy(shuffled, result, count);
        return result;
    }

    private void OnWeaponChosen(WeaponData chosen)
    {
        AudioManager.Instance.PlaySFX(_audioData.ButtonClick);
        int emptySlot = _weaponController.FindFirstEmptySlot();

        if (emptySlot >= 0)
        {
            EquipAndContinue(chosen, emptySlot);
        }
        else
        {
            _pendingWeapon = chosen;
            ShowSwapPanel();
        }
    }

    private void ShowSwapPanel()
    {
        _swapPanel.SetActive(true);
        WeaponSlot[] slots = _weaponController.GetSlots();
        for (int i = 0; i < _currentSlotCards.Length && i < slots.Length; i++)
        {
            int slotIndex = i;
            _currentSlotCards[i].Setup(slots[i].EquippedWeapon,
                _ => OnSlotChosen(slotIndex));
        }
    }

    private void OnSlotChosen(int slotIndex)
    {
        if (_pendingWeapon == null) return;
        EquipAndContinue(_pendingWeapon, slotIndex);
    }

    private void EquipAndContinue(WeaponData weapon, int slotIndex)
    {
        _weaponController.EquipWeapon(weapon, slotIndex);
        _swapPanel.SetActive(false);
        _pendingWeapon = null;
        ScreenManager.Instance.ShowScreen(GameScreen.Gameplay);
        GameManager.Instance.StartLevel();
    }
}
