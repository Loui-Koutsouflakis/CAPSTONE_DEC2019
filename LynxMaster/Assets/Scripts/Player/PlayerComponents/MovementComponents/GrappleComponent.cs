using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//updated 05/28- LF
//updated 08/06
[AddComponentMenu("Player Scripts/Grapple Movement", 4)]

public class GrappleComponent : MonoBehaviour
{
    public Rigidbody rb;

    public Transform attachedTetherPoint;
    public bool tether = false;
    public float maxSwingSpeed = 20;
    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;
    public float launchSpeed;
    public float boost = 20;
    private bool reachedZero = false;

    public bool isStaring;

    [Range(20, 90)]
    private float maxAngle = 60;

    [SerializeField]
    PlayerClass player;

    public Transform tetherPoint;

    // Start is called before the first frame update

    public void Initialize()
    {
        player = GetComponentInParent<PlayerClass>();
        rb = GetComponentInParent<Rigidbody>();
    }

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

        player.debugLine.GetComponent<LineRenderer>().enabled = true;

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

        player.debugLine.SetPosition(0, player.transform.position);
        player.debugLine.SetPosition(1, attachedTetherPoint.position);

        rb.mass = 100;
        Vector3 radial = (attachedTetherPoint.position - transform.position).normalized;
        Vector3 radialVelocityComponent = Vector3.Dot(rb.velocity, radial) * radial;
        rb.velocity -= radialVelocityComponent;
        rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, maxSwingSpeed);

        Vector3 tetherDirection = attachedTetherPoint.position - transform.position;
        float tetherLength = tetherDirection.magnitude;
        float x = Vector3.Dot(tetherDirection.normalized, Vector3.up);
        float y = (tetherDirection.normalized - x * Vector3.up).magnitude;
        float theta = Mathf.Atan2(y, x);
        float tension = rb.mass * 9.8f * 8 * Mathf.Cos(theta) + rb.mass * Mathf.Pow(rb.velocity.magnitude, 2) / tetherLength;
        rb.AddForce(tension * tetherDirection.normalized);
        Debug.DrawLine(transform.position, attachedTetherPoint.position);
        float thetaDegrees = theta * Mathf.Rad2Deg;
        //Debug.Log(thetaDegrees);

        Vector3 heading = attachedTetherPoint.position - transform.position;
        float dot = Vector3.Dot(heading, transform.forward);
        if (dot < 0)
        {
            Debug.Log("Reached Zero");
            reachedZero = true;
        }

        if (reachedZero)
        {
            rb.AddForce(rb.velocity.normalized * boost, ForceMode.Impulse);

            if (thetaDegrees > maxAngle)
            {
                DetatchGrapple();
                //rb.mass = 1;
                //rb.AddForce(rb.velocity.normalized * launchSpeed, ForceMode.Impulse);
                //normalMove.enabled = true;
                //tether = false;
                //attachedTetherPoint = null;
                // reachedZero = false;
            }

        }
    }

    
    public void DetatchGrapple()
    {
        if (!tether)
            return;

        Debug.Log("grapple detached");
        player.isGrappling = false;

        rb.mass = 1;
        rb.AddForce(rb.velocity.normalized * launchSpeed, ForceMode.Impulse);
        tether = false;

        if (tetherPoint == attachedTetherPoint && isStaring == false)
            tetherPoint = null;

        attachedTetherPoint = null;

        reachedZero = false;

        player.debugLine.enabled = false;
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

