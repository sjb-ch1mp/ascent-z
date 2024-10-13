using UnityEngine;
using System.Collections.Generic; // For using the List to track enemies
using System.Collections;


public class GrenadeBehaviour : MonoBehaviour
{
    public float pushForce = 2.5f;
    public float speed;
    public float damage;
    public float sizeMultiplier;
    public float penetration = 0;
    public float maxRange = Mathf.Infinity; // Default to no range limit

    private Vector2 lockedVelocity;  // Store the initial velocity
    private Collider2D projectileCollider;  // Reference to the projectile's collider
    private Vector2 initialPosition; // To track how far the projectile has traveled

    private SpriteRenderer spriteRenderer;  // Reference to the SpriteRenderer


    public Sprite explosionSprite;        // Public variable for the sprite
    public ExplosionBehaviour explosionBehaviour;


    void Start()
    {
        // Cache the projectile's collider to use it later for ignoring collisions
        projectileCollider = GetComponent<Collider2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // Store the initial position of the projectile
        initialPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

 

        // Ignore collision with the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(projectileCollider, playerCollider);
            }
        }

        StartCoroutine(DestroyAfterDelay(3f)); // Start coroutine to destroy after 3 seconds

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

        // Check if projectile has exceeded max range
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object is an enemy
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("OneWayPlatform") || collision.gameObject.CompareTag("Bullet"))
        {
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
    }

    // Coroutine to handle destruction after a delay
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        Debug.Log("Kaboom!"); // Display your custom message
        ExplosionBehaviour projectile = Instantiate(explosionBehaviour, transform.position, Quaternion.identity);
        //projectile.SetProjectileSprite(projectileExplosionSprite);
        projectile.lifeDuration = 1f;
        projectile.damage = 200;
        projectile.sizeMultiplier = 1;
        projectile.penetration = Mathf.Infinity;
        projectile.playAnimation = true;
        projectile.SetSize();

        Destroy(gameObject); // Then destroy the object
    }
}
