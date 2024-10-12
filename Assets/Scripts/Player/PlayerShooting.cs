using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public ProjectileBehaviour projectilePrefab;
    public Transform launchOffset;

    public float shootCooldown = 0.5f;  // Minimum time between shots
    private float nextShootTime;        // Time when the player can shoot again
    public bool rapidFire = false;      // Whether rapid fire is enabled

    private int ammo = 0;          // Tracks shots fired with machine gun
    [SerializeField] private int currentWeapon;

    private GameManager gameManager;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    public Sprite[] projectileSprites;  // Array to hold different sprites for each weapon


    public float projectileSpeed = 10f;  // Speed of the projectile

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = playerMovement.rb;  // Get reference to Rigidbody2D from PlayerMovement
    }

    // Public property to expose the currentWeapon variable
    public int CurrentWeapon
    {
        get { return currentWeapon; }
    }

    private void Update()
    {
        if (gameManager.IsPaused()) {
            return;
        }

        // Shooting logic
        if (rapidFire && Input.GetKey(KeyCode.Mouse0))  // Hold down mouse button for rapid fire
        {
            if (Time.time >= nextShootTime)
            {
                FireProjectile();
                gameManager.ConsumeAmmo();
                ammo--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))  // Single shot when mouse button is pressed
        {
            if (Time.time >= nextShootTime)
            {
                FireProjectile();
                gameManager.ConsumeAmmo();
                ammo--;
            }
        }

        // Check if need to switch back to original weapon
        if (ammo <= 0 && currentWeapon != 0)
        {
            currentWeapon = 0;
            rapidFire = false;
            shootCooldown = 0.5f;
            ammo = 0;
        }


    }

    void FireProjectile()
    {
        // Calculate the direction from the player to the mouse cursor
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;  // Ensure z=0 for 2D

        Vector2 direction = (mouseWorldPosition - launchOffset.position).normalized;


        // Weapon: Bat
        if (currentWeapon == 0)
        {
            // init projectile with direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            ProjectileBehaviour projectile = Instantiate(projectilePrefab, launchOffset.position, rotation);

            // set projectile characteristics
            projectile.damage = 25;
            projectile.speed = 15;
            projectile.sizeMultiplier = 9;
            projectile.SetSize();
            projectile.SetProjectileSprite(projectileSprites[currentWeapon]);

            projectile.SetInitialVelocity(direction * projectile.speed);

            // Set max range of projectile
            projectile.maxRange = 0.8f;
        }


        // Weapon: Handgun
        if (currentWeapon == 1)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            ProjectileBehaviour projectile = Instantiate(projectilePrefab, launchOffset.position, rotation);
            projectile.damage = 25;
            projectile.speed = 15;
            projectile.sizeMultiplier = 3;
            projectile.SetSize();
            projectile.SetProjectileSprite(projectileSprites[currentWeapon]);


            projectile.SetInitialVelocity(direction * projectile.speed);
        }

        //Shotgun
        if (currentWeapon == 2)
        {
            // Firing multiple projectiles (e.g., sniper weapon)
            int projectileCount = 12;
            float spreadAngle = 30f; // Total spread angle in degrees
            float angleStep = spreadAngle / (projectileCount - 1);
            float startingAngle = -spreadAngle / 2;

            for (int i = 0; i < projectileCount; i++)
            {
                // Get the mouse position in world space
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;  // Set z to 0 because we're working in 2D

                // Calculate direction from the launch position to the mouse position
                Vector2 directionToMouse = (mousePosition - launchOffset.position).normalized;

                // Calculate the spread direction with the angle step
                float currentAngle = startingAngle + (angleStep * i);
                Quaternion spreadRotation = Quaternion.Euler(0, 0, currentAngle);
                Vector2 spreadDirection = spreadRotation * directionToMouse;

                // Calculate the angle for projectile rotation
                float angle = Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion projectileRotation = Quaternion.Euler(0, 0, angle);

                // Instantiate the projectile with the calculated rotation
                ProjectileBehaviour projectile = Instantiate(projectilePrefab, launchOffset.position, projectileRotation);
                projectile.damage = 25;
                projectile.speed = 10;
                projectile.sizeMultiplier = 1.5f;
                projectile.SetSize();
                projectile.SetProjectileSprite(projectileSprites[currentWeapon]);



                // Set the projectile's velocity
                projectile.SetInitialVelocity(spreadDirection * projectile.speed);
            }
        }
        // Weapon: Assault Rifle
        if (currentWeapon == 3)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            ProjectileBehaviour projectile = Instantiate(projectilePrefab, launchOffset.position, rotation);
            projectile.damage = 20;
            projectile.speed = 10;
            projectile.sizeMultiplier = 2.25f;
            projectile.SetSize();
            projectile.SetProjectileSprite(projectileSprites[currentWeapon]);

            projectile.SetInitialVelocity(direction * projectile.speed);

        }
        // Weapon: Sniper
        if (currentWeapon == 4)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            ProjectileBehaviour projectile = Instantiate(projectilePrefab, launchOffset.position, rotation);
            projectile.damage = 200;
            projectile.speed = 25;
            projectile.sizeMultiplier = 5f;
            projectile.penetration = 3f;
            projectile.SetSize();
            projectile.SetProjectileSprite(projectileSprites[currentWeapon]);


            projectile.SetInitialVelocity(direction * projectile.speed);
        }

        // Set the next shoot time to enforce cooldown
        nextShootTime = Time.time + shootCooldown;
    }






    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ItemDrop"))
        {
            // Pick up a new weapon
            int randWeapon = Random.Range((int)Resources.Weapon.HANDGUN, (int)Resources.Weapon.SNIPER_RIFLE + 1);
            gameManager.PickUpWeapon((Resources.Weapon)randWeapon);

            // Weapon: handgun
            if (randWeapon == 1)
            {
                currentWeapon = 1;
                rapidFire = false;
                shootCooldown = 0.5f;
                ammo = Resources.GetAmmoForWeapon(Resources.Weapon.HANDGUN);
            }
            // Weapon: shotgun
            else if (randWeapon == 2)
            {
                currentWeapon = 2;
                rapidFire = false;
                shootCooldown = 1.0f;
                ammo = Resources.GetAmmoForWeapon(Resources.Weapon.SHOTGUN);
            }
            // Weapon: machinegun
            else if (randWeapon == 3)
            {
                currentWeapon = 3;
                rapidFire = true;
                shootCooldown = 0.1f;
                ammo = Resources.GetAmmoForWeapon(Resources.Weapon.ASSAULT_RIFLE);
            }
            // Weapon: sniper
            else if (randWeapon == 4)
            {
                currentWeapon = 4;
                rapidFire = false;
                shootCooldown = 1.25f;
                ammo = Resources.GetAmmoForWeapon(Resources.Weapon.SNIPER_RIFLE);
            }
        }
    }
}