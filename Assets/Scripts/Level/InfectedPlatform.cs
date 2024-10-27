using System.Collections.Generic;
//using System.Collections;
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
    private bool isInvulnerable = true;
    private int numSpawners = 0;
    private float initialHealth;
    private float maxHealthLen;
    private List<LevelActivated> levelComponents = new List<LevelActivated>();

    void Awake()
    {
        ContactFilter2D contactFilter = new ContactFilter2D().NoFilter();
        List<Collider2D> colliders = new List<Collider2D>();

        comps.levelZone.OverlapCollider(contactFilter, colliders);

        foreach (Collider2D collider in  colliders)
        {
            LevelActivated la = collider.GetComponent<LevelActivated>();
            if (la != null)
            {
                levelComponents.Add(la);
                ZombieSpawner zs = la.GetComponent<ZombieSpawner>();
                if (zs != null)
                {
                    numSpawners++;
                    zs.Link(this);
                }
            }
        }
        comps.levelZone.gameObject.SetActive(false);
    }

    void Start() {
        gameManager = GameManager.Instance;
        tutorialManager = TutorialManager.Instance;
        initialHealth = health;
        maxHealthLen = comps.healthBarDimensions.localScale.x;
    }

    public void Die() {
        if (isActive) {
            comps.healthBarDimensions.gameObject.SetActive(false);
            Debug.Log("Infected platform dead");
            isActive = false;
            comps.audioSource.Play();
            comps.animator.SetTrigger("isDead");
            comps.bits.gameObject.SetActive(true);
            comps.bits.Split();
            comps.collider.enabled = false;
            gameManager.RunScoreRoutine();
        }
    }

    public void DieSilent()
    {
        if (isActive)
        {
            Debug.Log("Infected platform Silent Dead");
            comps.healthBarDimensions.gameObject.SetActive(false);
            isActive = false;
            comps.animator.SetTrigger("isDead");
            comps.collider.enabled = false;
        }
    }

    public void Activate()
    {
        foreach (LevelActivated la in levelComponents)
        {
            if (la != null)
            {
                la.Activate();
            }
        }
    }

    public void Deactivate()
    {
        foreach (LevelActivated la in levelComponents)
        {
            if (la != null)
            {
                la.Deactivate();
            }
        }
    }

    public void Notify()
    {
        numSpawners--;
        if (numSpawners == 0)
        {
            comps.mushrooms.gameObject.SetActive(true);
            comps.mushrooms.Split();
            comps.animator.SetTrigger("isActive");
            comps.healthBarDimensions.gameObject.SetActive(true);
            isInvulnerable = false;
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
            if (!isInvulnerable) { //gameManager.AllSpawnersForLevelDead()) {    
                health -= other.gameObject.GetComponent<ProjectileBehaviour>().damage;
                if (health <= 0) {
                    Die();
                } else {
                    UpdateHealthBar();
                    comps.audioSource.PlayOneShot(hurtSound);
                }
            }
        }
    }

    // UpdateHealthBar calculates the new proportion of max health
    // that should be rendered as the health bar
    void UpdateHealthBar()
    {
        float newHealthLen = health / initialHealth * maxHealthLen;
        comps.healthBarDimensions.localScale = new Vector3(newHealthLen, comps.healthBarDimensions.localScale.y, comps.healthBarDimensions.localScale.z);
    }

    [System.Serializable]
    struct Components
    {
        public BitsContainer mushrooms;
        public BitsContainer bits;
        public AudioSource audioSource;
        public Collider2D collider;
        public Animator animator;
        public Collider2D levelZone;
        public Transform healthBarDimensions;
    }
}
