using System.Collections.Generic;
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
    public AudioClip noAmmoSound;
    // == Screens
    public GameOverScreen gameOverScreen;
    public WeaponStatusKeys weaponStatusKeys;

    // State
    Resources.Weapon activeWeapon;
    Resources.Rank currentRank = Resources.Rank.Private;
    int grenadeCount = 0;
    Dictionary<Resources.Weapon, int> ammoCache = Resources.GetAmmoCache();

    // References
    GameManager gameManager;

    public static UserInterface Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start() {
        gameManager = GameManager.Instance;
        talkingHead.NewMessage($"Welcome to hell, private!\nIf you're really as green as look, hold the TAB key to view your RANK and the GAME CONTROLS.\nOtherwise, stop gawking and kill some goddamn zombies!\n", TalkingHead.MessageDestination.Communication, null);
    }

    void Update() {
        if (!gameManager.IsGameOver()) {
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
            if (Input.GetKeyDown(KeyCode.Q)) {
                if (talkingHead.gameObject.activeSelf) {
                    talkingHead.Dismiss();
                }
            }
            
            // Weapons
            if (Input.GetKeyDown("1")) {
                ActivateWeapon(Resources.Weapon.BASEBALL_BAT);
            } else if (Input.GetKeyDown("2")) {
                ActivateWeapon(Resources.Weapon.HANDGUN);
            } else if (Input.GetKeyDown("3")) {
                ActivateWeapon(Resources.Weapon.SHOTGUN);
            } else if (Input.GetKeyDown("4")) {
                ActivateWeapon(Resources.Weapon.ASSAULT_RIFLE);
            } else if (Input.GetKeyDown("5")) {
                ActivateWeapon(Resources.Weapon.SNIPER_RIFLE);
            }
        }
    }

    public void IncreaseRank(int newRank) {
        currentRank += newRank;
    }

    public void RunScoreRoutine(int killScore, int survivorCount, int reviveCount, int finalScore) {
        talkingHead.ShowScore(killScore, survivorCount, reviveCount, finalScore);
    }

    public void PlayerSpawn() {
        activeWeapon = Resources.Weapon.BASEBALL_BAT;
        if (ammoCache[Resources.Weapon.HANDGUN] == 0) {
            ammoCache[Resources.Weapon.HANDGUN] = Resources.GetAmmoForWeapon(Resources.Weapon.HANDGUN);
            weaponStatusKeys.UpdateAmmoStatus(Resources.Weapon.HANDGUN, true);
        }
        ActivateNextWeaponWithAmmo();
    }

    public void ActivateWeapon(Resources.Weapon weaponToActivate) {
        Player player = GameManager.Instance.GetPlayer().GetComponent<Player>();
        if (!player.IsDead()) {
            if (weaponToActivate == activeWeapon) {
                return; // Nothing to do
            }
            if (weaponToActivate == Resources.Weapon.BASEBALL_BAT || ammoCache[weaponToActivate] > 0) {
                PlaySoundForWeapon(weaponToActivate);
                activeWeapon = weaponToActivate;
                UpdateElementsForWeapon();
                UpdateLabelsForWeapon();
                weaponStatusKeys.SelectWeapon(activeWeapon);
                player.PickUpWeapon(activeWeapon); // Pass the active weapon to the player
            } else {
                audioSource.PlayOneShot(noAmmoSound);
            }
        }
    }

    public void PlaySoundForWeapon(Resources.Weapon weapon) {
        switch (weapon) {
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
                audioSource.PlayOneShot(baseballBatLoad);
                break;
        }
    }

    public void UpdateLabelsForWeapon() {
        // Update labels
        if (activeWeapon == Resources.Weapon.BASEBALL_BAT) {
            if (!infinityLabel.gameObject.activeSelf) {
                ammoCountLabel.gameObject.SetActive(false);
                infinityLabel.gameObject.SetActive(true);
            }
        } else {
            if (!ammoCountLabel.gameObject.activeSelf) {
                infinityLabel.gameObject.SetActive(false);
                ammoCountLabel.gameObject.SetActive(true);
            }
            ammoCountLabel.text = $"{ammoCache[activeWeapon]}";
        }
        weaponStatusKeys.UpdateAmmoStatus(activeWeapon, true);
    }

    public void UpdateElementsForWeapon() {
        DisableAllWeapons();
        switch(activeWeapon) {
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
                break;
        }
    }

    public void PickUpWeapon(Resources.Weapon newWeapon) {

        PlaySoundForWeapon(newWeapon);

        // Add ammo to the gun type
        if (newWeapon != Resources.Weapon.BASEBALL_BAT) {
            bool newAmmoForWeapon = ammoCache[newWeapon] == 0;
            ammoCache[newWeapon] = Mathf.Clamp(ammoCache[newWeapon] + Resources.GetAmmoForWeapon(newWeapon), 0, Resources.MAX_AMMO);
            if (newAmmoForWeapon) {
                ActivateWeapon(newWeapon);
            }
        }
    }

    public void PickUpCollectible(Resources.Collectible collectible) {
        int increaseAmount = Resources.GetAmountForCollectible(collectible);
        Player player = GameManager.Instance.GetPlayer().GetComponent<Player>();
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
                player.PickUpArmour(increaseAmount);
                break;
            case Resources.Collectible.MEDPACK:
                audioSource.PlayOneShot(pickUpHealth);
                player.PickUpHealth(increaseAmount);
                break;
            case Resources.Collectible.LIFE:
                audioSource.PlayOneShot(pickUpLife);
                gameManager.Lives = Mathf.Clamp(gameManager.Lives + 1, 0, Resources.MAX_LIVES);
                RenderLives(gameManager.Lives);
                break;
            case Resources.Collectible.AMMUNITION:
                audioSource.PlayOneShot(pickUpAmmo);
                ammoCache[activeWeapon] = Mathf.Clamp(ammoCache[activeWeapon] + Resources.GetAmmoForWeapon(activeWeapon), 0, Resources.MAX_AMMO);
                UpdateLabelsForWeapon();
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
        if (ammoCache[activeWeapon] > 0) {
            ammoCache[activeWeapon]--;
            if (ammoCache[activeWeapon] == 0) {
                weaponStatusKeys.UpdateAmmoStatus(activeWeapon, false);
                ActivateNextWeaponWithAmmo();
            } else {
                ammoCountLabel.text = $"{ammoCache[activeWeapon]}";
            }
        }
    }

    public void ActivateNextWeaponWithAmmo() {
        Debug.Log("Got weapon: ");
        for (int i=(int)Resources.Weapon.SNIPER_RIFLE; i>(int)Resources.Weapon.BASEBALL_BAT; i--) {
            if (ammoCache[(Resources.Weapon)i] > 0) {
                Debug.Log($"Got weapon: {(Resources.Weapon)i}");
                ActivateWeapon((Resources.Weapon)i);
                return;
            }
        }
        Debug.Log($"Got weapon: {Resources.Weapon.BASEBALL_BAT}");
        ActivateWeapon(Resources.Weapon.BASEBALL_BAT);
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
        return ammoCache[activeWeapon] > 0;
    }

    public bool HasGrenades() {
        return grenadeCount > 0;
    }

    public Resources.Weapon GetCurrentWeapon() {
        return activeWeapon;
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