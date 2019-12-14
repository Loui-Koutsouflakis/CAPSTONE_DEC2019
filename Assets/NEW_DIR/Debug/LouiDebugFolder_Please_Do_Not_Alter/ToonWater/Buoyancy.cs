using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 currentForce = new Vector3(0.12f, 0f, 0.12f);
    public bool followCurrent;

    private bool floating;
    private float surfaceY;
    Vector3 floatForce = new Vector3(0f, 12f, 0f);
    const string waterTag = "Water";
    readonly float decayLimit = 0.1f;
    readonly float impactDecay = 0.35f;
    readonly float velocityDecay = 0.966f;

    private void Start()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == waterTag)
        {
            surfaceY = other.gameObject.transform.position.y;
            rb.useGravity = false;
            rb.velocity *= impactDecay;
            floating = true;
        }
    }

    void FixedUpdate()
    {
        if(floating)
        {
            rb.AddForce(floatForce * (surfaceY - transform.position.y));

            if (transform.position.y < surfaceY && rb.velocity.magnitude > decayLimit)
            {
                rb.velocity *= velocityDecay;
            }

            if(followCurrent)
            {
                rb.AddForce(currentForce);
            }
        }
    }
}
