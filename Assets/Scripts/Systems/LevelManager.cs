using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Checkpoint[] checkpoints;

    private int currentCheckpoint = 0;

    void Awake()
    {

        int i = 0;
        foreach (Checkpoint check in checkpoints)
        {
            check.Assign(this, i);
            i++;
        }

        // Trigger the initial checkpoint
        checkpoints[0].Trigger();
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
