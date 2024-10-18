using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private OneWayPlatform currentOneWayPlatform;

    [SerializeField] private Collider2D playerCollider;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject.GetComponent<OneWayPlatform>();
        }
    }

    private void OnCollisionStay2D(Collision2D collison)
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (currentOneWayPlatform != null)
            {
                //currentOneWayPlatform.Enter(playerCollider);
            }
        }
    }
}
