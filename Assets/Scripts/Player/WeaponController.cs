using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Camera mainCamera;
    private PlayerShooting playerShooting;

    // Array of weapon GameObjects
    [SerializeField]
    private GameObject[] weaponObjects;

    private GameObject activeWeapon;              // Currently active weapon GameObject
    private SpriteRenderer activeWeaponRenderer;  // SpriteRenderer of the active weapon

    void Start()
    {
        mainCamera = Camera.main;

        // Get reference to PlayerShooting script
        playerShooting = GetComponent<PlayerShooting>();

        // Deactivate all weapons initially
        foreach (GameObject weapon in weaponObjects)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }

        // Activate the default weapon
        UpdateActiveWeapon();
    }

    void Update()
    {
        // Update the active weapon if currentWeapon has changed
        UpdateActiveWeapon();

        // Rotate the active weapon towards the mouse cursor
        RotateActiveWeaponTowardsMouse();
    }

    void UpdateActiveWeapon()
    {
        int currentWeaponIndex = playerShooting.CurrentWeapon;

        // If the active weapon is already set correctly, do nothing
        if (activeWeapon != null && weaponObjects[currentWeaponIndex] == activeWeapon)
            return;

        // Deactivate the current active weapon
        if (activeWeapon != null)
            activeWeapon.SetActive(false);

        // Activate the new weapon
        if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponObjects.Length)
        {
            activeWeapon = weaponObjects[currentWeaponIndex];
            if (activeWeapon != null)
            {
                activeWeapon.SetActive(true);
                activeWeaponRenderer = activeWeapon.GetComponent<SpriteRenderer>();
            }
        }
    }
    void RotateActiveWeaponTowardsMouse()
    {
        if (activeWeapon == null || activeWeaponRenderer == null)
            return;

        // Get the mouse position in world space
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        // Calculate direction from player to mouse
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the parent object, which contains the weapon
        Transform weaponParent = activeWeapon.transform.parent;
        if (weaponParent != null)
        {
            weaponParent.rotation = Quaternion.Euler(0, 0, angle);

            // Flip the entire parent object by rotating 180 degrees around the Y axis
            if (angle > 90 || angle < -90)
            {
                weaponParent.localScale = new Vector3(1, -1, 1); // Flip on Y axis
            }
            else
            {
                weaponParent.localScale = new Vector3(1, 1, 1);  // Reset flip
            }
        }
    }

}