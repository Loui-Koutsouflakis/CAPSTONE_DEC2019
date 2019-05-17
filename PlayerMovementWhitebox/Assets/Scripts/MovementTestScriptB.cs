﻿//Created February 19, 2019 by Alek Tepylo - based Loui's base movement script refined for more polished movement 
//Edited February 19, 2019, by Dylan LeClair - wall jump functionality
//Edited March 4, 2019, by Dylan LeClair - flutter jump functionality
//Edited March 20, 2019, by Loui Koutsouflakis - Balancing of values and flutter jump,
//                                             - Added FlutterCheck Coroutine, 
//                                             - Added slight impulse to grounded jump

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTestScriptB : MonoBehaviour {

    //rigid body
    public Rigidbody rb;

    //camera object
    public Camera cammy;

    //raycast
    private RaycastHit footHit; //below player
    private RaycastHit faceHit; //infront of player

    //vectors
    private readonly Vector3 flyForce = new Vector3(0f, 4f, 0f);
    private readonly Vector3 lessOneY = new Vector3(0f, 0.7f, 0f);
    private readonly Vector3 initImpulse = new Vector3(0f, 14f, 0f);
    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);

    //priamtives
    //private
    private float horizontal;
    private float vertical;
    private float gravityArc = 0.9f;
    private float flutterBalance = 0f;
    private bool deadJoy;
    private bool jumpHoldy;
    private readonly float deadZone = 0.028f;
    private readonly float groundGravity = 4.6f; //gravity when player is in ground
    private readonly float dropGravity = 5.5f; // 6.5f //gravity when player is droping
    private readonly float wallGravity = 0.75f; //gravity when player is sliding down a wall
    private readonly float wallClimbGravity = -.98f;
    private readonly float initJumpGravity = 0.9f;
    private readonly float gravitySlope = 1.4f; //5.6f //1.7
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 1.2f;
    private readonly float groundCheckRate = 0.1f;
    private readonly float flutterDelay = 0.22f;
       
    //public - to test balance etc.

    public float walkAccel = 15;
    public float climbAccel = 12;
    public float walkMax = 8;
    public float airMax = 8;
    public float rotateSpeed = 30;
    public float jumpForce;
    private float forwardVelocity = 10;
    public bool grounded;
    public bool onWall;
    public float jumpMultiplier = 2f;
    public float fallMultiplier = 2.5f;
    public bool canFlutter;
    public float flutterForce = 5f;

    public float initJumpForce = 100;   
    private readonly float jumpSlope = 0.96f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();

        grounded = true;
        jumpHoldy = false;

        StartCoroutine(CheckGround());
        StartCoroutine(CheckWall());

        canFlutter = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        GravityControl();
        ControlInput();
    }

    public void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
        
        //jump
        if (Input.GetButtonDown("Jump") && onWall == true && grounded == false)
        {
            WallJump();
        }
        else if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpHoldy = false;
        }

        //another way of doing variable jump height
        //if (rb.velocity.y < 0)
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        //}
        //else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
        //}

        //if move input then move if no input stop
        if (horizontal > deadZone || horizontal < -deadZone || vertical > deadZone || vertical < -deadZone)
        {
            MoveTest();
        }
        else if (horizontal <= deadZone && horizontal >= -deadZone && vertical <= deadZone && vertical >= -deadZone)
        {
            Decel();
        }

        //Debug.Log(rb.velocity);

        //rb.velocity = Vector3.ClampMagnitude(rb.velocity, walkMax);
        if (rb.velocity.x > walkMax)
        {
            rb.velocity = new Vector3(walkMax, rb.velocity.y, rb.velocity.z);
        }
        else if (rb.velocity.x < -walkMax)
        {
            rb.velocity = new Vector3(-walkMax, rb.velocity.y, rb.velocity.z);
        }
        if (rb.velocity.z > walkMax)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, walkMax);
        }
        else if (rb.velocity.z < -walkMax)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -walkMax);
        }

        //to keep max jump height when moving and stop "slow fall" if it is happening (occasionally have problems with it)
        //Vector3 Vel = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, walkMax);
        //Vel.y = rb.velocity.y;
        //rb.velocity = Vel;
    }

    //jump physics using gravity 
    void GravityControl()
    {
        if (jumpHoldy)
        {

            if (jumpForce > 50)
            {
                jumpForce *= jumpSlope;
                //rb.AddForce(new Vector3(rb.velocity.x, jumpForce + rb.velocity.magnitude / velocityDivider, rb.velocity.z), ForceMode.Acceleration);
                rb.AddForce(transform.up * (jumpForce + rb.velocity.magnitude / velocityDivider));
            }
            gravityArc += Time.deltaTime * gravitySlope;
            rb.AddForce(Physics.gravity * rb.mass * gravityArc);
        }

        if (!jumpHoldy)
        {
            //gravityArc = initJumpGravity;
            //jumpForce = initJumpForce;


            if (grounded)
            {
                rb.AddForce(Physics.gravity * rb.mass * groundGravity);
            }
            else if (onWall && !grounded)
            {
                //if we want wall slide
                rb.AddForce(Physics.gravity * rb.mass * wallGravity);

                //if we want wall climb
                //rb.AddForce(Physics.gravity * rb.mass * wallClimbGravity);
            }
            else if (!grounded && !onWall)
            {
                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Physics.gravity * rb.mass * groundGravity);
                }
                else
                {
                    rb.AddForce(Physics.gravity * rb.mass * dropGravity);
                }
            }
        }

        if(canFlutter && !grounded & !onWall)
        {
            flutterBalance += Time.deltaTime * 20f;
        }
    }

    //xz axis movement
    public void MoveTest()
    {
        if (onWall && !grounded) //wall climbing
        {
            rb.AddForce((transform.up * vertical + transform.right * horizontal) * climbAccel, ForceMode.Force);
        }
        else
        {
            //movement based on direction camera is facing
            Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
            Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
            cammyRight.y = 0;
            cammyFront.y = 0;
            cammyRight.Normalize();
            cammyFront.Normalize();
            //Debug.Log(cammyFront + " - " + cammyRight);
            //Debug.Log(transform.forward);
            //Debug.Log(rb.velocity);

            //rotates the direction the character is facing to the correct direction based on camera
            //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(
            //    transform.forward,
            //    cammyFront * vertical + cammyRight * horizontal,
            //    rotateSpeed * Time.fixedDeltaTime,
            //    0.3f));
            //Debug.Log(Vector3.RotateTowards(
            //    transform.forward,
            //    cammyFront * vertical + cammyRight * horizontal,
            //    rotateSpeed * Time.fixedDeltaTime,
            //    0.3f));

            //slight acceleration (if we want to try it)
            //forwardVelocity += walkAccel * Time.fixedDeltaTime;

            //adds force to the player
            //rb.AddForce(transform.forward, ForceMode.Force);

            rb.AddForce(
                Vector3.RotateTowards(
                    transform.forward,
                    cammyFront * vertical + cammyRight * horizontal,
                    rotateSpeed * Time.fixedDeltaTime,
                    0.0f)
                * walkAccel, ForceMode.Force);

            //Debug.Log(Vector3.RotateTowards(
            //        transform.forward,
            //        cammyFront * vertical + cammyRight * horizontal,
            //        rotateSpeed * Time.fixedDeltaTime,
            //        0.3f));
        }

    }

    //to decelerate if no input is held
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

        //if (rb.velocity == Vector3.zero)
        //{
        //    forwardVelocity = 5;

        //}
    }

    public void Jump()
    {

        if (grounded)
        {
            rb.AddForce(initImpulse, ForceMode.Impulse); //Tap jump should have some initial impulse
            StartCoroutine(FlutterCheck()); // Stops flutter jump from being activated immediately to prevent high soaring
            //rb.AddForce(new Vector3(rb.velocity.x, jumpForce + rb.velocity.magnitude / velocityDivider, rb.velocity.z), ForceMode.Impulse);
            jumpForce = initJumpForce;
            jumpHoldy = true;
        }
        else if(canFlutter && !grounded && !onWall)
        {
            Debug.Log(flutterBalance.ToString() + " - " + (rb.velocity.y/10f).ToString());
            rb.AddForce(transform.up * (flutterForce + flutterBalance - rb.velocity.y/3f), ForceMode.Impulse); // approx balance of flutter to gravity

            jumpHoldy = true;
            canFlutter = false;
        }

        if (transform.parent != null)
        {
            transform.parent = null;
        }

        grounded = false;
    }

    public void WallJump()
    {
        if (onWall)
        {
            //jumps off of wall
            rb.AddForce((-transform.forward * 10f) + (transform.up * 14f), ForceMode.Impulse);
            StartCoroutine(FlutterCheck());
            jumpHoldy = true;
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
        
    public IEnumerator FlutterCheck()
    {
        flutterBalance = 0f;
        yield return new WaitForSecondsRealtime(flutterDelay);
        Debug.Log("<color=green> Can Flutter </color>");
        canFlutter = true;
    }

    //checks to see if player is on ground
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

    //checks to see if player is on a wall
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

    public void GroundMe()
    {
        //Debug.Log("<color=purple> Ground Me </color>");
        grounded = true;
        canFlutter = false; // true
        flutterBalance = 0f;
        gravityArc = initJumpGravity;
        jumpForce = initJumpForce;
    }

    public void WallMe()
    {
        onWall = true;
        //for wall climbing to prevent player from floating up
        if (!grounded)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "MovingPlatform")
        {            
            if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y))
            {
                if (footHit.collider.gameObject.tag == "Ground")
                {
                    GroundMe();
                }

                if (footHit.collider.gameObject.tag == "MovingPlatform")
                {
                    transform.parent = footHit.collider.gameObject.transform.parent;
                    GroundMe();
                }

            }
        }
    }

    private void OnCollisionStay(Collision collision) // IMPORTANT: OnCollisionStay is quite expensive 
                                                      // as it runs a collision check every frame.
                                                      // Try and create this functionality without it!
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "MovingPlatform")
        {
            //checks 360% around player in xz axis to see if it contacts a wall
            if (Physics.BoxCast(transform.position, halves, Vector3.forward, out faceHit, Quaternion.identity, halves.y))
            {
                if (faceHit.collider.gameObject.tag == "Ground")
                {
                    WallMe();
                    transform.LookAt(collision.gameObject.transform);
                    //transform.forward = -(collision.gameObject.transform.position - transform.position).normalized;
                }

                if (faceHit.collider.gameObject.tag == "MovingPlatform")
                {
                    transform.parent = faceHit.collider.gameObject.transform;
                    WallMe();
                    transform.LookAt(collision.gameObject.transform);

                    //transform.forward = -(collision.gameObject.transform.position - transform.position).normalized;
                }

            }
        }
    }
    
}
