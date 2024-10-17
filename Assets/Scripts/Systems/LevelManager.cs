using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Checkpoint[] checkpoints;

    private int currentCheckpoint = 0;

    public static LevelManager Instance { get; private set; }

    // Destructive Singleton, i.e. will refresh upon load
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            int i = 0;
            foreach (Checkpoint check in checkpoints)
            {
                check.Assign(this, i);
                i++;
            }

            // Trigger the initial checkpoint
            checkpoints[0].Trigger();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void SetCheckpoint(int checkpoint)
    {
        // Silently trigger all checkpoints until the current
        for (int i = 0; i < checkpoint; i++)
        {
            checkpoints[i].SilentTrigger();
        }
        currentCheckpoint = checkpoint;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 GetSpawn()
    {
        return checkpoints[currentCheckpoint].GetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
