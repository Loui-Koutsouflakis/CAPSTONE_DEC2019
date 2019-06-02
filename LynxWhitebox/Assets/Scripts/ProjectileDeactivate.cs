using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class ProjectileDeactivate : MonoBehaviour
{    

    void OnEnable()
    {
        Invoke("Deactivate", 1f);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            // Damage or Kill Player function
            gameObject.SetActive(false);
        }
    }
}
