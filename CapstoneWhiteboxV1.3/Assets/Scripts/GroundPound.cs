using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : MonoBehaviour {

    public float JmpForce; // 100000.0f
    public float nbForce; // Knock back force.

    public GameObject playerObj; // player object.
    public GameObject enemy1; // (Enemy must have a RigidBody!!) will turn this into a list next time so as to add more enemies easier.

    bool IsGrounded = false; // bool for ground.

    public float gpRadius; // ground pound radius of attack.

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!IsGrounded)
        {
            if (Input.GetKeyDown("4")) // Ground Pound
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.up * -JmpForce, ForceMode.Impulse);
                Debug.Log("Ground Pound !");

                if ((playerObj.transform.position).magnitude < gpRadius) // if enemy is in radius of gp, it will get knocked back.
                {
                    enemy1.GetComponent<Rigidbody>().AddForce(Vector3.up * nbForce); // makes enemy bounce up when ground pounded.
                }
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = false;
        }
    }
}
