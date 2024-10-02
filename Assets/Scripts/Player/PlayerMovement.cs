using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpPower = 15f;
    public int extraJump = 1;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] Transform feet;

    public float pushCooldown = 0.5f;  // Cooldown time after a push
    private float pushCooldownTime;    // Time when the push cooldown ends

    private int jumpCount = 0;
    public bool isFacingRight = true;
    bool isGrounded;
    float mx;

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void Update()
    {
        mx = Input.GetAxis("Horizontal");

        if (Time.time >= pushCooldownTime)  // Only allow movement if the cooldown has expired
        {
            if (mx > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (mx < 0 && isFacingRight)
            {
                Flip();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            CheckGrounded();
        }
    }

    private void FixedUpdate()
    {
        if (Time.time >= pushCooldownTime)  // Only move if the cooldown has expired
        {
            rb.velocity = new Vector2(mx * speed, rb.velocity.y);
        }
    }

    void Jump()
    {
        if (isGrounded || jumpCount < extraJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpCount++;
        }
    }

    void CheckGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(feet.position, 0.1f, groundLayer);
        if (collider != null)
        {
            isGrounded = true;
            jumpCount = 0;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            pushCooldownTime = Time.time + pushCooldown;  // Reset the cooldown timer when hit by an enemy

            // Determine the direction to push the player
            Vector2 pushDirection = (transform.position.x > collision.transform.position.x) ? Vector2.right : Vector2.left;

            // Apply pushback force
            rb.velocity = new Vector2(pushDirection.x * 10f, rb.velocity.y);  // Adjust the 10f to change the pushback force
        }
        else if (collision.gameObject.CompareTag("GameBoundary"))
        {
            // Reset player position to (0, 10) when colliding with a GameBoundary
            transform.position = new Vector3(0, 10, transform.position.z);
        }
    }
}





























































/*using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpPower = 15f;
    public int extraJump = 1;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform feet;

    public float pushCooldown = 0.5f;  // Cooldown time after a push
    private float pushCooldownTime;    // Time when the push cooldown ends

    private int jumpCount = 0;
    bool isGrounded;
    float mx;
    float jumpCoolDown;

    public ProjectileBehaviour projectilePrefab;
    public Transform launchOffset;

    private bool isFacingRight = true;


    public float shootCooldown = 0.5f;  // Minimum time between shots
    private float nextShootTime;        // Time when the player can shoot again
    public bool rapidFire = false;      // Whether rapid fire is enabled


    private int shotsFired = 0;          // Tracks shots fired with machine gun
    [SerializeField] private int currentWeapon;

    GameManager gameManager;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Public property to expose the currentWeapon variable
    public int CurrentWeapon
    {
        get { return currentWeapon; }
    }


    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void Update()
    {
        
        if (gameManager.IsPaused() || gameManager.IsGameOver()) {
            return;
        }

        mx = Input.GetAxis("Horizontal");

        if (Time.time >= pushCooldownTime)  // Only allow movement if the cooldown has expired
        {
            if (mx > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (mx < 0 && isFacingRight)
            {
                Flip();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

            CheckGrounded();

        }

        // Shooting logic
        if (rapidFire && Input.GetKey(KeyCode.Mouse0))  // Hold down space bar for rapid fire
        {
            if (Time.time >= nextShootTime)
            {
                FireProjectile();
                gameManager.ConsumeAmmo();
                shotsFired++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))  // Single shot when space bar is pressed
        {
            if (Time.time >= nextShootTime)
            {
                FireProjectile();
                gameManager.ConsumeAmmo();
                shotsFired++;
            }
        }


        // check if need to switch back to OG weapon
        if (shotsFired == 40 && currentWeapon == 1)
        {
            currentWeapon = 0;
            rapidFire = false;
            shootCooldown = 0.5f;
            shotsFired = 0;
        }

        // check if need to switch back to OG weapon
        if (shotsFired == 10 && currentWeapon == 2)
        {
            currentWeapon = 0;
            rapidFire = false;
            shootCooldown = 0.5f;
            shotsFired = 0;
        }




        
    }

    private void FixedUpdate()
    {
        if (Time.time >= pushCooldownTime)  // Only move if the cooldown has expired
        {
            rb.velocity = new Vector2(mx * speed, rb.velocity.y);
        }
    }

    void Jump()
    {
        if (isGrounded || jumpCount < extraJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpCount++;
        }
    }

    void CheckGrounded()
    {
        if (Physics2D.OverlapCircle(feet.position, 0.5f, groundLayer))
        {
            isGrounded = true;
            jumpCount = 0;
            jumpCoolDown = Time.time + 0.2f;
        }
        else if (Time.time < jumpCoolDown)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void FireProjectile()
    {
        if (currentWeapon == 2)
        {

            if (isFacingRight)
            {
                ProjectileBehaviour projectile1 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile1.SetInitialVelocity(rb.velocity + new Vector2(5f, 5f)); // Pass the player's velocity to the projectile

                ProjectileBehaviour projectile2 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile2.SetInitialVelocity(rb.velocity + new Vector2(5f, 5f));

                ProjectileBehaviour projectile3 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile3.SetInitialVelocity(rb.velocity + new Vector2(5f, 5f));

                ProjectileBehaviour projectile4 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile4.SetInitialVelocity(rb.velocity + new Vector2(5f, 5f));


                ProjectileBehaviour projectile5 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile5.SetInitialVelocity(rb.velocity + new Vector2(6f, 6f)); // Pass the player's velocity to the projectile

                ProjectileBehaviour projectile6 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile6.SetInitialVelocity(rb.velocity + new Vector2(6f, 6f));

                ProjectileBehaviour projectile7 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile7.SetInitialVelocity(rb.velocity + new Vector2(6f, 6f));

                ProjectileBehaviour projectile8 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile8.SetInitialVelocity(rb.velocity + new Vector2(6f, 6f));


                ProjectileBehaviour projectile9 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile9.SetInitialVelocity(rb.velocity + new Vector2(7f, 7f)); // Pass the player's velocity to the projectile

                ProjectileBehaviour projectile10 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile10.SetInitialVelocity(rb.velocity + new Vector2(7f, 7f));

                ProjectileBehaviour projectile11 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile11.SetInitialVelocity(rb.velocity + new Vector2(7f, 7f));

                ProjectileBehaviour projectile12 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile12.SetInitialVelocity(rb.velocity + new Vector2(7f, 7f));
            }
            else
            {
                ProjectileBehaviour projectile1 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile1.SetInitialVelocity(rb.velocity - new Vector2(5f, 5f)); // Pass the player's velocity to the projectile

                ProjectileBehaviour projectile2 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile2.SetInitialVelocity(rb.velocity - new Vector2(5f, 5f));

                ProjectileBehaviour projectile3 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile3.SetInitialVelocity(rb.velocity - new Vector2(5f, 5f));

                ProjectileBehaviour projectile4 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile4.SetInitialVelocity(rb.velocity - new Vector2(5f, 5f));


                ProjectileBehaviour projectile5 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile5.SetInitialVelocity(rb.velocity - new Vector2(6f, 6f)); // Pass the player's velocity to the projectile

                ProjectileBehaviour projectile6 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile6.SetInitialVelocity(rb.velocity - new Vector2(6f, 6f));

                ProjectileBehaviour projectile7 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile7.SetInitialVelocity(rb.velocity - new Vector2(6f, 6f));

                ProjectileBehaviour projectile8 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile8.SetInitialVelocity(rb.velocity - new Vector2(6f, 6f));


                ProjectileBehaviour projectile9 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile9.SetInitialVelocity(rb.velocity - new Vector2(7f, 7f)); // Pass the player's velocity to the projectile

                ProjectileBehaviour projectile10 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile10.SetInitialVelocity(rb.velocity - new Vector2(7f, 7f));

                ProjectileBehaviour projectile11 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile11.SetInitialVelocity(rb.velocity - new Vector2(7f, 7f));

                ProjectileBehaviour projectile12 = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
                projectile12.SetInitialVelocity(rb.velocity - new Vector2(7f, 7f));


            }
        }
        else
        {
            ProjectileBehaviour projectile = Instantiate(projectilePrefab, launchOffset.position, transform.rotation);
            projectile.SetInitialVelocity(rb.velocity);  // Pass the player's velocity to the projectile
        }
        
        

        // Set the next shoot time to enforce cooldown
        nextShootTime = Time.time + shootCooldown;



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            pushCooldownTime = Time.time + pushCooldown;  // Reset the cooldown timer when hit by an enemy

            // Determine the direction to push the player
            Vector2 pushDirection = (transform.position.x > collision.transform.position.x) ? Vector2.right : Vector2.left;

            // Apply pushback force
            rb.velocity = new Vector2(pushDirection.x * 10f, rb.velocity.y);  // Adjust the 10f to change the pushback force
        }
        else if (collision.gameObject.CompareTag("GameBoundary"))
        {
            // Reset player position to (0, 0) when colliding with a GameBoundary
            transform.position = new Vector3(0, 10, transform.position.z);
        }


        // ITEMDROP WEAPON CHANGING 
        else if (collision.gameObject.CompareTag("ItemDrop"))
        {

            // Pick up a new weapon
            int randWeapon = Random.Range((int) Resources.Weapon.HANDGUN, (int) Resources.Weapon.SNIPER_RIFLE + 1);
            gameManager.PickUpWeapon((Resources.Weapon) randWeapon);

            // Generate a random number between 0 and 1
            int randomWeapon = Random.Range(0, 2);

            if (randomWeapon == 0)
            {
                // Weapon: machine gun
                currentWeapon = 1;
                rapidFire = true;
                shootCooldown = 0.1f;
                shotsFired = 0;
            }
            else
            {
                // Weapon: sniper
                currentWeapon = 2;
                rapidFire = false;
                shootCooldown = 0.75f;
                shotsFired = 0;
            }


        }

        else if (collision.gameObject.CompareTag("Collectible")) {
            // Armour, Medpack, Lives, Grenades
            Resources.Collectible randCollectible = (Resources.Collectible) Random.Range((int) Resources.Collectible.ARMOUR, (int) Resources.Collectible.GRENADES + 1);
            gameManager.PickUpCollectible(randCollectible);
            Debug.Log($"Picked up collectible: {randCollectible}");

            // Other collectibles are being handled by PlayerHealth (strongly suggest consolidating the player stuff into a single class, though)
            PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();
            switch (randCollectible) {
                case Resources.Collectible.ARMOUR:
                    playerHealth.armour = Mathf.Clamp(playerHealth.armour + Resources.GetAmountForCollectible(randCollectible), 0, Resources.MAX_ARMOUR);
                    break;
                case Resources.Collectible.LIFE:
                    playerHealth.AddLife();
                    break;
                case Resources.Collectible.MEDPACK:
                    playerHealth.health = Mathf.Clamp(playerHealth.health + Resources.GetAmountForCollectible(randCollectible), 0, Resources.MAX_HEALTH);
                    break;
            }
            Destroy(collision.gameObject);
        }
    }
}*/