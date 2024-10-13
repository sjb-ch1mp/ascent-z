using UnityEngine;
using System.Collections.Generic; // For using the List to track enemies

public class ExplosionBehaviour : MonoBehaviour
{
    public float pushForce = 2.5f;
    public float speed;
    public float damage;
    public float sizeMultiplier;
    public float penetration = 0;
    public float maxRange = Mathf.Infinity; // Default to no range limit
    public float lifeDuration = 5f;
    public bool visible = true;

    public bool playAnimation = false;
    public Animation explosionAnimation;

    private Vector2 lockedVelocity;  // Store the initial velocity
    private HashSet<GameObject> collidedEnemies = new HashSet<GameObject>();  // Track enemies already hit
    private Collider2D projectileCollider;  // Reference to the projectile's collider
    private Vector2 initialPosition; // To track how far the projectile has traveled

    private SpriteRenderer spriteRenderer;  // Reference to the SpriteRenderer
    public Sprite projectileSprite;        // Public variable for the sprite

    private float timeElapsed = 0f; // Track how long the object has existed

    void Start()
    {
        // Cache the projectile's collider to use it later for ignoring collisions
        projectileCollider = GetComponent<Collider2D>();
        explosionAnimation = GetComponent<Animation>();

        // Store the initial position of the projectile
        initialPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (projectileSprite != null && visible)
        {
            spriteRenderer.sprite = projectileSprite;
        }


    }

    public void SetProjectileSprite(Sprite newSprite)
    {
        projectileSprite = newSprite;
    }

    public void SetInitialVelocity(Vector2 velocity)
    {
        lockedVelocity = velocity;  // Store the velocity
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    public void SetSize()
    {
        transform.localScale = transform.localScale * sizeMultiplier;
    }

    private void FixedUpdate()
    {
        // Lock the velocity so it doesn't change
        GetComponent<Rigidbody2D>().velocity = lockedVelocity;

        spriteRenderer.enabled = visible;


        // Check if projectile has exceeded max range
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }

        // Increment the timeElapsed by the time that has passed since the last frame
        timeElapsed += Time.deltaTime;
        // Check if the object has existed longer than lifeDuration
        if (timeElapsed >= lifeDuration)
        {
            Destroy(gameObject); // Destroy the object if it has exceeded its lifeDuration
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object is an enemy and if it hasn't been hit before
        if (collision.gameObject.CompareTag("Enemy") && !collidedEnemies.Contains(collision.gameObject))
        {
            collidedEnemies.Add(collision.gameObject);  // Mark the enemy as hit

            // Ignore further collisions between this projectile and this enemy
            Collider2D enemyCollider = collision.gameObject.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(projectileCollider, enemyCollider);
            }

            // Apply force to the enemy
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 pushDirection = collision.transform.position - transform.position;
                pushDirection.Normalize();
                rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }

            // You can also apply damage to the enemy here if needed.
        }

        // Handle penetration and destruction
        if (!collision.gameObject.CompareTag("Bullet"))
        {
            if (penetration <= 0)
            {
                Destroy(gameObject); // Destroy the projectile after hitting an object
            }
            else
            {
                penetration--; // Decrease penetration count
            }
        }
    }
}