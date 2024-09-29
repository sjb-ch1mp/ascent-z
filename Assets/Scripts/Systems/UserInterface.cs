using TMPro;
using UnityEngine;

public class UserInterface : MonoBehaviour
{

    // Exports
    // == Weapons
    public GameObject baseballBat;
    public GameObject handgun;
    public GameObject shotgun;
    public GameObject assaultRifle;
    public GameObject sniperRifle;
    // == Crosshairs
    public Texture2D baseballCrosshair;
    public Texture2D handgunCrosshair;
    public Texture2D shotgunCrosshair;
    public Texture2D assaultRifleCrosshair;
    public Texture2D sniperRifleCrosshair;
    // == Labels
    public TextMeshProUGUI ammoCountLabel;
    public TextMeshProUGUI grenadeCountLabel;
    public TextMeshProUGUI infinityLabel;
    // == Status
    public GameObject[] hearts;
    public GameObject armourActive;
    public GameObject grenadeActive;

    // State
    Resources.Weapon weapon;
    int ammoCount = 0;
    int grenadeCount = 0;

    void Start() {
        PickUpWeapon(Resources.Weapon.BASEBALL_BAT);
    }

    public void PickUpWeapon(Resources.Weapon newWeapon) {

        Debug.Log($"Picked up weapon: {weapon}");

        // If new weapon, reset ammo count
        if (weapon != newWeapon) {
            weapon = newWeapon;
            ammoCount = 0;
            DisableAllWeapons();

            // Reveal the current weapon in the UI 
            // Change the crosshair
            // Update the ammo
            switch(weapon) {
                case Resources.Weapon.HANDGUN:
                    handgun.SetActive(true);
                    Cursor.SetCursor(handgunCrosshair, Vector2.zero, CursorMode.Auto);
                    break;  
                case Resources.Weapon.SHOTGUN:
                    shotgun.SetActive(true);
                    Cursor.SetCursor(shotgunCrosshair, Vector2.zero, CursorMode.Auto);
                    break;  
                case Resources.Weapon.ASSAULT_RIFLE:
                    assaultRifle.SetActive(true);
                    Cursor.SetCursor(assaultRifleCrosshair, Vector2.zero, CursorMode.Auto);
                    break;  
                case Resources.Weapon.SNIPER_RIFLE: 
                    sniperRifle.SetActive(true);
                    Cursor.SetCursor(sniperRifleCrosshair, Vector2.zero, CursorMode.Auto);
                    break;
                default: // Baseball bat
                    baseballBat.SetActive(true);
                    Cursor.SetCursor(baseballCrosshair, Vector2.zero, CursorMode.Auto);
                    ammoCountLabel.gameObject.SetActive(false);
                    infinityLabel.gameObject.SetActive(true);
                    break;
            }
        }
        
        // Increase the ammo count to the maximum
        ammoCount += Resources.GetAmmoForWeapon(weapon);
        if (ammoCount > Resources.GetAmmoForWeapon(weapon)) {
            ammoCount = Resources.GetAmmoForWeapon(weapon);
        }

        // Update labels
        if (weapon == Resources.Weapon.BASEBALL_BAT) {
            if (!infinityLabel.gameObject.activeSelf) {
                ammoCountLabel.gameObject.SetActive(false);
                infinityLabel.gameObject.SetActive(true);
            }
        } else {
            if (!ammoCountLabel.gameObject.activeSelf) {
                infinityLabel.gameObject.SetActive(false);
                ammoCountLabel.gameObject.SetActive(true);
            }
            ammoCountLabel.text = $"{ammoCount} / {Resources.GetAmmoForWeapon(weapon)}";
        }
        
    }

    public void PickUpCollectible(Resources.Collectible collectible) {
        int increaseAmount = Resources.GetAmountForCollectible(collectible);
        switch(collectible) {
            case Resources.Collectible.GRENADES:
                grenadeCount = increaseAmount;
                grenadeCountLabel.text = $"{grenadeCount}";
                grenadeActive.SetActive(true);
                break;
            case Resources.Collectible.ARMOUR:
                armourActive.SetActive(true);
                break;
            default: 
                return;
        }
    }

    void DisableAllWeapons() {
        baseballBat.SetActive(false);
        handgun.SetActive(false);
        shotgun.SetActive(false);
        assaultRifle.SetActive(false);
        sniperRifle.SetActive(false);
    }

    public void ConsumeAmmo() {
        if (ammoCount > 0) {
            ammoCount--;
            if (ammoCount == 0) {
                // Switch back to baseball bat
                PickUpWeapon(Resources.Weapon.BASEBALL_BAT);
            } else {
                ammoCountLabel.text = $"{ammoCount} / {Resources.GetAmmoForWeapon(weapon)}";
            }
        }
    }

    public void ConsumeGrenade() {
        if (grenadeCount > 0) {
            grenadeCount--;
            grenadeCountLabel.text = $"{grenadeCount}";
        }
        if (grenadeCount == 0 && grenadeActive.activeSelf) {
            grenadeActive.SetActive(false);
        }
    }

    public void DepleteArmour() {
        armourActive.SetActive(false);
    }

    public bool HasAmmo() {
        return ammoCount > 0;
    }

    public bool HasGrenades() {
        return grenadeCount > 0;
    }

    public Resources.Weapon GetCurrentWeapon() {
        return weapon;
    }

    public void RenderLives(int lives) {   
        for (int i = 0; i < Resources.MAX_LIVES; i++) {
            if (i < lives) {
                hearts[i].SetActive(true);
            } else {
                hearts[i].SetActive(false);
            }
        }
    }
}