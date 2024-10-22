using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{

    // Exports
    public GameObject[] zombies;
    public float[] zombieSpawnThresholds;
    public float aggroRange = 50.0f;
    public float health = 2000f;
    public int score = 250;
    public float damage = 30;
    public int surgeZombies = 10;
    public float normalSpawnRate = 5;
    public float firstHitSpawnRate = 4;
    public float desperateSpawnRate = 1.5f; 
    public AudioClip deathSound;
    public AudioClip[] painSounds;

    // References
    GameManager gameManager;
    GameObject player;
    Animator animator;  
    Transform healthBarDimensions;
    float maxHealthLen;
    Transform spawnBoundLeft;
    Transform spawnBoundRight;
    GameObject zombieContainer;
    TutorialManager tutorialManager;
    AudioSource audioSource;

    // Initial state
    float initialHealth;

    // State
    bool isActive;
    bool isAlive = true;
    bool isHit = false;
    bool isDesperate = false;
    float spawnRate;
    
    public int id;

    // Start is called before the first frame update
    void Start() {
        spawnRate = normalSpawnRate;
        gameManager = GameManager.Instance;
        tutorialManager = TutorialManager.Instance;
        id = gameManager.GetNewZombieSpawnerId();
        zombieContainer = GameObject.Find("Zombies");
        player = gameManager.GetPlayer();
        animator = GetComponent<Animator>();
        initialHealth = health;
        spawnBoundLeft = transform.GetChild(0);
        spawnBoundRight = transform.GetChild(1);
        healthBarDimensions = transform.GetChild(2);
        maxHealthLen = healthBarDimensions.localScale.x;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnZombie());
    }

    // PlayerNearby checks if the player is within the aggroRange of the enemy
    bool PlayerNearby() {
        if (player == null) {
            player = gameManager.GetPlayer();
            if (player == null) {
                return false;
            }
        }

        return Physics2D.Distance(
            GetComponent<CapsuleCollider2D>(), 
            player.GetComponent<CapsuleCollider2D>()
        ).distance <= aggroRange;
    }

    IEnumerator SpawnZombie() {
        while (isAlive) {
            yield return new WaitForSeconds(isActive ? spawnRate : 0.5f);
            // When a spawner is about to die, it's spawn rate is significantly increased
            if (!isDesperate && health / initialHealth < 0.25) {
                spawnRate = desperateSpawnRate;
                isDesperate = true;
                StartCoroutine(DoSurge(false));
            }
            // Only spawn if the game isn't paused
            if (!gameManager.IsPaused() && !gameManager.IsGameOver()) {
                if (isActive && !gameManager.SpawnCapReached(id)) {
                    if (gameManager.NoZombiesForSpawner(id) && isHit) {
                        StartCoroutine(DoSurge(true));
                    } else {
                        InstantiateZombie();
                    }
                } else {
                    // If player isn't in range - check for the player
                    isActive = PlayerNearby();
                }
            }
        }
    }

    IEnumerator DoSurge(bool miniSurge) {
        int numZombies = miniSurge ? surgeZombies/2 : surgeZombies;
        for (int i=0; i<numZombies; i++) {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
            InstantiateZombie();
        }
    }
    
    public bool IsDead() {
        return !isAlive;
    }

    void InstantiateZombie() {
        animator.SetTrigger("spawn-zombie");
        float randomSpawnX = Random.Range(spawnBoundLeft.position.x, spawnBoundRight.position.x);
        float randomZombieThreshold = Random.value;
        GameObject zombie = null;
        if (randomZombieThreshold <= zombieSpawnThresholds[0]) {
            zombie = Instantiate(zombies[0], new Vector3(randomSpawnX, spawnBoundLeft.position.y, transform.position.z), zombies[0].transform.rotation, zombieContainer.transform);
        } else if (randomZombieThreshold <= zombieSpawnThresholds[1]) {
            zombie = Instantiate(zombies[1], new Vector3(randomSpawnX, spawnBoundLeft.position.y, transform.position.z), zombies[1].transform.rotation, zombieContainer.transform);
        } else {
            zombie = Instantiate(zombies[2], new Vector3(randomSpawnX, spawnBoundLeft.position.y, transform.position.z), zombies[2].transform.rotation, zombieContainer.transform);
        }
        if (zombie != null /*If not dead immediately*/) {
            zombie.GetComponent<Enemy>().Stamp(id);
        }
    }

    // TakeDamage reduces the enemies health by the damage of the projectile.
    void TakeDamage(ProjectileBehaviour projectile) {
        TakeDamage((int) projectile.damage);
    }

    // TakeDamage reduces the enemies health by the damage of the projectile.
    public void TakeDamage(int damage) {
        audioSource.PlayOneShot(painSounds[Random.Range(0, painSounds.Length)]);
        health -= damage;
        if (health <= 0) {
            health = 0;
            isAlive = false;
            Die();
        }
        UpdateHealthBar();
    }

    public float GetRandomXInVicinity() {
        return Random.Range(spawnBoundLeft.position.x, spawnBoundRight.position.x);
    }

    // TakeDamage reduces the enemies health by the damage of the EXPLOSION.
    void TakeDamageExplosion(ExplosionBehaviour projectile)
    {
        health -= projectile.damage;
        if (health <= 0)
        {
            health = 0;
            isAlive = false;
            Die();
        }
        UpdateHealthBar();
    }

    void Die() {
        gameManager.KillZombiesForSpawner(id);
        audioSource.PlayOneShot(deathSound);
        gameManager.AddKillScore(score);
        gameObject.layer = LayerMask.NameToLayer("Dead");
        animator.SetBool("isDead", true);
        Cache cache = GetComponent<Cache>();
        if (cache != null) {
            StartCoroutine(cache.RuptureCache());
        }
    }

    // UpdateHealthBar calculates the new proportion of max health
    // that should be rendered as the health bar
    void UpdateHealthBar() {
        float newHealthLen = health / initialHealth * maxHealthLen;
        healthBarDimensions.localScale = new Vector3(newHealthLen, healthBarDimensions.localScale.y, healthBarDimensions.localScale.z);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) {
            return;
        }
        string tag = collision.gameObject.tag;
        switch (tag) {
            case "Bullet":
                TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>());
                if (!isHit) {
                    isHit = true;
                    spawnRate = firstHitSpawnRate; // Increase spawn rate slightly on first hit
                    StartCoroutine(DoSurge(false));
                    StartCoroutine(tutorialManager.SpawnerFirstHitEvent());
                }
                break;
            case "Explosion":
                TakeDamageExplosion(collision.gameObject.GetComponent<ExplosionBehaviour>());
                break;
        }
    }

    // DestroyAfterAnimation is a public function that is 
    // used as an animation Event to destroy the enemy
    public void DestroyAfterAnimation() {
        //gameManager.CheckLevelComplete();
        Destroy(gameObject);
    }
}
