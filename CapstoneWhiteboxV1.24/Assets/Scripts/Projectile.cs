using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject, 3.0f);
        }

        if (collision.gameObject.tag == "Player")
        {
            // Damage or Kill Player function
            Destroy(gameObject);
        }
    }
}
