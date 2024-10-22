using UnityEngine;

public class MissileExplosion : MonoBehaviour
{
    public GameObject explosionSoundPrefab;

    public int damage = 250;

    Rigidbody2D explosionRigidBody;

    void Start() {
        explosionRigidBody = GetComponent<Rigidbody2D>();
        Instantiate(explosionSoundPrefab, transform.position, Quaternion.identity);
    }

    void Update() {
        explosionRigidBody.AddTorque(0.1f * Time.deltaTime); // rotate the explosion to force collisions
    }
}
