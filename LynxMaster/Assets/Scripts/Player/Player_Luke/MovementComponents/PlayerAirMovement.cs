//created Aug 8, 2019 - air controller

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirMovement : MonoBehaviour
{
    //architecture
    PlayerClass player;

    //rigid body
    private Rigidbody rb;

    //camera object
    private Camera cammy;

    //wall check
    private bool onWall;
    private float wallCheckRate = 0.1f;

    //for animator
    private Animator anim;

    //for jumps
    private bool canFlutter;
    private float flutterForce = 5;

    //gravity modifiers
    private float jumpMultiplier = 1.5f;
    private float fallMultiplier = 5f;
    private float wallMultiplier = 0.5f;

    //air movement
    private float horizontal;
    private float vertical;
    private float airForwardSpeed = 10f;
    private float airSideSpeed = 5f;
    //need high airMax to allow long jump
    private float airMax = 50f;
    private float rotateSpeed = 10f;

    private bool deadJoy;
    private readonly float deadZone = 0.028f;
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 1.2f;


    private bool wallDeadZone;


    //rayCasts
    private RaycastHit faceHit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerClass>();
        anim = GetComponentInParent<PlayerClass>().GetAnimator();

        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cammy = camObject.GetComponent<Camera>();

    }

    private void OnEnable()
    {
        StartCoroutine(WallWait());
        wallDeadZone = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        canFlutter = player.CanFlutter();

        ControlInput();

    }
           
    private void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        //will slide a slower speed down wall
        if(onWall)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (wallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        //if move input then move if no input stop
        if (horizontal > deadZone || horizontal < -deadZone || vertical > deadZone || vertical < -deadZone)
        {
            if (!wallDeadZone)
            {
                AirMovement();
            }
        }       
        
        ApplyVelocityCutoff();
        
    }

    public void AirMovement()
    {
        //movement based on direction camera is facing
        Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
        Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
        cammyRight.y = 0;
        cammyFront.y = 0;
        cammyRight.Normalize();
        cammyFront.Normalize();

        //rotates the direction the character is facing to the correct direction based on camera
        //air script is different than the ground movement, as the character moves slower in the air and can shift from side to side
        //player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical, rotateSpeed * Time.fixedDeltaTime, 0.0f));
        
        //adds force to the player
        //rb.AddForce(transform.forward * Mathf.Abs(vertical) * airForwardSpeed + cammyRight * horizontal * airSideSpeed, ForceMode.Force);
        rb.AddForce(transform.forward *vertical * airForwardSpeed + cammyRight * horizontal * airSideSpeed, ForceMode.Force);

    }

    void ApplyVelocityCutoff()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
       
        horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, airMax) * horizontalVelocity.normalized;
       
        rb.velocity = horizontalVelocity + rb.velocity.y * Vector3.up;
    }

    public void Jump()
    {       
        if(canFlutter && !onWall)
        {
            //Debug.Log("Flutter Jump");
            //zero out velocity at start of flutter jump to prevent to much height
            Vector3 tempVelocity = rb.velocity;
            tempVelocity.y = 0;
            rb.velocity = tempVelocity;

            rb.AddForce(transform.up * flutterForce, ForceMode.Impulse);
            player.SetFlutter(false);
        }
        else if(onWall)
        {
            
            // jumps off of wall
            rb.AddForce((-transform.forward * 7) + (transform.up * 5), ForceMode.Impulse);
            // sets player looking away from wall
            player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, -transform.forward, rotateSpeed * Time.fixedDeltaTime, 0.0f));
            player.transform.forward = -transform.forward;
                        
            onWall = false;
            wallDeadZone = true;
            StartCoroutine(WallDeadZone());
        }

        if (player.transform.parent != null)
        {
            player.transform.parent = null;
        }

        
    }


    public IEnumerator CheckWall()
    {
        //top of head boxcast
        Vector3 topRaycastLocation = new Vector3(transform.position.x, transform.position.y + 0.5f * transform.localScale.y - 0.1f, transform.position.z);
        Vector3 topRaycastHalf = new Vector3(0.5f * transform.localScale.x, 0.1f, 0.5f * transform.localScale.z);

        //euler in box cast currently checks all around player would need to change if only want it in the front
        //distance checks slighly in front of player, may want ot change depending on play testing
        //bool topOfHead = Physics.BoxCast(topRaycastLocation, topRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        bool topOfHead = Physics.Raycast(topRaycastLocation, transform.forward, 0.5f * transform.localScale.z + 0.1f);
        //if (topOfHead)
        //{
        //    Debug.Log("head hit");
        //}

        //toe  raycast
        Vector3 toeRaycastLocation = new Vector3(transform.position.x, transform.position.y - 0.5f * transform.localScale.y + 0.1f, transform.position.z);
        Vector3 toeRaycastHalf = new Vector3(0.5f * transform.localScale.x, 0.1f, 0.5f * transform.localScale.z);

        //bool toeCast = Physics.BoxCast(toeRaycastLocation, toeRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        bool toeCast = Physics.Raycast(toeRaycastLocation, transform.forward, 0.5f * transform.localScale.z + 0.1f);
        //if (toeCast)
        //{
        //    Debug.Log("mid hit");
        //}

        //mid raycast
        Vector3 midRaycastLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 midRaycastHalf = new Vector3(0.5f * transform.localScale.x, 0.1f, 0.5f * transform.localScale.z);

        //bool midCast = Physics.BoxCast(minRaycastLocation, toeRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        bool midCast = Physics.Raycast(midRaycastLocation, transform.forward, 0.5f * transform.localScale.z + 0.1f);
        //if (midCast)
        //{
        //    Debug.Log("toe hit");
        //}

        if (toeCast && midCast && topOfHead)
        {
            onWall = true;
        }
        else if (toeCast && !midCast && !topOfHead)
        {
            //call function to move player up on top of platform
        }
        else if (toeCast && midCast && !topOfHead)
        {
            //ledge grab if we have it
        }
        else
        {
            onWall = false;
        }

        yield return new WaitForSecondsRealtime(wallCheckRate);

        StartCoroutine(CheckWall());
    }

    IEnumerator WallWait()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckWall());
    }

    IEnumerator WallDeadZone()
    {
        yield return new WaitForSeconds(0.5f);
        wallDeadZone = false;
    }

}
