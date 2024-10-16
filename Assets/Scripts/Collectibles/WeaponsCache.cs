using UnityEngine;

public class WeaponsCache : MonoBehaviour
{

    // Exports
    public Sprite[] weaponsCacheSprites;

    // References
    SpriteRenderer spriteRenderer;

    // State
    public Resources.Weapon WeaponType { get; set; }

    // Start is called before the first frame update
    void Start() {
        int randomWeapon = Random.Range((int) Resources.Weapon.HANDGUN, (int) Resources.Weapon.SNIPER_RIFLE + 1); // Equal likelihood of any weapon
        WeaponType = (Resources.Weapon) randomWeapon;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weaponsCacheSprites[randomWeapon - 1];
    }
}
