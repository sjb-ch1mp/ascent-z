using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private SpriteRenderer healthBarRenderer;
    public float health = 100f;
    public float bulletDamage = 25f;


    // Start is called before the first frame update
    void Start()
    {
        // Find the HealthBar sprite by traversing the hierarchy
        healthBarRenderer = transform.Find("HealthBar").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            ProjectileBehaviour projectile = collision.gameObject.GetComponent<ProjectileBehaviour>();

            ReduceHealth(projectile.damage);

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
        // Check if the collision is with an Explosion
        if (collision.gameObject.CompareTag("Explosion"))
        {
            // Get the ProjectileBehaviour component from the explosion prefab
            ExplosionBehaviour explosion = collision.gameObject.GetComponent<ExplosionBehaviour>();

            // Reduce health based on explosion damage
            ReduceHealth(explosion.damage);

            if (health <= 0)
            {
                Destroy(gameObject); // Destroy this object if health reaches zero
            }
        }
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
