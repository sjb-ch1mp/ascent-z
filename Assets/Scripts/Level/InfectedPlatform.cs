using System.Collections;
using UnityEngine;

public class InfectedPlatform : MonoBehaviour
{
    [SerializeField] private float damage = 50;
    [SerializeField] private float health = 1000;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private Components comps;

    TutorialManager tutorialManager;
    GameManager gameManager;

    private bool isActive = true;

    void Start() {
        gameManager = GameManager.Instance;
        tutorialManager = TutorialManager.Instance;
    }

    public void Die() {
        if (isActive) {
            Debug.Log("Infected platform dead");
            isActive = false;
            comps.audioSource.Play();
            comps.animator.SetTrigger("isDead");
            comps.mushrooms.gameObject.SetActive(true);
            comps.mushrooms.Split();
            comps.bits.gameObject.SetActive(true);
            comps.bits.Split();
            comps.collider.enabled = false;
            gameManager.RunScoreRoutine();
        }
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
                    Die();
                } else {
                    comps.audioSource.PlayOneShot(hurtSound);
                }
            }
        }
    }

    [System.Serializable]
    struct Components
    {
        public BitsContainer mushrooms;
        public BitsContainer bits;
        public AudioSource audioSource;
        public Collider2D collider;
        public Animator animator;
    }
}
