// Sebastian Borkowski

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
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
    public float crouchAccel = 20;
    public float crouchMax = 8;
    public float airMax = 8;
    public float rotateSpeed = 30;
    private float frictionCoeff = 0.1f;

    public bool grounded;

    //for jumps
    public float jumpForce = 10; // upwards force.
    public float forwardForce = 20; // forward force.
    public float crouchJumpForce = 20; // crouch jump upwards force.
    public float jumpMultiplier = 1.5f;
    //public float fallMultiplier = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        grounded = true;

        StartCoroutine(CheckGround());
    }


    void FixedUpdate()
    {
        ControlInput();

    }

    public void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        //jump
        if (Input.GetButtonDown("Jump"))
        {
           if(rb.velocity.x <= 0.1 || rb.velocity.z <= 0.1)
            {
                LongJump();
            }
            else
            {
                HighJump();
            }
        }
                     
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
            Movement();
        }
        else if (horizontal <= deadZone && horizontal >= -deadZone && vertical <= deadZone && vertical >= -deadZone)
        {
            Decel();
        }

        

        ApplyVelocityCutoff();        

    }

    public void HighJump() // will apply force upwards similar to the hight of the double jump.
    {
        rb.AddForce(transform.up * crouchJumpForce, ForceMode.Impulse);
    }

    public void LongJump() // will apply significant forward force with little upwards force, creating a long jump.
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
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
        rb.AddForce(transform.forward * crouchAccel, ForceMode.Force);

        if (grounded)
        {
            Friction();
        }
    }

    void Friction()
    {
        float friction = frictionCoeff * rb.mass * Physics.gravity.magnitude;
        Vector3 groundNormal = footHit.normal;
        Vector3 normalComponentofVelocity = Vector3.Dot(rb.velocity, groundNormal) * groundNormal;
        Vector3 parallelComponentofVelocity = rb.velocity - normalComponentofVelocity;
        rb.AddForce(-friction * parallelComponentofVelocity);
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

        if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y))
        {
            if (footHit.collider.gameObject.tag == "MovingPlatform")
            {
                transform.parent = footHit.transform.parent;
            }

        }
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

    void ApplyVelocityCutoff()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (grounded)
        {
            horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, crouchMax) * horizontalVelocity.normalized;
        }
        else
        {
            horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, airMax) * horizontalVelocity.normalized;
        }
        rb.velocity = horizontalVelocity + rb.velocity.y * Vector3.up;
    }
}
