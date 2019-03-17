// Created by SChiraz 19/02/2019
// Modified by SChiraz 25/02/2019 -> Dash + DashAttack

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    public float DashSpeed = 100.0f;
    public float JmpForce = 100000.0f;
    public Transform Target;
    float Angle = 45.0f;

    float ChargeCap = 0.0f;

    bool IsGrounded = false;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        float Distance = Vector3.Distance(Target.position, transform.position);

        //if ((Vector3.Angle(Target.forward, transform.position - Target.position) < Angle) /*&& (Distance <= 2)*/) -> Need to adjust the angle radius + distance for range
        {
            if (Input.GetKeyDown("1")) // Light Attack
            {
                Debug.Log("You inflicted 20 Damage !");
            }
            else if (Input.GetKeyDown("2")) // Strong Attack
            {
                Debug.Log("Charging !");
                Charge();
            }
        }

        if (Input.GetKeyDown("q")) // Dash, Made but still requires work as it's inconsistent of the direction the body is moving. Would be easier if the entire model roated with the Cam and not just a part of it (i.e. the pivot point)
        {
            float horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

            if (horizontal != 0) { gameObject.GetComponent<Rigidbody>().AddForce(transform.position * DashSpeed * horizontal, ForceMode.Impulse); }
            if (vertical != 0) { gameObject.GetComponent<Rigidbody>().AddForce(transform.position * DashSpeed * vertical, ForceMode.Impulse); }
            if (vertical != 0 && horizontal != 0) { gameObject.GetComponent<Rigidbody>().AddForce(transform.position * DashSpeed * vertical * horizontal, ForceMode.Impulse); }

            float DashDur = 0.0f;

            while(DashDur < 10) // Have a function that actively checks which animation is playing so that the window if the dash attack will only be available during the Dash Animation
            {
                Debug.Log(DashDur); // Need Dash Animation to Complete
                if (Input.GetKeyDown("1"))
                {
                    Debug.Log("Attack from Dash !");
                }
                DashDur += Time.deltaTime;
            }
        }

        if (!IsGrounded)
        {
            if (Input.GetKeyDown("4")) // Ground Pound
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.up * -JmpForce, ForceMode.Impulse);
                Debug.Log("Ground Pound !");
            }
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
        //if (ChargeCap >= 3.0)
        //{
        //    Debug.Log("You did 60 Damage !");
        //    return;
        //}

        Debug.Log(ChargeCap);
    }
}
