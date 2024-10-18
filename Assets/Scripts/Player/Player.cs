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
    public GrenadeBehaviour grenadePrefab;
    public PlayerArms arms;

    // References
    GameManager gameManager;
    Animator animator;
    Transform armsPivot;
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSprite;
    SpriteRenderer healthBar;
    SpriteRenderer armourBar;

    // State
    bool isAlive = true;
    bool isStunned = false;
    bool isGrounded = false;
    int jumpCount = 0;
    public int Lives { get; set; }
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
        // Start Coroutines
        StartCoroutine(RegenerateHealth());
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
                playerRigidBody.velocity = new Vector2(movementDirection * moveSpeed, playerRigidBody.velocity.y);
            }
        }
    }
    void Update() {
        if (ContinueUpdate()) {

            // Sprite positioning
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerSprite.flipX = mouseWorldPosition.x < transform.position.x;
            arms.SetAngle(RotateArms());

            // Jumping
            if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))) {
                Jump();
            }

            // Firing
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                arms.FireWeapon();
            } else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                ThrowGrenade();
            }
        }
    }

    // ========== Movement
    void Jump()
    {
        if (jumpCount < extraJump) {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpPower);  // Apply the jump force
            jumpCount++;
        }
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
    // Alternative function to kill the player immediately (e.g. 
    // if they fall out of the boundary)
    public void KillImmediately() {

    }
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

    public void AddLife() {
        Lives = Mathf.Clamp(Lives + 1, 0, Resources.MAX_LIVES);
        gameManager.RenderLives(Lives);
    }

    // ========= Weapons
    float RotateArms() {
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
        return angle;
    }

    public void PickUpWeapon(Resources.Weapon newWeapon) {
        arms.PickUpWeapon(newWeapon);
    }

    // ========= Collision
    private void OnTriggerEnter2D(Collider2D other) {
        Checkpoint check = other.gameObject.GetComponent<Checkpoint>();
        if (check != null) {
            check.Trigger();
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            jumpCount = 0;
            isGrounded = true;
        }
    }

    // ========== Coroutines
    // While the player is alive, they will continuously
    // regenerate 'healthTickAmount' every 'healthTickTime'
    private IEnumerator RegenerateHealth() {
        while (isAlive) {
            yield return new WaitForSeconds(healthTickTime);
            if (Health < 100f) {
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
            CameraTracking.Instance.DisableTrigger(); // Stop tracking player with camera
            gameManager.AddRevivesCount(); //  Update the death count for scoring
            Lives = Mathf.Clamp(Lives - 1, 0, Resources.MAX_LIVES); // Update lives
            gameManager.RenderLives(Lives);
            arms.gameObject.SetActive(false); // Turn off arms for the death animation
            animator.SetBool("isDead", true); // Start the death animation
            yield return new WaitForSeconds(deathWaitTime); // Force the player to watch their avatar die horribly
            if (Lives == 0) {
                gameManager.GameOver();
            } else {
                gameManager.SpawnPlayer();
            }
        }
    }
}
