// Created April 30, 2019 by Alek Tepylo - updated movement script (no wall functionality)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementv2 : MonoBehaviour
{
    //rigidbody
    public Rigidbody rb;

    //camera object
    public Camera cammy;

    //raycasts
    private RaycastHit footHit;
    private RaycastHit faceHit;

    //vectors
    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);

    //priamtives
    //private
    private float horizontal;
    private float vertical;
    private bool deadJoy;
    private readonly float deadZone = 0.028f;
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 1.2f;
    private readonly float groundCheckRate = 0.1f;

    //public - to test balance etc.
    //for movement
    public float walkAccel = 30;
    public float climbAccel = 12;
    public float walkMax = 12;
    public float airMax = 8;
    public float rotateSpeed = 30;
    private float frictionCoeff = 0.2f;

    public bool grounded;
    public bool onWall;

    //for jumps
    public float jumpForce= 10;
    public float jumpMultiplier = 1.5f;
    public float fallMultiplier = 2.5f;
    public bool canFlutter;
    public float flutterForce = 5;

    //for grapple
    bool toggle;

    private GrappleComponent grapple;
    private PlayerMovementv2 normalMove;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //cammy = Camera.main;

        grounded = true;
        
        StartCoroutine(CheckGround());
        
        canFlutter = true;

        normalMove = GetComponent<PlayerMovementv2>();
        normalMove.enabled = true;

        grapple = GetComponent<GrappleComponent>();
        grapple.enabled = false;

    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        ControlInput();

        if(Input.GetKey(KeyCode.T))//t for testing
        {
            grapple.enabled = true;
            normalMove.enabled = false;
        }
    }

    public void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        //jump
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            Debug.Log(jumpForce);
        }
                     
        if (rb.velocity.y < 0)
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
            Movement();
        }
        else if (horizontal <= deadZone && horizontal >= -deadZone && vertical <= deadZone && vertical >= -deadZone)
        {
            Decel();
        }

        

        ApplyVelocityCutoff();        

    }

    public void Movement()
    {
        //movement based on direction camera is facing
        Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
        Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
        cammyRight.y = 0;
        cammyFront.y = 0;
        cammyRight.Normalize();
        cammyFront.Normalize();

        //rotates the direction the character is facing to the correct direction based on camera
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical + cammyRight * horizontal, rotateSpeed * Time.fixedDeltaTime, 0.0f));
                    
        //adds force to the player
        rb.AddForce(transform.forward * walkAccel, ForceMode.Force);

        if(grounded)
        {
            Friction();
        }
    }
    
    //clamps the velocity
    void ApplyVelocityCutoff()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (grounded)
        {
            horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, walkMax) * horizontalVelocity.normalized;
        }
        else
        {
            horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, airMax) * horizontalVelocity.normalized;
        }
        rb.velocity = horizontalVelocity + rb.velocity.y * Vector3.up;
    }

    void Friction()
    {
        float friction = frictionCoeff * rb.mass * Physics.gravity.magnitude;
        Vector3 groundNormal = footHit.normal;
        Vector3 normalComponentofVelocity = Vector3.Dot(rb.velocity, groundNormal) * groundNormal;
        Vector3 parallelComponentofVelocity = rb.velocity - normalComponentofVelocity;
        rb.AddForce(-friction * parallelComponentofVelocity);
    }


    public void Jump()
    {
        if (grounded)
        {
            //Debug.Log("Normal Jump");
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (canFlutter && !grounded && !onWall)
        {
            //Debug.Log("Flutter Jump");
            rb.AddForce(transform.up * flutterForce, ForceMode.Impulse);
            canFlutter = false;
        }

        if (transform.parent != null)
        {
            transform.parent = null;
        }

        grounded = false;
    }

    public void Decel()
    {
        if (!deadJoy)
        {
            deadJoy = true;
        }

        if (rb.velocity.magnitude > 1f && grounded)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelFactor);
        }
    }
    
    public IEnumerator CheckGround()
    {
        if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y))
        {
            GroundMe();
        }
        else
        {
            grounded = false;
        }

        yield return new WaitForSecondsRealtime(groundCheckRate);

        StartCoroutine(CheckGround());
    }
    public void GroundMe()
    {
        grounded = true;
        canFlutter = true;
    }

    /*
    public void WallJump()
    {
        if (onWall)
        {
            //jumps off of wall
            rb.AddForce((-transform.forward * 10) + (transform.up * 5), ForceMode.Impulse);
            //sets player looking away from wall
            //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, -transform.forward, rotateSpeed * Time.fixedDeltaTime, 0.0f));
            transform.forward = -transform.forward;
        }
        if (transform.parent != null)
        {
            transform.parent = null;
        }

        onWall = false;
    }

    public IEnumerator CheckWall()
    {
        if (Physics.BoxCast(transform.position, halves, transform.forward, out faceHit, Quaternion.Euler(0,2*Mathf.PI,0), halves.y))
        {
            WallMe();
        }
        else
        {
            onWall = false;
        }

        yield return new WaitForSecondsRealtime(groundCheckRate);

        StartCoroutine(CheckWall());
    }


    public void WallMe()
    {
        onWall = true;
    }
    */

}
