// Created by SChiraz 19/02/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    public float DashSpeed = 30.0f;
    public float JmpForce = 100000.0f;
    public Transform Target;
    public Transform PlayerFace;
    float Angle = 45.0f;

    float ChargeCap = 0.0f;

    bool IsGrounded = false;

    bool IsCharging = false;

    // Use this for initialization
    void Start () {

        //if(!PlayerFace)
        //{
        //    PlayerFace = null;
        //}
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("1")) // Light Attack
        {
            if (InRange()) { Debug.Log("You inflicted 20 Damage !"); }
        }

        if (Input.GetKeyDown("2")) // Strong Attack
        {
            IsCharging = true;
        }
        if (Input.GetKeyUp("2")) // Strong Attack
        {
            //if (InRange()) { Debug.Log("You Inflicted "+ ChargeCap * 20 + " Damage!"); }
            //else { Debug.Log("You Missed !"); }
            //ChargeCap = 0.0f;
            IsCharging = false;
        }

        if (Input.GetKeyDown("3")) // Dash (on forward) - will adjust once the Character controller is finalized
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * DashSpeed, ForceMode.Impulse);

        }

        if (!IsGrounded)
        {
            if (Input.GetKeyDown("4")) // Ground Pound
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.up * -JmpForce, ForceMode.Impulse);
                Debug.Log("Ground Pound !");
            }
        }

        if(IsCharging)
        {
            Charge();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
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

    void Charge()
    {
        ChargeCap += Time.deltaTime;
        if (ChargeCap >= 3.0)
        {
            if (InRange()) { Debug.Log("You Inflicted 60 Damage !"); }
            else { Debug.Log("You Missed !"); }
            ChargeCap = 0.0f;
            IsCharging = false;
        }
    }

    bool InRange()
    {
        float Distance = Vector3.Distance(Target.position, transform.position);
        return ((Vector3.Angle(PlayerFace.forward, Target.position - transform.position) < Angle) && (Distance <= 2));
    }
}
