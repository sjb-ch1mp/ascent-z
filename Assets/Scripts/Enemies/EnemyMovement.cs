using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float jumpPower = 15f;
    public LayerMask groundLayer;
    public Transform feet;
    public float dropDelay = 0.25f;
    public float reactionTime = 0.5f;
    public float pushCooldown = 0.5f;
    public float pushForce = 5.0f;  // Force applied to the player when collided

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isGrounded;
    private GameObject currentOneWayPlatform;
    private float nextActionTime;
    private float pushCooldownTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        nextActionTime = Time.time + reactionTime;
        pushCooldownTime = 0f;
    }

    void Update()
    {
        if (Time.time >= pushCooldownTime)
        {
            MoveTowardsPlayer();
        }

        if (playerTransform != null && Time.time >= nextActionTime)
        {
            CheckGrounded();

            if (isGrounded && Random.value < 0.33f)
            {
                if (playerTransform.position.y > transform.position.y + 1.0f)
                {
                    Jump();
                }
                else if (playerTransform.position.y < transform.position.y - 1.0f && currentOneWayPlatform != null)
                {
                    StartCoroutine(DropThroughPlatform());
                }
            }

            nextActionTime = Time.time + reactionTime;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(feet.position, 0.5f, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }

        if (collision.gameObject.CompareTag("GameBoundary"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            pushCooldownTime = Time.time + pushCooldown;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DropThroughPlatform()
    {
        if (currentOneWayPlatform != null)
        {
            BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
            Collider2D enemyCollider = GetComponent<Collider2D>();

            Physics2D.IgnoreCollision(enemyCollider, platformCollider);

            rb.velocity = new Vector2(rb.velocity.x, -jumpPower / 2);

            yield return new WaitForSeconds(dropDelay);

            Physics2D.IgnoreCollision(enemyCollider, platformCollider, false);
        }
    }
}