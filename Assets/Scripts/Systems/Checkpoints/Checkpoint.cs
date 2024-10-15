using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Relative position to spawn from
    public Vector3 spawnPoint;

    protected LevelManager lm;
    protected int priority;
    protected bool triggered;

    public void Assign(LevelManager lm, int i)
    {
        this.lm = lm;
        priority = i;
    }

    public virtual void Trigger()
    {
        // Early return if triggered
        if (triggered)
        {
            return;
        }

        // Trigger
        triggered = true;

        TriggerState();

        lm.SetCheckpoint(priority);
    }

    public virtual void SilentTrigger()
    {
        // Early return if triggered
        if (triggered)
        {
            return;
        }

        TriggerState();
    }

    protected void TriggerState()
    {
        // Trigger
        triggered = true;
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position + spawnPoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
