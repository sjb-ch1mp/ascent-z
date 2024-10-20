using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Exports
    public float healthTickTime = 1f;
    public int healthTickAmount = 5;
    public float deathWaitTime = 3f; 
    public float moveSpeed = 5f;
    public float jumpPower = 15f;
    public int extraJump = 1;
    public float fallDamage = 2;
    public float fallMin = 50;
    public GrenadeBehaviour grenadePrefab;
    public PlayerArms arms;
    public AudioClip[] deathSounds;
    public AudioClip[] fallDamageSounds;

    // References
    GameManager gameManager;
    Animator animator;
    Transform armsPivot;
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSprite;
    SpriteRenderer healthBar;
    SpriteRenderer armourBar;
    Transform feet;
    AudioSource audioSource;
    SpawnCar spawnCar;
    Transform worldBorderLeft;
    Transform worldBorderRight;

    // State
    bool isAlive = true;
    bool isStunned = false;
    bool isGrounded = false;
    int jumpCount = 0;
    Vector2 offGroundPosition = Vector2.negativeInfinity;
    public float Health { get; set; }
    public float Armour { get; set; }

    void Start() {
        // Get references
        gameManager = GameManager.Instance;
        animator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        armsPivot = transform.Find("ArmsPivot");
        playerSprite = GetComponent<SpriteRenderer>();
        healthBar = transform.Find("Health").GetComponent<SpriteRenderer>();
        armourBar = transform.Find("Armour").GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        worldBorderLeft = GameObject.Find("WorldBorderLeft").transform;
        worldBorderRight = GameObject.Find("WorldBorderRight").transform;
        feet = transform.Find("Feet");
        Health = Resources.MAX_HEALTH;
        UpdateStatusBar(false);
        Armour = 0;
        UpdateStatusBar(true);
        spawnCar = GameObject.Find("SpawnCar").GetComponent<SpawnCar>();
        spawnCar.DoSpawnCar(this);
        // Start Coroutines
        StartCoroutine(RegenerateHealth());
        StartCoroutine(DoRandomDriveBy());
    }

    // =========== Updates
    // Run conditional checks prior to continuing with an update function
    bool ContinueUpdate() {
        return isAlive && !isStunned && !gameManager.IsPaused() && !gameManager.IsGameOver();
    }
    void FixedUpdate() {
        if (ContinueUpdate()) {
            // Movement
            float movementDirection = Input.GetAxis("Horizontal");
            if (movementDirection > 0 || movementDirection < 0) {
                animator.SetBool("isMoving", true);
                playerRigidBody.velocity = new Vector2(movementDirection * moveSpeed, playerRigidBody.velocity.y);
            } else {
                animator.SetBool("isMoving", false);
            }
        }
    }
    void Update() {
        if (ContinueUpdate()) {

            // Sprite positioning
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerSprite.flipX = mouseWorldPosition.x < transform.position.x;
            arms.SetDirection(RotateArms());

            // Jumping
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) {
                Jump();
            }

            // Firing
            if (Input.GetKeyDown(KeyCode.Mouse0) || (arms.RapidFire() && Input.GetKey(KeyCode.Mouse0))) {
                arms.FireWeapon();
            } else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                ThrowGrenade();
            }
        }
        CheckOnGround();
        // World boundaries
        if (transform.position.x < worldBorderLeft.position.x) {
            transform.position = new Vector2(worldBorderLeft.position.x, transform.position.y);
        } else if (transform.position.x > worldBorderRight.position.x) {
            transform.position = new Vector2(worldBorderRight.position.x, transform.position.y);
        }
    }

    // ========== Movement
    void Jump() {
        if (isGrounded || jumpCount < extraJump) {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpPower);  // Apply the jump force
            jumpCount++;
        }
    }

    void CheckOnGround() {
        RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, 0.5f);
        if (hit.collider != null) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || 
                hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                if (!isGrounded) {
                    // Moment that we first land
                    float fallLength = offGroundPosition.y - transform.position.y;
                    if (fallLength > 0 && Math.Abs(fallLength) > fallMin) {
                        // apply fall damage
                        float dmg = (fallLength - fallMin) * fallDamage;
                        audioSource.PlayOneShot(fallDamageSounds[UnityEngine.Random.Range(0, fallDamageSounds.Length)]);
                        TakeDamage(dmg);
                    }
                    if (animator.GetBool("isAirborne")) {
                        animator.SetBool("isAirborne", false);
                    }
                }
                jumpCount = 0;
                isGrounded = true;
                return;
            } 
        } 
        
        // No ground or enemy hit
        if (isGrounded) {
            // Moment that we first leave the ground
            if (!animator.GetBool("isAirborne")) {
                animator.SetBool("isAirborne", true);
            }
            offGroundPosition = transform.position;
        }
        isGrounded = false;
    }

    public void ShowPlayer(bool showPlayer) {
        isAlive = showPlayer;
        playerSprite.enabled = showPlayer;
        healthBar.enabled = showPlayer;
        armourBar.enabled = showPlayer;
        arms.gameObject.GetComponent<SpriteRenderer>().enabled = showPlayer;
    }

    void ThrowGrenade() {
        if (gameManager.HasGrenades()) {
            gameManager.ConsumeGrenade();
            Vector2 mouseScreenPosition = Input.mousePosition;
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector2 direction = (mouseWorldPosition - (Vector2) transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            GrenadeBehaviour projectile = Instantiate(grenadePrefab, transform.position, rotation);
            projectile.speed = 12.5f;
            projectile.sizeMultiplier = 1f;
            projectile.SetSize();
            projectile.SetInitialVelocity(direction * projectile.speed);
        }
    }

    // =========== Health
    // TakeDamage first depletes armour (if it is non-zero) before
    // depleting health.
    public void TakeDamage(float damage) {
        if (Armour > 0) {
            float excessDamage = damage - Armour;
            ReduceArmour(damage);
            if (excessDamage > 0) {
                ReduceHealth(excessDamage);
            }
        } else {
            ReduceHealth(damage);
        }
    }

    // ReduceArmour removes the amount of damage from armour
    private void ReduceArmour(float damage) {
        Armour = Mathf.Clamp(Armour - damage, 0, Resources.MAX_ARMOUR);
        if (Armour == 0) {
            gameManager.DepleteArmour();
        }
        UpdateStatusBar(true);
    }

    // ReduceHealth removes the amount of damage from health
    private void ReduceHealth(float damage) {
        Health = Mathf.Clamp(Health - damage, 0, Resources.MAX_HEALTH);
        UpdateStatusBar(false);
        if (Health == 0) {
            StartCoroutine(Die());
        }
    }

    // UpdateStatusBar renders the new value of armour or health
    // on the health or armour status bars
    private void UpdateStatusBar(bool updateArmour) {
        if (updateArmour) {
            armourBar.transform.localScale = new Vector3(
                Armour / Resources.MAX_ARMOUR,
                armourBar.transform.localScale.y,
                armourBar.transform.localScale.z
            );
        } else {
            healthBar.transform.localScale = new Vector3(
                Health / Resources.MAX_HEALTH,
                healthBar.transform.localScale.y,
                healthBar.transform.localScale.z
            );
        }
    }

    public void PickUpArmour(float increaseAmount) {
        Armour = Mathf.Clamp(Armour + increaseAmount, 0, Resources.MAX_ARMOUR);
        UpdateStatusBar(true);
    }

    public void PickUpHealth(float increaseAmount) {
        Health = Mathf.Clamp(Health + increaseAmount, 0, Resources.MAX_HEALTH);
        UpdateStatusBar(false);
    }

    public bool IsDead() {
        return !isAlive;
    }

    // ========= Weapons
    Vector2 RotateArms() {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the mouse position in world space
        Vector2 direction = (mouseWorldPosition - (Vector2) transform.position).normalized; // Calculate direction from player to mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle in degrees
        armsPivot.rotation = Quaternion.Euler(0, 0, angle);
        // Flip the entire parent object by rotating 180 degrees around the Y axis
        if (angle > 90 || angle < -90) {
            armsPivot.localScale = new Vector3(1, -1, 1); // Flip on Y axis
        } else {
            armsPivot.localScale = new Vector3(1, 1, 1);  // Reset flip
        }
        return direction;
    }

    public void PickUpWeapon(Resources.Weapon newWeapon) {
        arms.PickUpWeapon(newWeapon);
    }

    IEnumerator Stun(float stunTime) {
        isStunned = true;
        animator.SetBool("isStunned", true);
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        animator.SetBool("isStunned", false);
    }

    IEnumerator DoRandomDriveBy() {
        while (!gameManager.IsGameOver()) {
            float randomTime = UnityEngine.Random.Range(30f, 60f);
            Debug.Log($"{randomTime} seconds until next drive by");
            yield return new WaitForSeconds(randomTime);
            Debug.Log("Doing Driveby!");
            if (isAlive) {
                spawnCar.DoDriveBy();
            }
        }
        Debug.Log("DriveBy cancelled");
    }

    // ========= Collision
    private void OnTriggerEnter2D(Collider2D other) {
        Checkpoint check = other.gameObject.GetComponent<Checkpoint>();
        if (check != null) {
            check.Trigger();
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        string tag = collision.gameObject.tag;
        switch (tag) {
            case "Enemy":
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                TakeDamage(enemy.damage);
                StartCoroutine(Stun(enemy.stunTime));
                break;  
            case "BossEnemy":
                ZombieSpawner zombieSpawner = collision.gameObject.GetComponent<ZombieSpawner>();
                if (zombieSpawner != null) {
                    TakeDamage(zombieSpawner.damage);
                }
                break;
        }
    }

    // ========== Coroutines
    // While the player is alive, they will continuously
    // regenerate 'healthTickAmount' every 'healthTickTime'
    private IEnumerator RegenerateHealth() {
        while (!gameManager.IsGameOver()) {
            yield return new WaitForSeconds(healthTickTime);
            if (Health < 100f && !isStunned && isAlive) {
                Health = Mathf.Clamp(Health + healthTickAmount, 0, 100);
                UpdateStatusBar(false);
            }
        }
    }

    // Die runs all processes associated with killing the player
    // It needs to be a coroutine because we wait for the death animation
    // to complete.
    private IEnumerator Die() {
        if (isAlive) {
            isAlive = false; // Block further calls
            gameObject.layer = LayerMask.NameToLayer("Dead");
            playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;

            Health = 0;
            UpdateStatusBar(false);

            audioSource.PlayOneShot(deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)]);
            animator.SetBool("isAirborne", false);
            animator.SetBool("isMoving", false);
            animator.SetBool("isStunned", false);
            arms.gameObject.SetActive(false); // Turn off arms for the death animation
            animator.SetTrigger("die");

            gameManager.AddRevivesCount(); //  Update the death count for scoring
            gameManager.LoseLife();
            
            yield return new WaitForSeconds(deathWaitTime); // Force the player to watch their avatar die horribly
            if (!gameManager.IsGameOver()) {
                gameManager.RespawnPlayer();
            }
        }
    }
}
