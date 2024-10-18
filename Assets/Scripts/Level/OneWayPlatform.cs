
using System.Collections;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private Collider2D playerCollider;

    private void Start() {
        platformCollider = GetComponent<CompositeCollider2D>(); // Collision doesn't work on TileMapCollider2D
        if (platformCollider == null) {
            platformCollider = GetComponent<Collider2D>();
        }
    }

    private void Update() {
        if (playerCollider != null && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))) {
            Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
            StartCoroutine(EnableCollider());
        }
    }

    private IEnumerator EnableCollider() {
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    private void SetPlayerOnPlatform(Collision2D other) {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null) {
            playerCollider = player.gameObject.GetComponent<CapsuleCollider2D>();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        SetPlayerOnPlatform(other);
    }

    private void OnCollisionExit2D(Collision2D other) {
        SetPlayerOnPlatform(other);
    }

    /*
    private static Collider2D playerCollider;
    public Collider2D platformCollider;
    private bool is_off;
    
    public void Enter(Collider2D player)
    {
        if (playerCollider == null)
        {
            playerCollider = player;
        }

        is_off = true;
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (is_off)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                is_off = false;
                if (playerCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
                }
            }
        }
    }*/
}
