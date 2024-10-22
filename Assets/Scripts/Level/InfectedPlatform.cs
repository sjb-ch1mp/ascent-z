using System.Collections;
using UnityEngine;

public class InfectedPlatform : MonoBehaviour
{
    float damage = 50;
    float health = 1000;
    bool isActive = true;
    AudioSource audioSource;
    BoxCollider2D boxCollider2D;
    Animator animator;
    Rigidbody2D[] bits;
    TutorialManager tutorialManager;
    GameManager gameManager;
    public AudioClip hurtSound;

    void Start() {
        gameManager = GameManager.Instance;
        tutorialManager = TutorialManager.Instance;
        audioSource = GetComponent<AudioSource>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        bits = transform.Find("Bits").gameObject.GetComponentsInChildren<Rigidbody2D>();
    }

    public IEnumerator Die() {
        if (isActive) {
            Debug.Log("Infected platform dead");
            isActive = false;
            audioSource.Play();
            animator.SetTrigger("isDead");
            foreach (Rigidbody2D bit in bits) {
                bit.gravityScale = 2;
                StartCoroutine(DestroyBit(bit.gameObject));
                yield return new WaitForSeconds(0.01f);
            }
            boxCollider2D.enabled = false;
            gameManager.RunScoreRoutine();
        }
    }

    IEnumerator DestroyBit(GameObject bit) {
        yield return new WaitForSeconds(0.5f);
        Destroy(bit);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<Player>().TakeDamage(damage);
            if (!tutorialManager.firstInfectedPlatformEncountered) {
                StartCoroutine(tutorialManager.FirstInfectedPlatformEncounter());
            }
        } else if (other.gameObject.CompareTag("Bullet")) {
            if (!tutorialManager.firstInfectedPlatformEncountered) {
                StartCoroutine(tutorialManager.FirstInfectedPlatformEncounter());
            }
            if (gameManager.AllSpawnersForLevelDead()) {    
                health -= other.gameObject.GetComponent<ProjectileBehaviour>().damage;
                if (health <= 0) {
                    StartCoroutine(Die());
                } else {
                    audioSource.PlayOneShot(hurtSound);
                }
            }
        }
    }
}
