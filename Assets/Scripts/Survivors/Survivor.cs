using System.Collections;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    // Export
    public Animator animator;

    // Reference
    CapsuleCollider2D survivorCollider;

    // State
    float health = 100f;

    void Start() {
        survivorCollider = GetComponent<CapsuleCollider2D>();
        int randomSurvivor = Random.Range(1, 12);
        animator.SetInteger("randomSurvivor", randomSurvivor);
    }

    public void DestroyCocoon() {
        animator.SetTrigger("open");
        survivorCollider.enabled = false;
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
