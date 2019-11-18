using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public Rigidbody[] rigidbodies;
    public Collider[] colliders;

    public void Cue(float force, float radius, Vector3 offset)
    {
        foreach(Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddExplosionForce(force, rb.gameObject.transform.position + offset, radius);
        }

        foreach(Collider col in colliders)
        {
            col.enabled = true;
        }
    }
}
