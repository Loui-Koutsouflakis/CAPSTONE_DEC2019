using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    // Loui create a tag with your desired name for the enemies you wish to pound.
    // Simply change the name on line 26 and add the enemies to the tag and all "should" work.

    // Note: must add desired enemies to GroundedEnemy tag in order for the GroundPound to work!

    public float DropForce; // Force of the downward force.
    public float KnockBackForce; // Knock back force.
    public float GroundPoundRadius; // ground pound radius of attack.

    public bool isPounding;

    public GameObject Player; // player object.
    public GameObject[] Enemies; // (Enemy must have a RigidBody!!)
    
    bool IsGrounded = false; // bool for ground.


    void Start()
    {
        if (Enemies == null)
        {
            Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
    }


    void Update()
    {

        if (!IsGrounded)
        {
            if (Input.GetButtonDown("Dive")) // Ground Pound
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.up * -DropForce, ForceMode.Impulse); // Force Down
                Debug.Log("Ground Pound !");
                isPounding = true;

                //foreach(GameObject Enemy in Enemies)
                //{
                //    if ((Vector3.Distance(Enemy.transform.position, Player.transform.position) < GroundPoundRadius)) // Radius check
                //    {
                //        Debug.Log("Hit!");

                //        Enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * KnockBackForce); // Bounces enemies.
                //    }
                //}
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (isPounding)
            {
                foreach (GameObject Enemy in Enemies)
                {
                    if ((Vector3.Distance(Enemy.transform.position, Player.transform.position) < GroundPoundRadius)) // Radius check
                    {
                        Debug.Log("Hit!");

                        Enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * KnockBackForce); // Bounces enemies.
                    }
                }
            }

            IsGrounded = true;
            isPounding = false;
            
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
