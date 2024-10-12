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

    public GameObject spriteHolder;  // Reference to the child object with SpriteRenderer


    /*void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }*/

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

    void Flip()
    {
        // Flip the player's facing direction
        isFacingRight = !isFacingRight;

        // Flip the sprite by scaling it on the X axis
        Vector3 scale = spriteHolder.transform.localScale;
        scale.x *= -1;  // Reverse the X axis
        spriteHolder.transform.localScale = scale;
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
