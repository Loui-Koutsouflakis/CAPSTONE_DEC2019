using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//updated 05/28- LF
//updated 08/06
[AddComponentMenu("Player Scripts/Grapple Movement", 4)]

public class GrappleComponent : PlayerVariables
{    
    public Transform attachedTetherPoint;
    public Transform tetherPoint;
    public bool tether = false;

    public bool isStaring;

    //[Range(20, 90)]
    //private float maxAngle = 60;


    
    //for tether location in relation to player
    private Vector3 tetherDirection;
    private float tetherLength;
    private float angleToTether;

    //for animations
    private float swingDirection;

    //attaches to the available tetherpoint
    public void Grapple()
    {
        tetherPoint = player.tetherPoint.GetTetherLocation();
        
        if (tetherPoint == null)
        {
            Debug.Log("No tetherpoint");
            return;
        }

        tether = true;
        attachedTetherPoint = tetherPoint;
        player.attachedGrapplePoint = attachedTetherPoint; 
        //new stuff
        tetherDirection = attachedTetherPoint.transform.position - transform.position;
        tetherLength = tetherDirection.magnitude;
        angleToTether = Vector3.Angle(tetherDirection.normalized, attachedTetherPoint.transform.up);

        player.rb.velocity = Vector3.zero;
        Vector3 initForceDir = Vector3.Cross(player.transform.right, tetherDirection);
        rb.AddForce(initForceDir.normalized * 8, ForceMode.Impulse);

    }

    private void OnDisable()
    {
        DetatchGrapple();
    }

    // Update is called once per frame
    void Update()
    {
        //to see if character runs into wall
        CheckCollisions();
    }

    private void FixedUpdate()
    {
        if (!tether)
            return;
        
        //updates velocity
        Vector3 deltaVelocity = rb.mass * Physics.gravity * Mathf.Sin(angleToTether * Mathf.Deg2Rad) * Time.fixedDeltaTime;
        rb.velocity -= Vector3.Dot(rb.velocity, deltaVelocity) * deltaVelocity;

        //calculates and adds tension force
        float tension = rb.mass * 9.8f * Mathf.Cos(angleToTether * Mathf.Deg2Rad) + (rb.mass * Mathf.Pow(rb.velocity.magnitude, 2) / tetherLength);

        //Vector3 projectOnRightPlane = Vector3.ProjectOnPlane(tetherDirection, transform.right);

        //rb.AddForce(tension * projectOnRightPlane.normalized);
        rb.AddForce(tension * tetherDirection.normalized);

        //updates position 
        tetherDirection = attachedTetherPoint.transform.position - transform.position;
        angleToTether = Vector3.Angle(tetherDirection.normalized, attachedTetherPoint.transform.up);
        //Debug.Log(angleToTether);
        
        ApplyVelocityCutoff();

        //trying to further minimize swinging around point 
        //Vector3 projectionOnFrontNormal = Vector3.ProjectOnPlane(tetherDirection, transform.forward);
        //float distanceOnPlane = projectionOnFrontNormal.magnitude;
        //if(distanceOnPlane > 3)
        //{
        //    if(Vector3.Dot(transform.right, tetherDirection) > 0)
        //    {
        //        rb.AddForce(transform.right * 0.5f, ForceMode.Force);
        //    }
        //    else if (Vector3.Dot(transform.right, tetherDirection) < 0)
        //    {
        //        rb.AddForce(-transform.right * 0.5f, ForceMode.Force);
        //    }
        //}

        swingDirection = Vector3.Dot(player.transform.forward, player.rb.velocity);
        anim.SetFloat("SwingDir", swingDirection);
        //Debug.Log(swingDirection);

    }

    //speed up the character on seperate input input
    public void SpeedUp()
    {
        //Debug.Log("adding force");
        rb.AddForce(tetherDirection.normalized * 3, ForceMode.Force);
    }

    //detaches for the current tether point
    public void DetatchGrapple()
    {
        if (!tether)
            return;

        Debug.Log("grapple detached");
        player.isGrappling = false;
        anim.SetBool("Grapple", false);
        player.debugLine.GetComponent<LineRenderer>().enabled = false;

        rb.mass = 1;
        rb.AddForce(rb.velocity.normalized * launchSpeed, ForceMode.Impulse);
        tether = false;
        

        if (tetherPoint == attachedTetherPoint && isStaring == false)
            tetherPoint = null;

        attachedTetherPoint = null;
        player.attachedGrapplePoint = null;

        player.SetFlutter(true);

        player.SetMovementType(MovementType.air);

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

    //clamps the velocity to prevent flinging off the grapple point
    void ApplyVelocityCutoff()
    {
        rb.velocity = Mathf.Min(rb.velocity.magnitude, grappleMax) * rb.velocity.normalized;        
    }

    //check in direction of movement to detach grapple if hit something
    private void CheckCollisions()
    {
        //top of head raycast
        Vector3 topRaycastLocation = new Vector3(transform.position.x, transform.position.y + 0.5f * transform.localScale.y - 0.1f, transform.position.z);

        //distance checks slighly in front of player, may want ot change depending on play testing
        bool topOfHead = Physics.Raycast(topRaycastLocation, rb.velocity.normalized, 0.4f, player.airMask);//0.5f * transform.localScale.z + 0.1f);

        //toe  raycast
        Vector3 toeRaycastLocation = new Vector3(transform.position.x, transform.position.y - 0.5f * transform.localScale.y + 0.1f, transform.position.z);
        bool toeCast = Physics.Raycast(toeRaycastLocation, rb.velocity.normalized, 0.4f, player.airMask);

        RaycastHit hit;
        //mid raycast
        Vector3 midRaycastLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        bool midCast = Physics.Raycast(midRaycastLocation, rb.velocity.normalized, out hit, 0.4f, player.airMask);

        if(topOfHead || toeCast || midCast)
        {
            DetatchGrapple();
        }
    }
}

