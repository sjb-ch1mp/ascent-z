using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Exports
    public float countDownTimeMin = 0.2f;
    public float countDownTimeMax = 0.5f;
    public GameObject explosionPrefab;
    public int damage = 250;

    void Start() {
        StartCoroutine(DoCountDown());
    }

    IEnumerator DoCountDown() {
        yield return new WaitForSeconds(Random.Range(countDownTimeMin, countDownTimeMax));
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
