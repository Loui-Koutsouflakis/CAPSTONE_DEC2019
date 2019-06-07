﻿//Written By Michael Elkin 05/06/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce_Back : MonoBehaviour
{
    //[SerializeField]
    //PlayerMovementv2 playerRef;    

    [SerializeField]
    Vector3 lastGrounded;
    [SerializeField]
    Vector3 aimPoint;
    [SerializeField]
    Vector3 forceVector;
    [SerializeField]
    Vector3 rangePos;

    [SerializeField, Range(1, 100)]
    float positionOffSet = 100.0f;
    float rangeOffSet;

    //float gravity = 9.8f;
    //float LaunchTheta;
    //float maxHeight;
    [SerializeField, Range(10, 1000)]
    float velocityModifier = 10.0f;

    [SerializeField]
    bool needNewLastGrounded = false;

    float maxDistance = 100.0f; // Used To Store Max Distance for Raycasts
    RaycastHit hit; // Used to store Raycast Hit info
    bool grounded;

    private void Awake()
    {
        //CreateAimPoint();
        //playerRef = GetComponent<PlayerMovementv2>();        
        lastGrounded = new GameObject().transform.position;        
    }
    void Update()
    {
        IsGrounded();
        if(grounded == false && needNewLastGrounded)
        {
            SetMaxHeightPos();            
            needNewLastGrounded = false;
        }
    }


    private void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Ground")
        {
            
            needNewLastGrounded = true;
        }
    }    
    public void Rebound(GameObject temp)
    {
        forceVector = (aimPoint - transform.position).normalized * velocityModifier;
        temp.GetComponent<PlayerMovementv2>().GetComponent<Rigidbody>().velocity = Vector3.zero;
        temp.GetComponent<PlayerMovementv2>().GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }

    void SetMaxHeightPos()
    {
        lastGrounded = transform.position - (Vector3.forward * 10);
        aimPoint = lastGrounded + new Vector3(0, positionOffSet, 0); 
    }

    void IsGrounded()
    {
        Ray groundedRay = new Ray(transform.position, Vector3.down);
        Physics.Raycast(groundedRay, out hit, maxDistance);
        if (hit.distance <= 1.125f)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }



    //void SetAimPoint()
    //{
    //    aimPoint = lastGrounded + new Vector3(0, 10, 0);
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(forceVector, 1.0f);

    }



}