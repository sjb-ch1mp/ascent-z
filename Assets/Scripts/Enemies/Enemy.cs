using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Exports - define different zombie types
    public float moveSpeed = 3.0f;
    public float health = 100f;
    public float aggroRange = 10f;
    public float energy = 0.33f; // energy determines how often the zombie will change elevation (jump/fall)
    public float jumpPower = 15f;
    public float jumpCooldown = 5.0f;
    public float stunTime = 1.0f;
    public int score = 10;

    // References
    GameObject player;
    GameManager gameManager;
    Rigidbody2D enemyRigidbody;
    CapsuleCollider2D enemyCollider;
    Animator animator;
    SpriteRenderer sprite;    
    Transform healthBarDimensions;
    float maxHealthLen;

    // Initial Values
    float initialHealth;
    float initialEnergy;

    // State
    bool isAlive = true;
    bool isStunned = false;
    bool detectedPlayer = false;
    Collider2D onPlatform;
    float elevationOffset = 1.0f;
    int spawnerId;

    void Start() {
        gameManager = GameManager.Instance;
        player = gameManager.GetPlayer();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        healthBarDimensions = transform.GetChild(0);
        maxHealthLen = healthBarDimensions.localScale.x;
        initialHealth = health;
        initialEnergy = energy;
        StartCoroutine(JumpCooldown()); // Set the energy to zero, so zombies don't jump on spawn
        sprite.flipX = Random.value >= 0.5f; // random flip (50/50)
    }

    void Update() {
        // Don't move if the game is paused or is finished
        if (gameManager.IsPaused() || gameManager.IsGameOver() || !isAlive || isStunned) {
            return;
        }

        // Get state of zombie for this frame
        bool isGrounded = onPlatform != null;
        bool playerOnRight = player.transform.position.x > transform.position.x;
        sprite.flipX = !playerOnRight;

        // Do movement based on state
        if (detectedPlayer) {
            // If the zombie has found itself falling - try to jump a random direction
            if (!isGrounded && energy != 0) {
                Jump();
            }

            // Change elevation if the zombie is energetic enough (on a jump cooldown, the zombie will have 0 energy)
            if (Random.value < energy && isGrounded) {
                if (player.transform.position.y - elevationOffset > transform.position.y) {
                    // Jump up
                    Jump();
                } else if (player.transform.position.y + elevationOffset < transform.position.y) {
                    // Jump down
                    StartCoroutine(Descend());
                }
            }

            // Ultimately move towards the player
            MoveTowardsPlayer();
        } else {
            // Check for player
            detectedPlayer = PlayerNearby();
        }
    }

    public void Stamp(int id) {
        spawnerId = id;
    }

    public int GetSpawnerId() {
        return spawnerId;
    }

    // PlayerNearby checks if the player is within the aggroRange of the enemy
    bool PlayerNearby() {
        return Physics2D.Distance(
            GetComponent<CapsuleCollider2D>(), 
            player.GetComponent<CapsuleCollider2D>()
        ).distance <= aggroRange;
    }

    void Jump() {
        enemyRigidbody.velocity = new Vector2(enemyRigidbody.velocity.x, jumpPower);
        onPlatform = null;
        animator.SetBool("isAirborne", true);
        StartCoroutine(JumpCooldown());
    }

    // Descend temporarily disables collisions between the enemy and the platform 
    // so that it can drop down to the platform below
    IEnumerator Descend() {
        // Cache local collider
        Collider2D platformCollider = onPlatform;
        Physics2D.IgnoreCollision(enemyCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(enemyCollider, platformCollider, false);
    }

    // MoveTowardsPlayer causes the zombie to walk towards the player
    void MoveTowardsPlayer() {
        // Calculate the direction towards the player
        Vector2 direction = (player.transform.position - transform.position).normalized;
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
                if (onPlatform == null) {
                    onPlatform = collision.collider;
                }
                break;
            case "GameBoundary":
                Destroy(gameObject);
                break;
            case "Bullet":
                TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>());
                if (gameManager.GetCurrentWeapon() == Resources.Weapon.BASEBALL_BAT && health > 0) {
                    StartCoroutine(Stun());
                }
                break;
            case "Explosion":
                TakeDamageExplosion(collision.gameObject.GetComponent<ExplosionBehaviour>());
                StartCoroutine(Stun());
                break;
            case "Player":
                animator.SetTrigger("attack");
                break;
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
        animator.SetBool("isAirborne", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isStunned", true);
        if (withScore) {
            gameManager.AddKillScore(score);
        }
        gameObject.layer = LayerMask.NameToLayer("Dead");
        float lastY = transform.position.y;
        while (onPlatform == null) {
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
        CapsuleCollider2D playerCollider = player.GetComponent<CapsuleCollider2D>();
        // Stun the enemy
        isStunned = true;
        animator.SetBool("isStunned", true);
        Physics2D.IgnoreCollision(enemyCollider, playerCollider);
        // Wait for stun duration
        yield return new WaitForSeconds(stunTime);
        // Unstun enemy
        isStunned = false;
        animator.SetBool("isStunned", false);
        Physics2D.IgnoreCollision(enemyCollider, playerCollider, false);
    }

    // DestroyAfterAnimation is a public function that is 
    // used as an animation Event to destroy the enemy
    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }

    public bool IsGrounded() {
        return onPlatform != null;
    }
}
 