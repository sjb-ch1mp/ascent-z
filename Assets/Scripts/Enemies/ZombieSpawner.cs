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

    // Initial state
    float initialHealth;

    // State
    bool isActive;
    bool isAlive = true;
    bool isDesperate = false;
    float spawnRate = 3.0f;
    List<Enemy> spawn = new List<Enemy>();

    // Start is called before the first frame update
    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
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
                if (isActive) {
                    animator.SetTrigger("spawn-zombie");
                    float randomSpawnX = Random.Range(spawnBoundLeft.position.x, spawnBoundRight.position.x);
                    float randomZombieThreshold = Random.value;
                    GameObject zombie = null;
                    if (randomZombieThreshold <= zombieSpawnThresholds[0]) {
                        zombie = Instantiate(zombies[0], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[0].transform.rotation);
                    } else if (randomZombieThreshold <= zombieSpawnThresholds[1]) {
                        zombie = Instantiate(zombies[1], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[1].transform.rotation);
                    } else {
                        zombie = Instantiate(zombies[2], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[2].transform.rotation);
                    }
                    spawn.Add(zombie.GetComponent<Enemy>());
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

    void Die() {
        // Kill all the spawn for this spawner to give the player some breathing room
        foreach (Enemy z in spawn) {
            if (z != null) {
                z.KillImmediately();
            }
        }
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
        }
    }

    // DestroyAfterAnimation is a public function that is 
    // used as an animation Event to destroy the enemy
    public void DestroyAfterAnimation() {
        gameManager.RunScoreRoutine();
        Destroy(gameObject);
    }
}
