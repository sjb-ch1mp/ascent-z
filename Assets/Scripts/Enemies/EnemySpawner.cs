using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab1;  // Reference to the first enemy prefab
    public GameObject enemyPrefab2;  // Reference to the second enemy prefab
    public GameObject enemyPrefab3;  // Reference to the third enemy prefab
    public float spawnDelay = 2.0f;  // Delay between waves
    public float spawnRadius = 5.0f;  // Radius within which enemies can spawn around the spawner
    [SerializeField] private int currentWave = 1;  // Tracks the current wave number
    private int remainingPoints;  // Points left to allocate to enemy spawns

    GameManager gameManager;

    public int CurrentWave
    {
        get { return currentWave; }
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {

        while (gameManager.IsPaused()) {
            yield return new WaitForSeconds(Resources.SPIN_TIME);
        }

        while (true)
        {
            remainingPoints = currentWave;  // Each wave has points equal to the wave number
            List<GameObject> enemies = new List<GameObject>();

            while (remainingPoints > 0)
            {
                GameObject enemyToSpawn = null;
                int enemyCost = 0;

                // Generate a random value to determine which enemy to spawn
                float randomValue = Random.Range(0f, 1f);

                // Determine which enemy to spawn based on the remaining points and random chance
                if (remainingPoints >= 10 && randomValue < 0.5f)  // 10% chance to spawn the enemy worth 10 points
                {
                    enemyToSpawn = enemyPrefab3;  // Spawn the third enemy
                    enemyCost = 10;
                }
                else if (remainingPoints >= 5 && randomValue < 0.3f)  // 30% chance to spawn the enemy worth 5 points
                {
                    enemyToSpawn = enemyPrefab2;  // Spawn the second enemy
                    enemyCost = 5;
                }
                else if (remainingPoints >= 1)
                {
                    enemyToSpawn = enemyPrefab1;  // Spawn the first enemy
                    enemyCost = 1;
                }

                if (enemyToSpawn != null)
                {
                    Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
                    GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
                    enemies.Add(newEnemy);
                    remainingPoints -= enemyCost;  // Deduct points based on enemy cost
                }

                yield return null;
            }

            // Wait for all enemies to be destroyed before starting the next wave
            while (enemies.Count > 0)
            {
                enemies.RemoveAll(item => item == null);  // Remove null entries from the list
                yield return null;
            }

            yield return new WaitForSeconds(spawnDelay);  // Delay before the next wave
            currentWave++;  // Increase wave number
        }
    }
}