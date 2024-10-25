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
    [SerializeField] private float despawnVariance;
    private Rigidbody2D[] rigidBodies;
    private State state = State.IDLE;
    private int numBits = 0;

    void Awake()
    {
        numBits = transform.childCount;
        rigidBodies = new Rigidbody2D[numBits];

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
                float despawn = despawnTime + Random.Range(-despawnVariance, despawnVariance);
                StartCoroutine(Despawn(rigidBody.gameObject, despawn));
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
                float despawn = despawnTime + Random.Range(-despawnVariance, despawnVariance);
                StartCoroutine(Despawn(rigidBody.gameObject, despawn, complete));
            }
            state = State.DROPPED;
        }
    }

    // Overload split to provide an impulse as well as a callback
    // @overload
    public void Split(System.Action complete)
    {
        if (state == State.IDLE)
        {
            foreach (Rigidbody2D rigidBody in rigidBodies)
            {
                Vector3 dir = Random.insideUnitCircle;
                rigidBody.AddForce(dir * force, ForceMode2D.Impulse);
                rigidBody.AddTorque(Random.Range(-maxTorque, maxTorque));
                float despawn = despawnTime + Random.Range(-despawnVariance, despawnVariance);
                StartCoroutine(Despawn(rigidBody.gameObject, despawn, complete));
            }
            state = State.DROPPED;
        }
    }

    IEnumerator Despawn(GameObject bit, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bit != null)
        {
            bit.SetActive(false);
        }

        numBits -= 1;
        if (numBits == 0)
        {
            gameObject.SetActive(false);
        }
    }

    // Overload the despawn to handle a callback at the end of the despawn 
    // @overload
    IEnumerator Despawn(GameObject bit, float delay, System.Action complete)
    {
        yield return new WaitForSeconds(delay);

        if (bit != null)
        {
            bit.SetActive(false);
        }

        numBits -= 1;
        if (numBits == 0)
        {
            complete.Invoke();
            gameObject.SetActive(false);
        }
    }
}
