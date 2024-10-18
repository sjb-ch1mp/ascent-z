using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Checkpoint check = other.gameObject.GetComponent<Checkpoint>();
        if (check != null)
        {
            check.Trigger();
        }
    }
}
