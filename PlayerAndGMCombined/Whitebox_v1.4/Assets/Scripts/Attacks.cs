// Created by SChiraz 19/02/2019
// Modified by SChiraz 25/02/2019 -> Dash + DashAttack
// Modified by SChiraz 18/03/2019 -> Preparing to implement attacks 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    //public Animator PAnim;
    public float DashSpeed = 10.0f;
    //public CapsuleCollider RHand;
    //public CapsuleCollider LHand;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("q"))
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * DashSpeed, ForceMode.Impulse);
        }

        //if(PAnim.GetCurrentAnimatorStateInfo(0).IsTag("Dash"))
        //{
        //    if (Input.GetKeyDown("e"))
        //    {
        //        Debug.Log("Dash Hit");
        //    }
        //}

        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit, 2)) // Add an Angle Radius ? - Checks if in Range
        //{
        //    if (hit.collider.gameObject.tag == "Enemy")
        //    {
        //        if (Input.GetKeyDown("e"))
        //        {
        //            Debug.Log("Hit");
        //        }
        //    }
        //}

        //if (PAnim.GetCurrentAnimatorStateInfo(0).IsTag("Swipe"))
        //{
        //    RHand.enabled = true;
        //    LHand.enabled = true;
        //}
        //else
        //{
        //    RHand.enabled = false;
        //    LHand.enabled = false;
        //}
    }
}
