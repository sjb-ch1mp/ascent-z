using System.Collections;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject objectToSpawn; // Assign this in the inspector
    public float spawnInterval = 30f; // Time interval between spawn attempts

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        if (!gameManager.IsGameOver()) {

            while (true) // Loop indefinitely
            {

                while (gameManager.IsPaused()) {
                    yield return new WaitForSeconds(Resources.SPIN_TIME);
                }

                yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval

                if (Random.value < 0.33f) // 33% chance
                {
                    Instantiate(objectToSpawn, transform.position, Quaternion.identity);
                }
            }    

        }
    }
}
