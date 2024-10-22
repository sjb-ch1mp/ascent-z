using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestedWindow : MonoBehaviour
{
    // Exports
    [SerializeField] private float spawnDelay;
    [SerializeField] private float numEnemies;
    [SerializeField] private Sprite trigger;
    [SerializeField] private Components comps;

    // References
    private GameManager gameManager;

    // State
    private State state = State.IDLE;
    private int id;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        id = gameManager.GetNewZombieSpawnerId();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!(state == State.TRIGGERED) && collider.gameObject.CompareTag("Player"))
        {
            state = State.TRIGGERED;
            comps.bits.gameObject.SetActive(true);
            comps.bits.Split();
            comps.sprite.sprite = trigger;
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numEnemies; i++)
        {
            yield return new WaitForSeconds(spawnDelay);
            Instantiate(comps.zombie, transform.position, comps.zombieContainer.transform.rotation, comps.zombieContainer.transform);
        }
    }

    enum State
    {
        IDLE,
        TRIGGERED,
    }

    [System.Serializable]
    struct Components
    {
        public SpriteRenderer sprite;
        public BitsContainer bits;
        public GameObject zombie;
        public GameObject zombieContainer;
    }
}
