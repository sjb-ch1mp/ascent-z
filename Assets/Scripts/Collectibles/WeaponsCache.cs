using UnityEngine;

public class WeaponsCache : MonoBehaviour
{

    // Exports
    public Sprite[] weaponsCacheSprites;
    public bool inCache = false;

    // State
    public Resources.Weapon WeaponType { get; set; }

    // Start is called before the first frame update
    void Awake() {
        int randomWeapon = Random.Range((int) Resources.Weapon.HANDGUN, (int) Resources.Weapon.SNIPER_RIFLE + 1); // Equal likelihood of any weapon
        WeaponType = (Resources.Weapon) randomWeapon;
        if (inCache) {
            GetComponent<SpriteRenderer>().sprite = weaponsCacheSprites[(int) WeaponType - 1];
        }
    }
}
