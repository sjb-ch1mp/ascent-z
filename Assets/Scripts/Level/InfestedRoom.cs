using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestedRoom : MonoBehaviour
{
    // Exports
    [SerializeField] private float spawnDelay;
    [SerializeField] private float numEnemies;
    [SerializeField] private GameObject zombie;
    [SerializeField] private GameObject zombieContainer;

    // References
    private GameManager gameManager;

    // State
    private bool triggered = false;
    private int id;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        id = gameManager.GetNewZombieSpawnerId();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!triggered && collider.gameObject.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numEnemies; i++)
        {
            yield return new WaitForSeconds(spawnDelay);
            zombie = Instantiate(zombie, transform.position, Quaternion.identity, zombieContainer.transform);
        }
    }
}
