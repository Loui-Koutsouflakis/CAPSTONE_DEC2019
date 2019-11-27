//created Aug 8, 2019 - air controller
//19/10/19 - modified by KE - cleaned redundant code, moved all initialization functions to the playerVariables script to reduce load times and optimize script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirMovement : PlayerVariables
{
    //wall check
    private bool onWall;
    private float wallCheckRate = 0.05f;
      
    //for ground pound
    private float DropForce = 20;
    
    //air max changes depending on the type of air movement (long/normal/high jumps)
    private float airMax = 12f;

    private bool deadJoy;
    private readonly float deadZone = 0.028f;
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 1.2f;

    //air control variables 
    private bool wallDeadZone;
    public bool doubleJumpControl;
    private bool wallRotate;
   
    //rayCasts
    private RaycastHit faceHit;

    private Vector3 wallNormal;
    
    //movement directions
    private float horizontal;
    private float vertical;
       
    private void OnEnable()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        //starts checking walls when ever the script is enabled
        StartCoroutine(WallWait());
        wallDeadZone = false;
        onWall = false;
        wallRotate = false;
        
        if(!player.GetBouncing())
        {
            doubleJumpControl = false;
        }
        else
        {
            doubleJumpControl = true;
            StartCoroutine(DoubleJumpControl());
        }


    }

    private void OnDisable()
    {
        //stops checking wall on disables to prevent multiple checks running
        StopAllCoroutines();
        //resets high and long jumps
        if (player)//or error on startup
        {
            player.SetHighJump(false);
            player.SetLongJump(false);
            player.SetDoubleJump(false);
            player.SetBouncing(false);
            anim.SetFloat("YVelocity", 0);
            anim.SetBool("GroundPound", false);
            wallRotate = false;
            ledgeHoping = false;

            if (ledgeHoping)
            {
                ledgeHoping = false;
                player.EnableControls();
                anim.SetBool("LedgeGrab", false);
            }

        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(player.GetControlsEnabled())
        {
            ControlInput();
        }

        //rotates the player when jumping off a wall
        if (wallRotate)
        {
            //Debug.Log("rotating");
            player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(player.transform.forward, transform.right, 10 * Time.fixedDeltaTime, 0.0f));
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
    }

    private void LateUpdate()
    {
        anim.SetBool("OnWall", onWall);
    }

    private void ControlInput()
    {
        //will slide a slower speed down wall
        if(onWall && player.rb.velocity.y <= 0)
        {
            player.rb.velocity += Vector3.up * Physics.gravity.y * (wallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if(player.rb.velocity.y < -peakTime && rb.velocity.y > peakTime)
        {
            player.rb.velocity += Vector3.up * Physics.gravity.y * (peakHeightMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (player.rb.velocity.y < peakTime)
        {
            player.rb.velocity += Vector3.up * Physics.gravity.y * EaseIn(fallMultiplier/fallTime) * Time.fixedDeltaTime;
        }
        //player will fall faster on way down
        else if (player.rb.velocity.y > peakTime && !Input.GetButton("AButton") && !player.GetHighJump() && !player.GetLongJump())
        {
            player.rb.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        Vector3 forward = transform.forward;
        Vector3 inputDir = transform.forward * vertical + transform.right * horizontal;

        //if move input then move if no input stop
        if (horizontal > deadZone || horizontal < -deadZone || vertical > deadZone || vertical < -deadZone)
        {
            if (!wallDeadZone)
            {
                if (onWall)
                {
                    if (Vector3.Dot(forward, inputDir) < 0) // to prevent sticking to walls (and not slidining down) when input is in the direction of the wall
                    {
                        AirMovement();
                    }
                    
                    player.transform.forward = -wallNormal;
                    if(Mathf.Abs(horizontal) < deadZone && Mathf.Abs(vertical) < deadZone)
                    {
                        player.GenericAddForce(player.transform.forward, 0.5f);
                    }
                }
                else
                {
                    AirMovement();
                }
            }
        }       
        
        ApplyVelocityCutoff();

        anim.SetFloat("YVelocity", player.rb.velocity.y);
        //Debug.Log(rb.velocity.y);
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
        //air script is different than the ground movement, as the character moves slower in the air and does NOT rotate to face camera forward instead will shift from side to side

        //following two lines are what we's use if we wanted to rotate character
        //player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical, airRotateSpeed * Time.fixedDeltaTime, 0.0f));
        //rb.AddForce(transform.forward * Mathf.Abs(vertical) * airForwardSpeed + cammyRight * horizontal * airSideSpeed, ForceMode.Force);


        if (player.GetRunningIntoWall() && Vector3.Dot(cammyFront * vertical * airForwardSpeed + cammyRight * horizontal, player.frontCheckNormal) < 0)
        {

        }
        else
        {
            //adds force to the player
            if (!doubleJumpControl)
            {
                player.rb.AddForce(cammyFront * vertical * airForwardSpeed + cammyRight * horizontal * airSideSpeed, ForceMode.Force);
                //Debug.DrawLine(player.transform.position, player.transform.position + cammyFront * vertical * airForwardSpeed + cammyRight * horizontal * airSideSpeed, Color.black, 1);
                //Debug.Log(cammyFront * vertical * airForwardSpeed + cammyRight * horizontal * airSideSpeed);
            }
            else
            {
                player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(player.transform.forward, cammyFront * vertical + cammyRight * horizontal, airRotateSpeed * Time.fixedDeltaTime, 0.0f));
                player.rb.AddForce(player.transform.forward * airForwardSpeed, ForceMode.Force);
                //Debug.Log(player.transform.forward);
                //Debug.Log(player.rb.velocity);
            }
        }
    }

    void ApplyVelocityCutoff()
    {
        //if long jumping has a higher air max to allow longer jumps
        if(player.GetLongJump())
        {
            airMax = longJumpMax;
        }
        else if(player.GetHighJump())
        {
            airMax = highJumpAirMax;
        }
        //air max of 12 is for normal jumps
        else if(player.GetDoubleJump())
        {
            airMax = doubleJumpMax;
        }
        else
        {
            airMax = testAirMax;
        }


        //horizontal clamp
        Vector3 horizontalVelocity = player.rb.velocity;
        horizontalVelocity.y = 0;
       
        horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, airMax) * horizontalVelocity.normalized;
              
        player.rb.velocity = horizontalVelocity + player.rb.velocity.y * Vector3.up;

        //vertical clamp
        Vector3 verticalVelocity = player.rb.velocity;
        verticalVelocity.x = 0;
        verticalVelocity.z = 0;
        verticalVelocity = Mathf.Max(verticalVelocity.magnitude, -10) * verticalVelocity.normalized;

        player.rb.velocity = verticalVelocity + player.rb.velocity.x * Vector3.right + player.rb.velocity.z * Vector3.forward;

        //Debug.Log(rb.velocity);
    }

    public void Jump()
    {
        //Vector3 inputDir = transform.forward * vertical + transform.right * horizontal;
        Vector3 inputDir = cammy.transform.forward * vertical + cammy.transform.right * horizontal;


        if (onWall && Vector3.Dot(wallNormal, inputDir) > 0) //wall jump only if pressing away from the wall (as per brad's request) if we want when no dir is pressed add >=
        {                   
            anim.SetBool("WallJumpB", true);
            
            //zero out velocity
            player.rb.velocity = Vector3.zero;
            //jump off the wall
            player.rb.AddForce((-player.transform.forward * wallJumpHorizontal) + (transform.up * wallJumpVertical), ForceMode.Impulse);
            //sets player looking away from wall (two ways to do it)         
            wallRotate = true;

            onWall = false;
            wallDeadZone = true;
            StartCoroutine(WallDeadZone());
            player.SetFlutter(true);
            player.SetDoubleJump(false);            
        }
        else if (player.CanFlutter())
        {
            //Debug.Log("Flutter Jump");
            //zero out velocity at start of flutter jump to prevent to much height
            Vector3 tempVelocity = player.rb.velocity;
            tempVelocity.y = 0;
            player.rb.velocity = tempVelocity;

            anim.SetTrigger("DJump");

            onWall = false;

            //to allow control for a brief period after double jump
            doubleJumpControl = true;
            StartCoroutine(DoubleJumpControl());

            player.rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
            player.SetFlutter(false);
            player.SetDoubleJump(true);
        }

        if (player.transform.parent != null && !GetComponentInParent<PlayerController>().bossLevel)
        {
            player.transform.parent = null;
        }

        //resets air control when jumping out of a long jump or high jump
        player.SetHighJump(false);
        player.SetLongJump(false);
    }

    public void GroundPound()
    {
        Debug.Log("Start pound");
        player.rb.isKinematic = true;
        player.DisableControls();
        StartCoroutine(GroundPoundFloat());
        
        anim.SetBool("GroundPound", true);
    }

    IEnumerator GroundPoundFloat()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("start drop");
        player.rb.isKinematic = false;
        player.EnableControls();
        player.rb.AddForce(transform.up * -DropForce, ForceMode.Impulse); // Force Down
    }


    private bool ledgeHoping = false;
    public IEnumerator CheckWall()
    {
        //top of head boxcast
        Vector3 topRaycastLocation = new Vector3(transform.position.x, transform.position.y + 0.5f * transform.localScale.y - 0.1f, transform.position.z);
        Vector3 topRaycastHalf = new Vector3(0.5f * transform.localScale.x, 0.1f, 0.5f * transform.localScale.z);

        //euler in box cast currently checks all around player would need to change if only want it in the front
        //bool topOfHead = Physics.BoxCast(topRaycastLocation, topRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        //distance checks slighly in front of player, may want ot change depending on play testing
        bool topOfHead = Physics.Raycast(topRaycastLocation, transform.forward, 0.5f * transform.localScale.z + 0.1f);
       
        //toe  raycast
        Vector3 toeRaycastLocation = new Vector3(transform.position.x, transform.position.y - 0.5f * transform.localScale.y + 0.1f, transform.position.z);
        Vector3 toeRaycastHalf = new Vector3(0.5f * transform.localScale.x, 0.1f, 0.5f * transform.localScale.z);

        //bool toeCast = Physics.BoxCast(toeRaycastLocation, toeRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        bool toeCast = Physics.Raycast(toeRaycastLocation, transform.forward, 0.5f * transform.localScale.z + 0.1f);
       
        RaycastHit hit;
        //mid raycast
        Vector3 midRaycastLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 midRaycastHalf = new Vector3(0.1f, 0.2f, 0.1f);

        //bool midCast = Physics.BoxCast(minRaycastLocation, toeRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        bool midCast = Physics.Raycast(midRaycastLocation, transform.forward, out hit, 0.5f * transform.localScale.z + 0.1f);
        bool midBoxCast = Physics.BoxCast(midRaycastLocation, midRaycastHalf, transform.forward, Quaternion.identity, 0.25f, player.airMask );

        //if all three
        if (toeCast && midCast && topOfHead && Vector3.Dot(cammy.transform.forward * vertical * airForwardSpeed + cammy.transform.right * horizontal, player.frontCheckNormal) < 0)
        {
            onWall = true;
            wallNormal = hit.normal;
        }
        //else if (toeCast && !midCast && !topOfHead)
        //{
        //    onWall = false;
        //    //call function to move player up on top of platform if we want
        //}
        //else if (midBoxCast && !topOfHead)
        //{            
        //    onWall = false;
        //    if (!ledgeHoping)
        //    {
        //        player.rb.velocity = Vector3.zero;
        //        player.rb.isKinematic = true;
        //        player.DisableControls();
        //        anim.SetBool("LedgeGrab", true);
        //        Debug.Log("triggered");
        //        StartCoroutine(LedgeHopStart());
        //        ledgeHoping = true;
        //    }
        //    //ledge grab if we have it
        //}
        else if(!toeCast || !midCast || !topOfHead)
        {
            onWall = false;
        }

        yield return new WaitForSecondsRealtime(wallCheckRate);

        StartCoroutine(CheckWall());
    }

    IEnumerator LedgeHopStart()
    {
        yield return new WaitForSeconds(0.15f);
        player.rb.isKinematic = false;
        player.GenericAddForce(transform.up, 5.5f);
        StartCoroutine(LedgeHopFinish());
        //for jump bug fix
        player.SetGroundCheck(false);
        player.StartCoroutine(player.GroundCheckStop());
    }
    IEnumerator LedgeHopFinish()
    {
        yield return new WaitForSeconds(0.15f);
        Debug.Log("forward force");
        ledgeHoping = false;
        player.GenericAddForce(transform.forward, 3.5f);
        player.EnableControls();
        anim.SetBool("LedgeGrab", false);
    }

    //wait for time after air controller is initially enabled to prevent sticking to wall initially if next to wall
    IEnumerator WallWait()
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(CheckWall());
    }

    //prevents player input for a time directly after wall jumping
    IEnumerator WallDeadZone()
    {
        yield return new WaitForSeconds(0.3f);
        wallDeadZone = false;
        wallRotate = false;
        anim.SetBool("WallJumpB", false);
    }

    IEnumerator DoubleJumpControl()
    {
        yield return new WaitForSeconds(0.75f);
        doubleJumpControl = false;
        player.SetBouncing(false);
    }

}
