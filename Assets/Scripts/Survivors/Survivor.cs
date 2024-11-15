using UnityEngine;

public class Survivor : MonoBehaviour
{
    // Export
    public Animator animator;
    public GameObject helicopterPrefab;

    // Reference
    CapsuleCollider2D survivorCollider;
    GameManager gameManager;
    TutorialManager tutorialManager;
    Vector2 helicopterPosn;

    // State
    float health = 100f;
    bool isSaved = false;

    void Start() {
        gameManager = GameManager.Instance;
        tutorialManager = TutorialManager.Instance;
        survivorCollider = GetComponent<CapsuleCollider2D>();
        helicopterPosn = transform.Find("HelicopterPosn").position;
        int randomSurvivor = Random.Range(1, 12);
        animator.SetInteger("randomSurvivor", randomSurvivor);
    }

    public void DestroyCocoon() {
        if (!isSaved) {
            isSaved = true;
            gameManager.AddSurvivorCount();
            Instantiate(helicopterPrefab, helicopterPosn, Quaternion.identity);
            animator.SetTrigger("open");
            survivorCollider.enabled = false;
        }
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Bullet")) {
            StartCoroutine(tutorialManager.FirstCocoonHitEvent());
            ProjectileBehaviour projectile = collision.gameObject.GetComponent<ProjectileBehaviour>();
            health -= projectile.damage;
            if (health <= 0) {
                DestroyCocoon();
            }
        }
    }
}
