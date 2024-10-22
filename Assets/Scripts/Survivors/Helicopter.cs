using System.Collections;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    // Exports
    public GameObject missilePrefab;
    public float dropFrequency = 0.75f;
    public float departureDelay = 2f;
    public float moveSpeed = 5f;
    public float departureElevation = 4f;

    // State
    bool departingRight;
    float destinationX;
    float destinationY;
    bool departing;

    // References
    SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        destinationY = transform.position.y + departureElevation;
        if (transform.position.x >= 1) {
            spriteRenderer.flipX = true;
            destinationX = GameObject.Find("WorldBorderLeft").transform.position.x;
            departingRight = false;
        } else {
            destinationX = GameObject.Find("WorldBorderRight").transform.position.x;
            departingRight = true;
        }
        StartCoroutine(FireMissiles());    
    }

    // Update is called once per frame
    void Update() {
        if (departing) {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(destinationX, destinationY), Time.deltaTime * moveSpeed);
            if ((departingRight && transform.position.x >= destinationX) || (!departingRight && transform.position.x <= destinationX)) {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator FireMissiles() {
        yield return new WaitForSeconds(departureDelay);
        departing = true;
        while (true) {
            yield return new WaitForSeconds(dropFrequency);
            Instantiate(missilePrefab, transform.position, Quaternion.identity);
        }
    }


}
