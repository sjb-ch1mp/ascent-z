using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitsContainer : MonoBehaviour
{
    enum State
    {
        IDLE,
        DROPPED
    }

    [SerializeField] private float force;
    [SerializeField] private float maxTorque;
    [SerializeField] private float despawnTime;
    private Rigidbody2D[] rigidBodies;
    private State state = State.IDLE;

    void Awake()
    {
        rigidBodies = new Rigidbody2D[transform.childCount];

        int i = 0;
        foreach (Transform child in transform)
        {
            rigidBodies[i++] = child.GetComponent<Rigidbody2D>();
        }
    }

    public void Split()
    {
        if (state == State.IDLE)
        {
            foreach (Rigidbody2D rigidBody in rigidBodies)
            {
                Vector3 dir = Random.insideUnitCircle;
                rigidBody.AddForce(dir * force, ForceMode2D.Impulse);
                rigidBody.AddTorque(Random.Range(-maxTorque, maxTorque));
                StartCoroutine(Despawn());
            }
            state = State.DROPPED;
        }
    }

    // Overload split to provide an impulse as well as a callback
    // @overload
    public void Split(Vector3 impulse, System.Action complete)
    {
        if (state == State.IDLE)
        {
            foreach (Rigidbody2D rigidBody in rigidBodies)
            {
                Vector3 dir = Random.insideUnitCircle;
                rigidBody.AddForce(impulse + dir * force, ForceMode2D.Impulse);
                rigidBody.AddTorque(Random.Range(-maxTorque, maxTorque));
                StartCoroutine(Despawn(complete));
            }
            state = State.DROPPED;
        }
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);
        gameObject.SetActive(false);
    }

    // Overload the despawn to handle a callback at the end of the despawn 
    // @overload
    IEnumerator Despawn(System.Action complete)
    {
        yield return new WaitForSeconds(despawnTime);
        gameObject.SetActive(false);
        complete.Invoke();
    }
}
