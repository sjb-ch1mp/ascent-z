using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudScript : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public GameObject spawner;

    public GameObject pistol; // Reference to the square GameObject
    public GameObject machineGun;
    public GameObject sniper;

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;
    public GameObject heart4;
    public GameObject heart5;

    public TextMeshProUGUI waveText;




    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private EnemySpawner enemySpawner;

    void Start()
    {
        // Get the PlayerMovement component from the player GameObject
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerHealth = player.GetComponent<PlayerHealth>();
            enemySpawner = spawner.GetComponent<EnemySpawner>();
        }
    }

    void Update()
    {
        if (playerMovement != null)
        {
            // Check the currentWeapon and hide the square if it is 0
            pistol.SetActive(playerMovement.CurrentWeapon == 0);

            machineGun.SetActive(playerMovement.CurrentWeapon == 1);

            sniper.SetActive(playerMovement.CurrentWeapon == 2);

        }


        if (playerHealth != null)
        {

            heart1.SetActive(playerHealth.Lives >= 1);
            heart2.SetActive(playerHealth.Lives >= 2);
            heart3.SetActive(playerHealth.Lives >= 3);
            heart4.SetActive(playerHealth.Lives >= 4);
            heart5.SetActive(playerHealth.Lives >= 5);
        }




        // Update the weaponText with the current weapon number
        waveText.text = "Wave: " + enemySpawner.CurrentWave.ToString();



    }
}