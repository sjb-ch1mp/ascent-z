using UnityEngine;

public class SpawnCar : MonoBehaviour
{

    public int damage = 100;
    public AudioClip engineSound;
    public AudioClip doorSound;
    public Transform spawnPoint;
    public Transform destination;

    float carSpeed = 10f;
    bool isSpawning = false;
    bool isDrivingBy = false;
    bool playerDroppedOff = false;
    Vector2 startPosition;
    Vector2 spawnPointRelativePosition;
    Vector2 destinationRelativePosition;
    AudioSource audioSource;
    Player player;

    void Awake() {
        startPosition = transform.position;
        spawnPointRelativePosition = spawnPoint.transform.position;
        destinationRelativePosition = destination.transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (isSpawning || isDrivingBy) {
            transform.position = Vector2.MoveTowards(transform.position, destination.position, carSpeed * Time.deltaTime);
            if (!playerDroppedOff && transform.position.x >= spawnPointRelativePosition.x) {
                if (isSpawning) {
                    playerDroppedOff = true;
                    player.ShowPlayer(true);
                    player = null;
                    audioSource.PlayOneShot(doorSound);
                }
            } else if (transform.position.x >= destinationRelativePosition.x) {
                Reset();
            }
        } 
    }

    public void DoSpawnCar(Player newPlayer) {
        Reset();
        player = newPlayer;
        player.ShowPlayer(false);
        isSpawning = true;
        playerDroppedOff = false;
        audioSource.Play();
    }

    public void DoDriveBy() {
        Reset();
        isDrivingBy = true;
        audioSource.Play();
    }

    void Reset() {
        transform.position = startPosition;
        isSpawning = false;
        isDrivingBy = false;
        audioSource.Stop();
    }

}
