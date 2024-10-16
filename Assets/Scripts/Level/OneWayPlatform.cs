using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private static Collider2D playerCollider;
    public Collider2D platformCollider;
    private bool is_off;
    
    public void Enter(Collider2D player)
    {
        if (playerCollider == null)
        {
            playerCollider = player;
        }

        is_off = true;
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (is_off)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                is_off = false;
                if (playerCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
                }
            }
        }
    }
}
