using System.Collections;
using System.Collections.Generic;
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

    // References
    GameManager gameManager;
    GameObject player;
    Animator animator;  
    Transform healthBarDimensions;
    float maxHealthLen;
    Transform spawnBoundLeft;
    Transform spawnBoundRight;
    GameObject zombieContainer;

    // Initial state
    float initialHealth;

    // State
    bool isActive;
    bool isAlive = true;
    bool isDesperate = false;
    float spawnRate = 3.0f;
    int id;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameManager.Instance;
        id = gameManager.GetNewZombieSpawnerId();
        zombieContainer = GameObject.Find("Zombies");
        player = gameManager.GetPlayer();
        animator = GetComponent<Animator>();
        initialHealth = health;
        spawnBoundLeft = transform.GetChild(0);
        spawnBoundRight = transform.GetChild(1);
        healthBarDimensions = transform.GetChild(2);
        maxHealthLen = healthBarDimensions.localScale.x;
        StartCoroutine(SpawnZombie());
    }

    // PlayerNearby checks if the player is within the aggroRange of the enemy
    bool PlayerNearby() {
        if (player == null)
        {
            player = gameManager.GetPlayer();
            if (player == null)
            {
                return false;
            }
        }

        return Physics2D.Distance(
            GetComponent<BoxCollider2D>(), 
            player.GetComponent<BoxCollider2D>()
        ).distance <= aggroRange;
    }

    IEnumerator SpawnZombie() {
        while (isAlive) {
            yield return new WaitForSeconds(isActive ? spawnRate : 0.5f);
            // When a spawner is about to die, it's spawn rate is significantly increased
            if (!isDesperate && health / initialHealth < 0.25) {
                spawnRate = 1.5f;
                isDesperate = true;
            }
            // Only spawn if the game isn't paused
            if (!gameManager.IsPaused() && !gameManager.IsGameOver()) {
                if (isActive && !gameManager.SpawnCapReached(id)) {
                    animator.SetTrigger("spawn-zombie");
                    float randomSpawnX = Random.Range(spawnBoundLeft.position.x, spawnBoundRight.position.x);
                    float randomZombieThreshold = Random.value;
                    GameObject zombie = null;
                    if (randomZombieThreshold <= zombieSpawnThresholds[0]) {
                        zombie = Instantiate(zombies[0], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[0].transform.rotation, zombieContainer.transform);
                    } else if (randomZombieThreshold <= zombieSpawnThresholds[1]) {
                        zombie = Instantiate(zombies[1], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[1].transform.rotation, zombieContainer.transform);
                    } else {
                        zombie = Instantiate(zombies[2], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[2].transform.rotation, zombieContainer.transform);
                    }
                    if (zombie != null /*If not dead immediately*/) {
                        zombie.GetComponent<Enemy>().Stamp(id);
                    }
                } else {
                    // If player isn't in range - check for the player
                    isActive = PlayerNearby();
                }
            }
        }
    }

    // TakeDamage reduces the enemies health by the damage of the projectile.
    void TakeDamage(ProjectileBehaviour projectile) {
        health -= projectile.damage;
        if (health <= 0) {
            health = 0;
            isAlive = false;
            Die();
        }
        UpdateHealthBar();
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
        // Kill all the spawn for this spawner to give the player some breathing room
        gameManager.KillZombiesForSpawner(id);
        gameManager.AddKillScore(score);
        gameObject.layer = LayerMask.NameToLayer("Dead");
        animator.SetBool("isDead", true);
    }

    // UpdateHealthBar calculates the new proportion of max health
    // that should be rendered as the health bar
    void UpdateHealthBar() {
        float newHealthLen = health / initialHealth * maxHealthLen;
        healthBarDimensions.localScale = new Vector3(newHealthLen, healthBarDimensions.localScale.y, healthBarDimensions.localScale.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) {
            return;
        }
        string tag = collision.gameObject.tag;
        switch (tag) {
            case "Bullet":
                TakeDamage(collision.gameObject.GetComponent<ProjectileBehaviour>());
                break;
            case "Explosion":
                TakeDamageExplosion(collision.gameObject.GetComponent<ExplosionBehaviour>());
                break;
        }
    }

    // DestroyAfterAnimation is a public function that is 
    // used as an animation Event to destroy the enemy
    public void DestroyAfterAnimation() {
        gameManager.RunScoreRoutine();
        Destroy(gameObject);
    }
}
