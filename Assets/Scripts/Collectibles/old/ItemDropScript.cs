using UnityEngine;

public class ItemDropScript : MonoBehaviour
{
    public bool collected = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("GameBoundary"))
        {
            collected = true;
            Destroy(gameObject);
        }
    }
}
