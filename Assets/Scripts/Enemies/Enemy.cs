using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Exports - define different zombie types
    public float damage = 5.0f;
    public float moveSpeed = 3.0f;
    public float health = 100f;
    public float aggroRange = 10f;
    public float energy = 0.33f; // energy determines how often the zombie will change elevation (jump/fall)
    public float jumpPower = 15f;
    public float jumpCooldown = 5.0f;
    public float stunTime = 1.0f;
    public float pushForce = 15f;
    public int score = 10;
    public AudioClip aggroSound;
    public AudioClip dieSound;

    // References
    Vector2 target;
    GameManager gameManager;
    Rigidbody2D enemyRigidbody;
    CapsuleCollider2D enemyCollider;
    Animator animator;
    SpriteRenderer sprite;    
    Transform healthBarDimensions;
    float maxHealthLen;
    AudioSource audioSource;

    // Initial Values
    float initialHealth;
    float initialEnergy;

    // State
    bool isAlive = true;
    bool isStunned = false;
    bool isGrounded = false;
    bool isAggroed = false;
    bool onPlatform = false;
    float elevationOffset = 2.0f;
    int spawnerId;
    Vector2 spawnPoint;

    void Start() {
        gameManager = GameManager.Instance;
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        healthBarDimensions = transform.GetChild(0);
        maxHealthLen = healthBarDimensions.localScale.x;
        initialHealth = health;
        initialEnergy = energy;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(JumpCooldown()); // Set the energy to zero, so zombies don't jump on spawn
        sprite.flipX = Random.value >= 0.5f; // random flip (50/50)
        spawnPoint = transform.position;
    }

    void Update() {
        // Don't move if the game is paused or is finished
        if (gameManager.IsPaused() || gameManager.IsGameOver() || !isAlive || isStunned) {
            return;
        }

        if (GetTarget()) {
            // Get state of zombie for this frame
            bool targetOnRight = target.x > transform.position.x;
            sprite.flipX = !targetOnRight;

            // If the zombie has found itself falling - try to jump a random direction
            if (!isGrounded && energy != 0) {
                Jump();
            }

            // Change elevation if the zombie is energetic enough (on a jump cooldown, the zombie will have 0 energy)
            if (Random.value < energy && isGrounded) {
                if (target.y - elevationOffset > transform.position.y) {
                    // Jump up if not on the actual ground
                    Jump(); 
                } else if (target.y + elevationOffset < transform.position.y) {
                    if (onPlatform) {    
                        // Jump down
                        StartCoroutine(Descend());
                    }
                }
            }

            // Ultimately move towards the current target
            MoveTowardsTarget();
        } else {
            // No target - idle
            if (animator.GetBool("isMoving")) {
                animator.SetBool("isMoving", false);
                animator.SetBool("isStunned", false);
                animator.SetBool("isAirborne", false);
            }
        }
    }

    public bool IsDead() {
        return !isAlive;
    }

    public void Stamp(int id) {
        spawnerId = id;
    }

    public int GetSpawnerId() {
        return spawnerId;
    }

    // PlayerNearby checks if the player is within the aggroRange of the enemy
    bool PlayerNearby(GameObject player) {
        return Physics2D.Distance(GetComponent<CapsuleCollider2D>(), player.GetComponent<CapsuleCollider2D>()).distance <= aggroRange;
    }

    void Jump() {
        enemyRigidbody.velocity = new Vector2(enemyRigidbody.velocity.x, onPlatform ? jumpPower : jumpPower / 2);
        isGrounded = false;
        onPlatform = false;
        animator.SetBool("isAirborne", true);
        StartCoroutine(JumpCooldown());
    }

    // Descend temporarily disables collisions between the enemy and the platform 
    // so that it can drop down to the platform below
    IEnumerator Descend() {
        gameObject.layer = LayerMask.NameToLayer("WallCrawling");
        yield return new WaitForSeconds(0.5f);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    // GetTarget gives a zombie a new target to walk to
    bool GetTarget() {
        // Check player is available
        GameObject player = gameManager.GetPlayer();
        if (player != null && !player.GetComponent<Player>().IsDead() && PlayerNearby(player)) {
            if (!isAggroed) {
                isAggroed = true;
                audioSource.PlayOneShot(aggroSound);
            }
            target = player.transform.position;
            return true;
        }
        // If not, target a random spot around the parent spawner
        if (isAggroed) {
            isAggroed = false;
        }
        ZombieSpawner zombieSpawner = GetParentSpawner();
        if (zombieSpawner != null) {
            target = new Vector2(zombieSpawner.GetRandomXInVicinity(), zombieSpawner.gameObject.transform.position.y);
            return true;
        }
        // Neither the spawner nor player are available
        return false;
    }

    // GetParentSpawner retrives the gameobject of the zombie spawner that 
    // spawned this zombie
    ZombieSpawner GetParentSpawner() {
        ZombieSpawner[] zombieSpawners = FindObjectsOfType<ZombieSpawner>();
        for (int i = 0; i < zombieSpawners.Length; i++) {
            if (zombieSpawners[i] != null && zombieSpawners[i].id == spawnerId) {
                return zombieSpawners[i];
            }
        }
        return null;
    }

    // MoveTowardsTarget causes the zombie to walk towards their current target
    void MoveTowardsTarget() {
        // Calculate the direction towards the player
        Vector2 direction = (target - (Vector2) transform.position).normalized;
        enemyRigidbody.velocity = new Vector2(direction.x * moveSpeed, enemyRigidbody.velocity.y);
        // Ensure the move animation is playing
        if (!animator.GetBool("isMoving")) {
            animator.SetBool("isMoving", true);
        }
    }

    // TakeDamage reduces the enemies health by the damage of the projectile.
    void TakeDamage(ProjectileBehaviour projectile) {
        health -= projectile.damage;
        if (health <= 0) {
            health = 0;
            isAlive = false;
            enemyRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(Die(true));
        }
        UpdateHealthBar();
    }

    // TakeDamage overloaded for interger variable
    // @overload
    void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            health = 0;
            isAlive = false;
            enemyRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(Die(true));
        }
        UpdateHealthBar();
    }

    // TakeDamage reduces the enemies health by the damage of the EXPLOSION.
    void TakeDamageExplosion(ExplosionBehaviour projectile)
    {
        health -= projectile.damage;
        if (health <= 0)
        {
            health = 0;
            isAlive = false;
            enemyRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(Die(true));
        }
        UpdateHealthBar();
    }

    public void KillImmediately() {
        isAlive = false;
        health = 0;
        UpdateHealthBar();
        enemyRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        StartCoroutine(Die(false));
    }

    // UpdateHealthBar calculates the new proportion of max health
    // that should be rendered as the health bar
    void UpdateHealthBar() {
        float newHealthLen = health / initialHealth * maxHealthLen;
        healthBarDimensions.localScale = new Vector3(newHealthLen, healthBarDimensions.localScale.y, healthBarDimensions.localScale.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) {
            return;
        }
        string tag = collision.gameObject.tag;
        switch (tag) {
            case "OneWayPlatform": 
                animator.SetBool("isAirborne", false);
                isGrounded = true;
                onPlatform = true;
                return;
            case "GameBoundary":
                Destroy(gameObject);
                return;
            case "Bullet":
                TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>());
                if (gameManager.GetCurrentWeapon() == Resources.Weapon.BASEBALL_BAT && health > 0) {
                    StartCoroutine(Stun());
                }
                return;
            case "Explosion":
                TakeDamageExplosion(collision.gameObject.GetComponent<ExplosionBehaviour>());
                StartCoroutine(Stun());
                return;
            case "Player":
                StartCoroutine(Attack(collision.gameObject.GetComponent<Rigidbody2D>()));
                return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isGrounded = true;
            onPlatform = false;
            animator.SetBool("isAirborne", false);
        }
    }

    // JumpCooldown enforces a cooldown between jumps so that
    // the enemy doesn't continuously jump
    IEnumerator JumpCooldown() {
        energy = 0;
        yield return new WaitForSeconds(jumpCooldown);
        energy = initialEnergy;
    }

    // Die ensures that the enemy is grounded before playing the
    // death animation so that it doesn't look funky
    IEnumerator Die(bool withScore) {
        gameObject.layer = LayerMask.NameToLayer("Dead");
        audioSource.PlayOneShot(dieSound);
        animator.SetBool("isAirborne", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isStunned", true);
        if (withScore) {
            gameManager.AddKillScore(score);
        }
        float lastY = transform.position.y;
        while (!isGrounded) {
            yield return new WaitForSeconds(0.25f);
            if (lastY == transform.position.y) {
                break; // Collision has failed but the zombie has stopped falling
            } else {
                lastY = transform.position.y;
            }
        }
        animator.SetBool("isStunned", false);
        animator.SetBool("isDead", true);
    }

    // When stunned, the player will not take damage from enemies
    IEnumerator Stun() {
        gameObject.layer = LayerMask.NameToLayer("Stunned");
        // Stun the enemy
        isStunned = true;
        animator.SetBool("isStunned", true);
        // Wait for stun duration
        yield return new WaitForSeconds(stunTime);
        // Unstun enemy
        isStunned = false;
        animator.SetBool("isStunned", false);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    IEnumerator Attack(Rigidbody2D playerRigidBody) {
        animator.SetTrigger("attack");
        audioSource.PlayOneShot(aggroSound);
        playerRigidBody.AddForce((sprite.flipX ? Vector2.left : Vector2.right) * pushForce, ForceMode2D.Impulse);
        // Stun the enemy
        isStunned = true;
        // Wait for stun duration
        yield return new WaitForSeconds(0.5f);
        // Unstun enemy
        isStunned = false;
    }

    // DestroyAfterAnimation is a public function that is 
    // used as an animation Event to destroy the enemy
    public void DestroyAfterAnimation() {
        if (gameManager.LastManStanding()) {
            gameManager.RunScoreRoutine();
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Car")) {
            KillImmediately();
        }
    }
}
 