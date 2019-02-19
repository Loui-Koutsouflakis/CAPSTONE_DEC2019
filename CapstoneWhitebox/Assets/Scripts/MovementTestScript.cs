//Created February 19, 2019 by Alek Tepylo - based Loui's base movement script refined for more polished movement 
//Edited February 19, 2019, by Dylan LeClair - wall jump functionality

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTestScript : MonoBehaviour {

    //rigid body
    private Rigidbody rb;

    //camera object
    private Camera cammy;

    //raycast
    private RaycastHit footHit; //below player
    private RaycastHit faceHit; //infront of player

    //vectors
    private readonly Vector3 flyForce = new Vector3(0f, 4f, 0f);
    private readonly Vector3 lessOneY = new Vector3(0f, 0.7f, 0f);
    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);

    //priamtives
    //private
    private float horizontal;
    private float vertical;
    private float gravityArc = 0.9f;
    private bool deadJoy;
    private bool jumpHoldy;
    private readonly float deadZone = 0.028f;
    private readonly float groundGravity = 4.6f; //gravity when player is in ground
    private readonly float dropGravity = 5.3f; //gravity when player is droping
    private readonly float wallGravity = 0.1f; //gravity when player is sliding down a wall
    private readonly float initJumpGravity = 0.9f;
    private readonly float gravitySlope = 1.7f;
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 3.6f;
    private readonly float groundCheckRate = 0.1f;
       
    //public - to test balance etc.
    public float walkAccel = 10;
    public float walkMax = 12;
    public float airMax = 12;
    public float rotateSpeed = 30;
    public float jumpForce = 21.8f;
    private float forwardVelocity =5;
    public bool grounded;
    public bool onWall;
        
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        cammy = Camera.main;

        grounded = true;
        jumpHoldy = false;

        StartCoroutine(CheckGround());
        StartCoroutine(CheckWall());
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
        
        //if move input then move if no input stop
        if (horizontal > deadZone || horizontal < -deadZone || vertical > deadZone || vertical < -deadZone)
        {
            MoveTest();
        }
        else if (horizontal <= deadZone && horizontal >= -deadZone && vertical <= deadZone && vertical >= -deadZone)
        {
            Decel();
        }

        if (grounded)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, walkMax);
        }
        else if (!grounded)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, airMax);
        }
    }

    //jump physics using gravity 
    void GravityControl()
    {
        if (jumpHoldy)
        {
            gravityArc += Time.deltaTime * gravitySlope;
            rb.AddForce(Physics.gravity * rb.mass * gravityArc);
        }
        else if (!jumpHoldy)
        {
            gravityArc = initJumpGravity;

            if (grounded)
            {
                rb.AddForce(Physics.gravity * rb.mass * groundGravity);
            }
            else if(onWall)
            {
                rb.AddForce(Physics.gravity * rb.mass * wallGravity);
            }
            else if (!grounded && !onWall)
            {
                rb.AddForce(Physics.gravity * rb.mass * dropGravity);
            }
        }
    }

    //xz axis movement
    public void MoveTest()
    {
        //movement based on direction camera is facing
        Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
        Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
        cammyRight.y = 0;
        cammyFront.y = 0;
        cammyRight.Normalize();
        cammyFront.Normalize();

        //rotates the direction the character is facing to the correct direction based on camera
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront*vertical + cammyRight*horizontal, rotateSpeed * Time.fixedDeltaTime, 0.0f));
        
        //slight acceleration
        forwardVelocity += walkAccel * Time.fixedDeltaTime;
                
        rb.AddForce(transform.forward * forwardVelocity, ForceMode.Force);
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

        if (rb.velocity.magnitude == 0)
        {
            forwardVelocity = 0;
        }
    }

    public void Jump()
    {
        if (grounded)
        {
            rb.AddForce(new Vector3(0, jumpForce + rb.velocity.magnitude / velocityDivider), ForceMode.Impulse);
            jumpHoldy = true;
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
            rb.AddForce((-transform.forward + 2*transform.up) * jumpForce, ForceMode.Impulse);
            jumpHoldy = true;
            //sets player looking away from wall
            //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, -transform.forward, rotateSpeed * Time.fixedDeltaTime, 0.0f));
            transform.forward = -transform.forward * Time.deltaTime;
        }
        if (transform.parent != null)
        {
            transform.parent = null;
        }

        onWall = false;
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
        grounded = true;
    }

    public void WallMe()
    {
        onWall = true;
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
                    transform.parent = footHit.collider.gameObject.transform;
                    GroundMe();
                }

            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "MovingPlatform")
        {
            //checks 360% around player in xz axis to see if it contacts a wall
            if (Physics.BoxCast(transform.position, halves, Vector3.forward, out faceHit, Quaternion.identity, halves.y))
            {
                if (faceHit.collider.gameObject.tag == "Ground")
                {
                    WallMe();
                    //transform.forward = -(collision.gameObject.transform.position - transform.position).normalized;
                }

                if (faceHit.collider.gameObject.tag == "MovingPlatform")
                {
                    transform.parent = faceHit.collider.gameObject.transform;
                    WallMe();
                    //transform.forward = -(collision.gameObject.transform.position - transform.position).normalized;
                }

            }
        }
    }
     

}
