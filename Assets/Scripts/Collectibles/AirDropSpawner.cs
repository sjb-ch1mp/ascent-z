using System.Collections;
using UnityEngine;

public class AirDropSpawner : MonoBehaviour
{

    public GameObject weaponAirDropPrefab;
    public GameObject utilityAirDropPrefab;
    public Transform activeDrops;
    
    float airDropFrequency = 20;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameManager.Instance;
        StartCoroutine(RandomStartTime());
    }

    IEnumerator RandomStartTime() {
        yield return new WaitForSeconds(Random.Range(airDropFrequency / 2, airDropFrequency));
        StartCoroutine(SpawnAirDrop());
    }

    IEnumerator SpawnAirDrop() {
        while (!gameManager.IsGameOver()) {
            if (gameManager.IsPaused()) {
                yield return new WaitForSeconds(airDropFrequency / 2);
            } else {
                if (activeDrops.GetComponentsInChildren<CollectibleItem>().Length == 0) {
                    // Only spawn if there are no active drops (weapons more likely)
                    if (Random.value <= 0.75f) {
                        // Weapons cache
                        Instantiate(weaponAirDropPrefab, transform.position, transform.rotation, activeDrops);
                    } else {
                        // Item cache
                        Instantiate(utilityAirDropPrefab, transform.position, transform.rotation, activeDrops);
                    }
                }
                yield return new WaitForSeconds(airDropFrequency);
            } 
        }
    }
}
