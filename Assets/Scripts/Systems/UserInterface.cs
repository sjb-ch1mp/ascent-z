using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    // == Info
    public TalkingHead talkingHead;
    public GameObject controlScheme;
    // == Audio
    public AudioSource audioSource;
    public AudioClip openControls;
    public AudioClip closeControls;
    public AudioClip baseballBatLoad;
    public AudioClip handgunLoad;
    public AudioClip shotgunLoad;
    public AudioClip assaultRifleLoad;
    public AudioClip sniperRifleLoad;
    public AudioClip pickUpArmour;
    public AudioClip pickUpLife;
    public AudioClip pickUpHealth;
    public AudioClip pickUpGrenade;
    public AudioClip pickUpAmmo;

    // State
    Resources.Weapon weapon;
    Resources.Rank currentRank = Resources.Rank.Private;
    int ammoCount = 0;
    int grenadeCount = 0;

    void Start() {
        PickUpWeapon(Resources.Weapon.BASEBALL_BAT);
        talkingHead.NewMessage($"Welcome to hell, private!\nIf you're really as green as look, hold the TAB key to view your RANK and the GAME CONTROLS.\nOtherwise, stop gawking and kill some goddamn zombies!\n", TalkingHead.MessageDestination.Communication, null);
    }

    void Update() {
        if (Input.GetKey(KeyCode.Tab)) {
            if (!controlScheme.activeSelf) {
                audioSource.PlayOneShot(openControls);
                controlScheme.SetActive(true);

                // Update the rank
                GameObject rankWindow = controlScheme.gameObject.transform.GetChild(0).gameObject;
                RankScreen rankScreen = rankWindow.transform.GetChild(1).gameObject.GetComponent<RankScreen>();
                TextMeshProUGUI rankText = rankWindow.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
                rankScreen.SetRank(currentRank);
                rankText.text = Resources.GetNameForRank(currentRank);

            }
        } else {
            if (controlScheme.activeSelf) {
                audioSource.PlayOneShot(closeControls);
                controlScheme.SetActive(false);
            }
        }
        if (Input.GetKey(KeyCode.Escape)) {
            if (talkingHead.gameObject.activeSelf) {
                talkingHead.Dismiss();
            }
        }
    }

    public void IncreaseRank(int newRank) {
        currentRank += newRank;
    }

    public void RunScoreRoutine(int killScore, int survivorCount, int reviveCount, int finalScore) {
        talkingHead.ShowScore(killScore, survivorCount, reviveCount, finalScore);
    }

    public void PickUpWeapon(Resources.Weapon newWeapon) {

        // Do sounds regardless
        switch (newWeapon) {
            case Resources.Weapon.HANDGUN:
                audioSource.PlayOneShot(handgunLoad);
                break;  
            case Resources.Weapon.SHOTGUN:
                audioSource.PlayOneShot(shotgunLoad);
                break;  
            case Resources.Weapon.ASSAULT_RIFLE:
                audioSource.PlayOneShot(assaultRifleLoad);
                break;  
            case Resources.Weapon.SNIPER_RIFLE: 
                audioSource.PlayOneShot(sniperRifleLoad);
                break;
            default: // Baseball bat
                baseballBat.SetActive(true);
                audioSource.PlayOneShot(baseballBatLoad);
                break;
        }

        // If new weapon, reset ammo count
        PlayerShooting player = GameObject.Find("Player").GetComponent<PlayerShooting>();
        if (weapon != newWeapon || newWeapon == Resources.Weapon.BASEBALL_BAT) {
            weapon = newWeapon;
            ammoCount = 0;
            DisableAllWeapons();
            player.PickUpWeapon(newWeapon); // Pass the new weapon to the player

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
        
        // Increase the ammo count (no maximum)
        ammoCount += Resources.GetAmmoForWeapon(weapon);

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
            ammoCountLabel.text = $"{ammoCount}";
        }

        // Update the ammo for the player
        player.UpdateAmmunitionCount(ammoCount);
        
    }

    public void PickUpCollectible(Resources.Collectible collectible) {
        int increaseAmount = Resources.GetAmountForCollectible(collectible);
        PlayerHealth playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        switch(collectible) {
            case Resources.Collectible.GRENADES:
                audioSource.PlayOneShot(pickUpGrenade);
                grenadeCount = increaseAmount;
                grenadeCountLabel.text = $"{grenadeCount}";
                grenadeActive.SetActive(true);
                break;
            case Resources.Collectible.ARMOUR:
                audioSource.PlayOneShot(pickUpArmour);
                armourActive.SetActive(true);
                playerHealth.armour = Mathf.Clamp(playerHealth.armour + increaseAmount, 0, Resources.MAX_ARMOUR);
                break;
            case Resources.Collectible.MEDPACK:
                audioSource.PlayOneShot(pickUpHealth);
                playerHealth.health = Mathf.Clamp(playerHealth.health + increaseAmount, 0, Resources.MAX_HEALTH);
                break;
            case Resources.Collectible.LIFE:
                audioSource.PlayOneShot(pickUpLife);
                playerHealth.AddLife();
                break;
            case Resources.Collectible.AMMUNITION:
                audioSource.PlayOneShot(pickUpAmmo);
                PickUpWeapon(GetCurrentWeapon());
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
                ammoCountLabel.text = $"{ammoCount}";
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