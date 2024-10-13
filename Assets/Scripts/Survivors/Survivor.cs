using UnityEngine;

public class Survivor : MonoBehaviour
{
    // Export
    public Animator animator;

    // Reference
    CapsuleCollider2D survivorCollider;
    GameManager gameManager;

    // State
    float health = 100f;
    bool isSaved = false;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        survivorCollider = GetComponent<CapsuleCollider2D>();
        int randomSurvivor = Random.Range(1, 12);
        animator.SetInteger("randomSurvivor", randomSurvivor);
    }

    public void DestroyCocoon() {
        if (!isSaved) {
            isSaved = true;
            gameManager.AddSurvivorCount();
            animator.SetTrigger("open");
            survivorCollider.enabled = false;
        }
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Bullet")) {
            ProjectileBehaviour projectile = collision.gameObject.GetComponent<ProjectileBehaviour>();
            health -= projectile.damage;
            if (health <= 0) {
                DestroyCocoon();
            }
        }
    }
}
