// Sebastian Borkowski
// edited by AT 19-08-14 - changed to support new architecture
//19/10/19 - modified by KE - cleaned redundant code, moved all initialization functions to the playerVariables script to reduce load times and optimize script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Crouch Movement", 6)]

public class Crouch : PlayerVariables
{
    //raycasts
    private RaycastHit footHit;
    
    protected bool deadJoy;
    protected readonly float deadZone = 0.028f;
    protected readonly float decelFactor = 0.14f;   

    public bool enteredScript;

    //movespeed to feed into animaator
    float moveSpeed;

    //movement directions
    private float horizontal;
    private float vertical;

    private HandleSfx SoundManager;
   

    // Start is called before the first frame update
    void Start()
    {
        enteredScript = true;
        SoundManager = GetComponentInParent<HandleSfx>();
    }

    public void OnEnable()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        enteredScript = true;
        StartCoroutine(LongJumpTimer());
        anim.SetBool("Crouching", true);
        anim.SetBool("HighJumpB", false);
        anim.SetBool("LongJump", false);
    }

    public void OnDisable()
    {
        enteredScript = false;
        anim.SetBool("Crouching", false);
        if(player.rb.isKinematic == true)
        {
            player.rb.isKinematic = false;
        }
    }   

    void FixedUpdate()
    {
        if(player.GetControlsEnabled())
        { 
            ControlInput();
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
    }

    public void ControlInput()
    {
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

        //animations
        moveSpeed = rb.velocity.magnitude / crouchMax;
        anim.SetFloat("CrouchSpeed", moveSpeed);
    }

    public void Jump()
    {       
        if(enteredScript && Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical)) >= deadZone && player.rb.velocity.magnitude >= 0.2f)
        {
            anim.SetBool("LongJump", true);
            LongJump();
            //Debug.Log("long jump");
        }
        else
        {
            anim.SetBool("HighJumpB", true);
            HighJump();
            //Debug.Log("high jump");
        }
        player.StartCoroutine(player.GroundCheckStop());
        

        if (player.transform.parent != null && !GetComponentInParent<PlayerController>().bossLevel)
        {
            player.transform.parent = null;
        }

        //for jump bug fix
        player.SetGroundCheck(false);
        player.StartCoroutine(player.GroundCheckStop());
    }

    public void HighJump() // will apply force upwards similar to the hight of the double jump.
    {
        rb.AddForce(player.transform.up * highJumpForce, ForceMode.Impulse);
        player.SetHighJump(true);
        anim.SetBool("Grounded", false);
        SoundManager.PlayOneShotByName("Jump");
    }

    public void LongJump() // will apply significant forward force with little upwards force, creating a long jump.
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(player.transform.up * longJumpUpForce + player.transform.forward * longJumpForwardForce, ForceMode.Impulse);
        player.SetLongJump(true);
        anim.SetBool("Grounded", false);
        SoundManager.PlayOneShotByName("Jump");
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
        player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(player.transform.forward, cammyFront * vertical + cammyRight * horizontal, crouchRotateSpeed * Time.fixedDeltaTime, 0.0f));

        //adds force to the player
        rb.AddForce(player.transform.forward * crouchAccel, ForceMode.Force);

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
        float friction = player.GetFriction() * rb.mass * Physics.gravity.magnitude;
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

        if (rb.velocity.magnitude > 1f && player.IsGrounded())
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelFactor);
        }
    }

    IEnumerator LongJumpTimer()
    {
        yield return new WaitForSeconds(1);
        enteredScript = false;
    }
}
