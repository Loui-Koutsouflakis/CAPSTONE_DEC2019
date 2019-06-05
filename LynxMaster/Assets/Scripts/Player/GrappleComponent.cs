using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//updated 05/28- LF

public class GrappleComponent : MonoBehaviour
{

    [Range(45, 90)]

    private Rigidbody body;
    public Transform tetherPoint;
    public Transform attachedTetherPoint; 
    public bool tether;
    public float maxSwingSpeed;
    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;
    public float launchSpeed;
    public float boost = 20;
    private bool reachedZero = false;


    public bool isStaring;

    [Range(20, 90)]
    public float maxAngle;

    private GrappleComponent grapple;
    private PlayerMovementv2 normalMove;

    // Start is called before the first frame update

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        tether = false;

        normalMove = GetComponent<PlayerMovementv2>();
        grapple = GetComponent<GrappleComponent>();

        singleton = this;
    }



    // Update is called once per frame
    void Update()
    {
        //if there is a tether point
        if (tetherPoint != null)
        {
            if (Input.GetButtonDown("LeftBumper"))//t for testing
            {

                tether = true;
                attachedTetherPoint = tetherPoint;

            }

        }


        if (tether)
        {
           
            body.mass = 5;
            Vector3 radial = (attachedTetherPoint.position - transform.position).normalized;
            Vector3 radialVelocityComponent = Vector3.Dot(body.velocity, radial) * radial;
            body.velocity -= radialVelocityComponent;
            body.velocity = body.velocity.normalized * Mathf.Min(body.velocity.magnitude, maxSwingSpeed);

            Vector3 tetherDirection = attachedTetherPoint.position - transform.position;
            float tetherLength = tetherDirection.magnitude;
            float x = Vector3.Dot(tetherDirection.normalized, Vector3.up);
            float y = (tetherDirection.normalized - x * Vector3.up).magnitude;
            float theta = Mathf.Atan2(y, x);
            float tension = body.mass * 9.8f * Mathf.Cos(theta) + body.mass * Mathf.Pow(body.velocity.magnitude, 2) / tetherLength;
            body.AddForce(tension * tetherDirection.normalized);
            Debug.DrawLine(transform.position, attachedTetherPoint.position);
            float thetaDegrees = theta * Mathf.Rad2Deg;
            //Debug.Log(thetaDegrees);


            if (thetaDegrees < 5)
            {
                Debug.Log("Reached Zero");
                reachedZero = true;
            }

            if (reachedZero)
            {
                body.AddForce(body.velocity.normalized * boost, ForceMode.Impulse);

                if (thetaDegrees > maxAngle)
                {
                    body.mass = 1;
                    body.AddForce(body.velocity.normalized * launchSpeed, ForceMode.Impulse);
                    normalMove.enabled = true;
                    tether = false;
                    attachedTetherPoint = null;
                    reachedZero = false;
                }

            }



        }



        if (Input.GetButtonUp("LeftBumper"))
        {
            body.mass = 1;
            body.AddForce(body.velocity.normalized * launchSpeed, ForceMode.Impulse);
            normalMove.enabled = true;
            tether = false;

            if (tetherPoint == attachedTetherPoint && isStaring == false)
                tetherPoint = null;

            attachedTetherPoint = null; 

            reachedZero = false;

        }
    }

    public Transform setTetherPoint(Transform t)
    {
        if (t != null)
        {
            tetherPoint = t;
            return tetherPoint;
        }


        Debug.Log("No tetherpoint");
        tetherPoint = null; 
        return tetherPoint;

    }




    private void OnDisable()
    {
        tether = false;
        body.mass = 1;
    }



    public static GrappleComponent singleton;

}


