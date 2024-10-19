using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusKeys : MonoBehaviour
{

    public Color32 hasAmmoColor;
    public Color32 noAmmoColor;
    
    public TextMeshProUGUI[] weaponKeys;
    public Image[] selectedIndicator;

    public void UnselectAllWeapons() {
        for (int i=0; i<weaponKeys.Length; i++) {
            selectedIndicator[i].gameObject.SetActive(false);
        }
    }

    public void SelectWeapon(Resources.Weapon weapon) {
        UnselectAllWeapons();
        selectedIndicator[(int) weapon].gameObject.SetActive(true);
    }

    public void UpdateAmmoStatus(Resources.Weapon weapon, bool hasAmmo) {
        weaponKeys[(int) weapon].color = hasAmmo ? hasAmmoColor : noAmmoColor;
    }
}
