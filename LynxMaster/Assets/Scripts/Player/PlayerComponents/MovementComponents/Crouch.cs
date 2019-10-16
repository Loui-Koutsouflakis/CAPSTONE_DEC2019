// Sebastian Borkowski
// edited by AT 19-08-14 - changed to support new architecture

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Crouch Movement", 6)]

public class Crouch : PlayerVariables
{
    //rigidbody
    public Rigidbody rb;

    //camera object
    public Camera cammy;

    //raycasts
    private RaycastHit footHit;

    //vectors
    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);

    //priamtives
    //private
    protected float horizontal;
    protected float vertical;
    protected bool deadJoy;
    protected readonly float deadZone = 0.028f;
    protected readonly float decelFactor = 0.14f;

    //public - to test balance etc.
    //for movement
   
    
    private float frictionCoeff = 0.2f;

    public bool grounded;

    public bool enteredScript;

    //for jumps

  

    PlayerClass player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerClass>();

        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cammy = camObject.GetComponent<Camera>();
        enteredScript = false;
    }

    public void OnEnable()
    {
        enteredScript = true;
        StartCoroutine(LongJumpTimer());
    }

    public void OnDisable()
    {
        enteredScript = false;
    }

    IEnumerator LongJumpTimer()
    {
        yield return new WaitForSeconds(1);
        enteredScript = false;
    }

    void FixedUpdate()
    {
        grounded = player.IsGrounded();

        ControlInput();
    }

    public void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

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

    public void Jump()
    {
        //if(Mathf.Abs(horizontal) <= deadZone && Mathf.Abs(vertical) <= deadZone)
        //{
        //    HighJump();
        //}
        //else
        //{
        //    LongJump();
        //}
        if(enteredScript && Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical)) >= deadZone)
        {
            LongJump();
            Debug.Log("long jump");
        }
        else
        {
            HighJump();
            Debug.Log("high jump");
        }

        //for jump bug fix
        player.SetGroundCheck(false);
        player.StartCoroutine(player.GroundCheckStop());
    }

    public void HighJump() // will apply force upwards similar to the hight of the double jump.
    {
        rb.AddForce(transform.up * highJumpForce, ForceMode.Impulse);
        player.SetHighJump(true);
    }

    public void LongJump() // will apply significant forward force with little upwards force, creating a long jump.
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * longJumpUpForce + transform.forward * longJumpForwardForce, ForceMode.Impulse);
        player.SetLongJump(true);

    }

    public void Movement()
    {
        //same movement controls as normal, execpt slowe
        //movement based on direction camera is facing
        Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
        Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
        cammyRight.y = 0;
        cammyFront.y = 0;
        cammyRight.Normalize();
        cammyFront.Normalize();

        //rotates the direction the character is facing to the correct direction based on camera
        player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical + cammyRight * horizontal, crouchRotateSpeed * Time.fixedDeltaTime, 0.0f));

        //adds force to the player
        rb.AddForce(transform.forward * crouchAccel, ForceMode.Force);

        Friction();
    }

    void ApplyVelocityCutoff()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;

        horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, crouchMax) * horizontalVelocity.normalized;

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
}
