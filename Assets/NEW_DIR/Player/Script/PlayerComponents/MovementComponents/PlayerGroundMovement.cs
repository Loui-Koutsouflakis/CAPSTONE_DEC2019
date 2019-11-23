//created 19-08-09 - ground movement scrip based on old player movement script by AT edited by LF
//keeps ground movement but removed air funtionallity
//19/10/19 - modified by KE - cleaned redundant code, moved all initialization functions to the playerVariables script to reduce load times and optimize script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Ground Movement", 7)]

public class PlayerGroundMovement : PlayerVariables
{   
    private bool deadJoy;
    private readonly float deadZone = 0.028f;
    private readonly float decelFactor = 0.14f;
       
    //for animation
    private float moveSpeed;
    private float rotate;
    private float animRotate;
    private float flairTimer;
    private float flairRandom;

    //movement directions
    private float horizontal;
    private float vertical;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player.GetControlsEnabled())
        {
            ControlInput();
        }

        setSpeed();//for animations
        setRotate();//for animations
    }

    private void Update()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
    }

    private void OnEnable()
    {        
        flairRandom = Random.Range(5, 10);

        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
    }

    private void OnDisable()
    {
        flairTimer = 0;
        anim.SetBool("Idle", false);
    }

    public void ControlInput()
    {
        //if move input then move if no input stop
        if (horizontal > deadZone || horizontal < -deadZone || vertical > deadZone || vertical < -deadZone)
        {
            if (horizontal > movementThreshold || horizontal < -movementThreshold || vertical > movementThreshold || vertical < -movementThreshold)
                Movement(walkAccel);
            else 
                Movement(slowWalk);
        }
        else if (horizontal <= deadZone && horizontal >= -deadZone && vertical <= deadZone && vertical >= -deadZone && Mathf.Abs(player.rb.velocity.y) < 0.1f)
        {
            //if (!player.GetBouncing())
            //{
                Decel();
                //Debug.Log("decel");
            //}
        }

        //idle waiting animation (for testing move into above function when working)
        if(Mathf.Abs(horizontal) <= deadZone && Mathf.Abs(vertical) <= deadZone)
        {
            flairTimer += Time.deltaTime;
            if(flairTimer >= flairRandom)
            {
                anim.SetBool("Idle", true);
                flairTimer = 0;
                flairRandom = Random.Range(5, 10);
            }
        }
        else
        {
            flairTimer = 0;
            anim.SetBool("Idle", false);
        }
               
        ApplyVelocityCutoff();
    }

    public void Movement(float speed)
    {
        //movement based on direction camera is facing
        Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
        Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
        cammyRight.y = 0;
        cammyFront.y = 0;
        cammyRight.Normalize();
        cammyFront.Normalize();

        player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(player.transform.forward, cammyFront * vertical + cammyRight * horizontal, rotateSpeed * Time.fixedDeltaTime, 0.0f));
        //adds force to the player
        player.rb.AddForce(player.transform.forward * speed, ForceMode.Force);

        Friction();
    }    

    //clamps the velocity
    void ApplyVelocityCutoff()
    {
        Vector3 horizontalVelocity = player.rb.velocity;
        horizontalVelocity.y = 0;
        
        horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, walkMax) * horizontalVelocity.normalized;
       
        player.rb.velocity = horizontalVelocity + player.rb.velocity.y * Vector3.up;
    }

    void Friction()
    {
        float friction = player.GetFriction() * rb.mass * Physics.gravity.magnitude;
        Vector3 groundNormal = player.GetFootHit().normal;
        Vector3 normalComponentofVelocity = Vector3.Dot(player.rb.velocity, groundNormal) * groundNormal;
        Vector3 parallelComponentofVelocity = player.rb.velocity - normalComponentofVelocity;
        player.rb.AddForce(-friction * parallelComponentofVelocity);
    }
    
    public void Jump()
    {
        //Debug.Log("Normal Jump");
        player.rb.velocity /= 2;

        player.rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (player.transform.parent != null && !GetComponentInParent<PlayerController>().bossLevel)
        {
            player.transform.parent = null;
        }

        //for jump bug fix
        player.SetGroundCheck(false);
        player.StartCoroutine(player.GroundCheckStop());

        anim.SetTrigger("Jump");              
    }

    public void Decel()
    {
        if (!deadJoy)
        {
            deadJoy = true;
        }

        if (player.rb.velocity.magnitude > 1f && player.IsGrounded())
        {
            player.rb.velocity = Vector3.Lerp(player.rb.velocity, Vector3.zero, decelFactor);
        }
    }
       
    //for animator
    public void setSpeed()
    {
        moveSpeed = player.rb.velocity.magnitude / walkMax;
        anim.SetFloat("Speed", moveSpeed);
    }

    public void setRotate()
    {
        animRotate = rotate / rotateSpeed;
        anim.SetFloat("Rotate", animRotate);
    }
}
