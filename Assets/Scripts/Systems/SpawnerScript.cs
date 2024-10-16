using System.Collections;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject weaponCase; // Assign this in the inspector
    public GameObject collectable;
    public float spawnInterval = 30f; // Time interval between spawn attempts

    GameManager gameManager;
    GameObject collectibleContainer;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        collectibleContainer = GameObject.Find("Collectibles");
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        if (!gameManager.IsGameOver())
        {
            while (true) // Loop indefinitely
            {
                while (gameManager.IsPaused())
                {
                    yield return new WaitForSeconds(Resources.SPIN_TIME);
                }

                yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval

                if (Random.value < 0.33f) // 33% chance to spawn something
                {
                    if (Random.value < 0.5f) // 50% chance between weapon case and collectable
                    {
                        Instantiate(weaponCase, transform.position, Quaternion.identity, collectibleContainer.transform);
                    }
                    else
                    {
                        Instantiate(collectable, transform.position, Quaternion.identity, collectibleContainer.transform);
                    }
                }
            }
        }
    }
}
