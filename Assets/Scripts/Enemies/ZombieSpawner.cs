using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{

    // Exports
    public GameObject[] zombies;
    public float[] zombieSpawnThresholds;
    public float spawnRate = 1.0f;
    public float aggroRange = 50.0f;
    public float health = 2000f;

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
            yield return new WaitForSeconds(isActive ? spawnRate : spawnRate / 2);
            if (gameManager.IsPaused() && gameManager.IsGameOver()) {
                continue;
            } else if (isActive) {
                animator.SetTrigger("spawn-zombie");
                float randomSpawnX = Random.Range(spawnBoundLeft.position.x, spawnBoundRight.position.x);
                float randomZombieThreshold = Random.value;
                if (randomZombieThreshold <= zombieSpawnThresholds[0]) {
                    Instantiate(zombies[0], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[0].transform.rotation);
                } else if (randomZombieThreshold <= zombieSpawnThresholds[1]) {
                    Instantiate(zombies[1], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[1].transform.rotation);
                } else {
                    Instantiate(zombies[2], new Vector3(randomSpawnX, transform.position.y, transform.position.z), zombies[2].transform.rotation);
                }
            } else {
                // If player isn't in range - check for the player
                isActive = PlayerNearby();
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
        Destroy(gameObject);
    }
}
