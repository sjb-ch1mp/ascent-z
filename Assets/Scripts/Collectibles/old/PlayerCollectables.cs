using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectables : MonoBehaviour
{
    private GameManager gameManager;

    private PlayerHealth playerHealth;



    private void Start()
    {
        gameManager = GameManager.Instance;
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
   

        if (collision.gameObject.CompareTag("Collectible"))
        {

            // Armour, Medpack, Lives, Grenades
            Resources.Collectible randCollectible = (Resources.Collectible)Random.Range((int)Resources.Collectible.ARMOUR, (int)Resources.Collectible.GRENADES + 1);
            gameManager.PickUpCollectible(randCollectible);

            // Handle collectibles
            /*PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();*/
            switch (randCollectible)
            {
                case Resources.Collectible.ARMOUR:
                    playerHealth.armour = Mathf.Clamp(playerHealth.armour + Resources.GetAmountForCollectible(randCollectible), 0, Resources.MAX_ARMOUR);
                    break;
                case Resources.Collectible.LIFE:
                    playerHealth.AddLife();
                    break;
                case Resources.Collectible.MEDPACK:
                    playerHealth.health = Mathf.Clamp(playerHealth.health + Resources.GetAmountForCollectible(randCollectible), 0, Resources.MAX_HEALTH);
                    break;
            }
            Destroy(collision.gameObject);
        }





    }
}