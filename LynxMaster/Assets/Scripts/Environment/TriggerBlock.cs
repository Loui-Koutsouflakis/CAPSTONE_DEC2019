//Written by Mike Elkin 07/10/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[AddComponentMenu("Mike's Scripts/TriggerBlock", 9)]

public class TriggerBlock : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;// Switch Rigidbody
    [SerializeField, Range(0,50)]
    float blockMass = 1.0f;// Setting For Mass of Rigidbody
    [SerializeField, Range(0, 1)]
    float deadZone = 0.5f;// Buffer Zone for Block Movement
    [SerializeField]
    MultiPurposePlatform triggerActivated;//Platform to Trigger
    [SerializeField]
    Vector3 lockedPosition;// Home Position Once Triggered 
    [SerializeField]
    Vector3 currentPosition;// Position in Real Time
    [SerializeField]
    Vector3 startPosition;// Position on Awake
    [SerializeField]
    Vector3 endPosition;// Trigger Point
    Vector3 downForce;// Downward Force Vector
    Vector3 upForce;// Upward Force Vector


    bool playerOnSwitch;

    void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.mass = blockMass;        
        startPosition = transform.position;
        endPosition = startPosition + Vector3.down * .65f;
        lockedPosition = endPosition + Vector3.down;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Istriggered();
        
        AddForce();
        CurrentPosition();

    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Player")
        {
            //if (transform.position.y <= endPosition.y && !playerOnSwitch)
            //{
            //    startPosition = endPosition;
            //    playerOnSwitch = true;
            //}
            //impactVel = c.gameObject.GetComponent<Rigidbody>().mass * Physics.gravity;
        }
        else
        {
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), c.collider);

        }
    }
    private void OnTriggerEnter(Collider o)
    {
        if(o.gameObject.tag == "Player")
        {
            playerOnSwitch = true;
        }
    }
    private void OnTriggerExit(Collider o)
    {
        if (o.gameObject.tag == "Player")
        {
            playerOnSwitch = false; ;
        }
    }

    void Istriggered()
    {
        if (transform.position.y <= endPosition.y && playerOnSwitch)
        {
            startPosition = lockedPosition;
            rb.drag = 5.0f;
            rb.mass = 50.0f;
            // Add Trigger For Asociated Platform
        }
    }

    void CurrentPosition()
    {
        currentPosition = transform.position;
    }
    

    void AddForce()
    {
        //CalcUpwardForce();
        if (transform.position.y > startPosition.y + deadZone)
        {
            rb.AddForce((rb.mass * Physics.gravity) * (startPosition - transform.position).magnitude * 0.5f);
        }
        if (transform.position.y < startPosition.y - deadZone)
        {
            rb.AddForce(-(rb.mass * Physics.gravity) * (startPosition - transform.position).magnitude * 0.5f);

        }
    }

}
