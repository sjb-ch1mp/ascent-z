using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 4.5f;
    public float pushForce = 2.5f;
    private Vector2 initialVelocity;  // To store the initial velocity from the player

    public void SetInitialVelocity(Vector2 velocity)
    {
        initialVelocity = initialVelocity + velocity;
    }

    void Start()
    {
        // Combine the base speed of the bullet with the player's velocity
        GetComponent<Rigidbody2D>().velocity = (Vector2)transform.right * speed + initialVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 pushDirection = collision.transform.position - transform.position;
                pushDirection.Normalize();
                rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }

        if (!collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}