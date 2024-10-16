using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCheckpoint : Checkpoint
{
    public Sprite triggerSprite;

    // Loudly changes state with audio cue
    public override void Trigger()
    {
        if (triggered)
        {
            return;
        }

        TriggerState();

        lm.SetCheckpoint(priority);

        // Change visual
        GetComponent<SpriteRenderer>().sprite = triggerSprite;
        // Disable collider
        GetComponent<Collider2D>().enabled = false;
        // Play sound
    }

    // Silently changes state and visual
    public override void SilentTrigger()
    {
        if (triggered)
        {
            return;
        }

        TriggerState();
        // Change visual
        GetComponent<SpriteRenderer>().sprite = triggerSprite;
        // Disable collider
        GetComponent<Collider2D>().enabled = false;
    }
}
