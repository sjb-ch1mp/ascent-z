using System.Collections;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{

    // Export
    public float timeLimit = 10f;

    // References
    GameManager gameManager;

    // State
    bool weaponsCache = false;
    bool isCollected = false;

    void Start() {
        gameManager = GameManager.Instance;
        weaponsCache = GetComponent<UtilityCrate>() == null || GetComponent<WeaponsCache>() != null;
        StartCoroutine(DestroyOnTimeUp());
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player") && !isCollected) {
            isCollected = true;
            if (weaponsCache) {
                WeaponsCache weaponsCache = GetComponent<WeaponsCache>();
                gameManager.PickUpWeapon(weaponsCache.WeaponType);
            } else {
                UtilityCrate utilityCrate = GetComponent<UtilityCrate>();
                gameManager.PickUpCollectible(utilityCrate.CollectibleType);
            }
            Destroy(gameObject);
        } else if (other.gameObject.CompareTag("GameBoundary")) {
            // Destroy cache if it falls out of the boundary
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyOnTimeUp() {
        yield return new WaitForSeconds(timeLimit);
        Destroy(gameObject);
    }
}