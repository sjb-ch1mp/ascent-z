using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject objectToSpawn; // Assign this in the inspector
    public float spawnInterval = 30f; // Time interval between spawn attempts

    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        while (true) // Loop indefinitely
        {
            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval

            if (Random.value < 0.33f) // 33% chance
            {
                Instantiate(objectToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
}
