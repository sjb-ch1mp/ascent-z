using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    private SpriteRenderer healthBarRenderer;
    public float health = 100f;


    private bool isInLava = false;
    private Coroutine lavaDamageCoroutine;


    public GameManager gameManager;
    private bool isDead = false;



    [SerializeField] private int lives = 5;
    public int Lives
    {
        get { return lives; }
    }





    // Start is called before the first frame update
    void Start()
    {
        // Find the HealthBar sprite by traversing the hierarchy
        healthBarRenderer = transform.Find("HealthBar").GetComponent<SpriteRenderer>();


        StartCoroutine(RegenerateHealth());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ReduceHealth(15f);

        }

        if (collision.gameObject.CompareTag("Lava"))
        {
            // Lose initial health
            ReduceHealth(25f);

            // Start the damage over time coroutine if not already running
            isInLava = true;
        }


        if (health <= 0f || collision.gameObject.CompareTag("GameBoundary"))
        {
            // Reset player position to (0, 0) when colliding with a GameBoundary
            transform.position = new Vector3(0, 10, transform.position.z);

            health = 100f;

            // Calculate the new width based on the remaining health
            float healthPercentage = health / 100f;

            // Adjust the local scale of the health bar along the x-axis
            Vector3 healthBarScale = healthBarRenderer.transform.localScale;
            healthBarScale.x = healthPercentage;
            healthBarRenderer.transform.localScale = healthBarScale;

            lives -= 1;

            if (lives <= 0 && isDead == false)
            {
                isDead = true;
                gameManager.gameOver();
            }
        }



    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            isInLava = false;
        }
    }




    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second

            // Regenerate 5 health per second
            if (health < 100f)
            {
                health += 5f;

                if (isInLava)
                {
                    health -= 15;
                }


                health = Mathf.Clamp(health, 0, 100); // Ensure health doesn't exceed 100

                // Update health bar
                UpdateHealthBar();
            }
        }
    }

    private void UpdateHealthBar()
    {
        // Calculate the new width based on the remaining health
        float healthPercentage = health / 100f;

        // Adjust the local scale of the health bar along the x-axis
        Vector3 healthBarScale = healthBarRenderer.transform.localScale;
        healthBarScale.x = healthPercentage;
        healthBarRenderer.transform.localScale = healthBarScale;
    }



    private void ReduceHealth(float damage)
    {
        health -= damage;

        health = Mathf.Clamp(health, 0, 100);

        // Calculate the new width based on the remaining health
        float healthPercentage = health / 100f;

        // Adjust the local scale of the health bar along the x-axis
        Vector3 healthBarScale = healthBarRenderer.transform.localScale;
        healthBarScale.x = healthPercentage;
        healthBarRenderer.transform.localScale = healthBarScale;
    }




}
