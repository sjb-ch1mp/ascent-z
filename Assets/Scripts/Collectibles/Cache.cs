using System.Collections;
using TMPro;
using UnityEngine;

public class Cache : MonoBehaviour {

    // Exports
    public Size cacheSize = Size.MEGA;
    public GameObject utilityCratePrefab;
    public GameObject weaponCachePrefab;
    public float leftDropBound = -2;
    public float rightDropBound = 2;
    public Transform collectibleContainer;
    public bool attached = false; // If cache is attached to an entity (e.g. zombie spawner) don't do animation or audioclip, because they don't exist

    // Types
    public enum Size {
        MEGA,
        NORMAL,
        MINOR
    }

    // Consts
    private const int MEGA_DROP = 8;
    private const int NORMAL_DROP = 4;
    private const int MINOR_DROP = 2;
    private const int MEGA_HEALTH = 250;
    private const int NORMAL_HEALTH = 100;
    private const int MINOR_HEALTH = 50;
    private const float dropDelay = 0.1f;

    // State
    float health = MEGA_HEALTH;
    int dropNumber = MEGA_DROP;
    bool isRuptured = false;

    // References
    Animator animator;
    AudioSource audioSource;

    void Start() {
        switch (cacheSize) {
            case Size.MINOR:
                health = MINOR_HEALTH;
                dropNumber = MINOR_DROP;
                break;
            case Size.NORMAL:
                health = NORMAL_HEALTH;
                dropNumber = NORMAL_DROP;
                break;
            default: //MEGA
                health = MEGA_HEALTH;
                dropNumber = MEGA_DROP;
                break;
        }
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (animator == null) {
            attached = true;
        }
    }

    public IEnumerator RuptureCache() {
        if (!isRuptured) {
            isRuptured = true;
            if (!attached) {
                animator.SetTrigger("destroy");
                audioSource.Play();
            }
            for (int i=0; i<dropNumber; i++) {
                Vector2 randomDropPosition = new Vector2(transform.position.x + Random.Range(leftDropBound, rightDropBound), transform.position.y);
                if (Random.value < 0.25) {
                    Instantiate(weaponCachePrefab, randomDropPosition, weaponCachePrefab.transform.rotation, collectibleContainer);
                } else {
                    Instantiate(utilityCratePrefab, randomDropPosition, weaponCachePrefab.transform.rotation, collectibleContainer);
                }
                yield return new WaitForSeconds(dropDelay);
            }
            if (!attached) {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (attached) {
            // Check if we're attached to a zombie spawner
            ZombieSpawner z = GetComponent<ZombieSpawner>();
            if (z != null) {
                z.OnCollisionEnter2D(other);
            }
        } else if (other.gameObject.CompareTag("Bullet")) {
            health -= other.gameObject.GetComponent<ProjectileBehaviour>().damage;
            if (health <= 0) {
                StartCoroutine(RuptureCache());
            }
            Destroy(other.gameObject);
        }
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }
}
