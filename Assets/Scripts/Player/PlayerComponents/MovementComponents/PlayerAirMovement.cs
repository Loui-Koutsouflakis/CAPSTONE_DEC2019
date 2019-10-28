//created Aug 8, 2019 - air controller
//19/10/19 - modified by KE - cleaned redundant code, moved all initialization functions to the playerVariables script to reduce load times and optimize script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirMovement : PlayerVariables
{
    //wall check
    private bool onWall;
    private float wallCheckRate = 0.1f;

    //for jumps
    private bool canFlutter;
   
    //for ground pound
    private float DropForce = 20;
    
    //need high airMax to allow long jump
    private float airMax = 12f;
    
    private float airRotateSpeed = 120f;

    private bool deadJoy;
    private readonly float deadZone = 0.028f;
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 1.2f;

    private bool wallDeadZone;
    public bool doubleJumpControl;

    private Vector3 wallNormal;
   
    //rayCasts
    private RaycastHit faceHit;

    private float horizontal;
    private float vertical;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //starts checking walls when ever the script is enabled
        StartCoroutine(WallWait());
        wallDeadZone = false;
        doubleJumpControl = false;
        onWall = false;
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
            anim.SetFloat("YVelocity", 0);
            anim.SetBool("GroundPound", false);
            //Debug.Log("script disabled");
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        canFlutter = player.CanFlutter();
        ControlInput();
    }

    private void Update()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
        anim.SetBool("OnWall", onWall);
    }    

    private void ControlInput()
    {

        //will slide a slower speed down wall
        if(onWall && rb.velocity.y <= 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (wallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if(rb.velocity.y < -peakTime && rb.velocity.y > peakTime)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (peakHeightMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y < peakTime)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * EaseIn(fallMultiplier/fallTime) * Time.fixedDeltaTime;
        }
        //player will fall faster on way down
        else if (rb.velocity.y > peakTime && !Input.GetButton("AButton") && !player.GetHighJump() && !player.GetLongJump())
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
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
             

        //adds force to the player
        if (!doubleJumpControl)
        {
            rb.AddForce(cammy.transform.forward * vertical * airForwardSpeed + cammyRight * horizontal * airSideSpeed, ForceMode.Force);
        }
        else
        {
            player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical + cammyRight * horizontal, airRotateSpeed * Time.fixedDeltaTime, 0.0f));
            rb.AddForce(transform.forward * airForwardSpeed, ForceMode.Force);
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

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
       
        horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, airMax) * horizontalVelocity.normalized;
       
        rb.velocity = horizontalVelocity + rb.velocity.y * Vector3.up;
    }

    public void Jump()
    {
        //Vector3 inputDir = transform.forward * vertical + transform.right * horizontal;
        Vector3 inputDir = cammy.transform.forward * vertical + cammy.transform.right * horizontal;


        if (onWall && Vector3.Dot(wallNormal, inputDir) > 0) //wall jump only if pressing away from the wall (as per brad's request) if we want when no dir is pressed add >=
        {
            //zero out velocity
            rb.velocity = Vector3.zero;
            // sets player looking away from wall (two ways to do it)         
            player.transform.forward = wallNormal;
            //jump off the wall
            rb.AddForce((transform.forward * wallJumpHorizontal) + (transform.up * wallJumpVertical), ForceMode.Impulse);

            anim.SetTrigger("WallJump");
            onWall = false;
            wallDeadZone = true;
            StartCoroutine(WallDeadZone());
            player.SetFlutter(true);
            player.SetDoubleJump(false);
        }
        else if (canFlutter)
        {
            //Debug.Log("Flutter Jump");
            //zero out velocity at start of flutter jump to prevent to much height
            Vector3 tempVelocity = rb.velocity;
            tempVelocity.y = 0;
            rb.velocity = tempVelocity;

            anim.SetTrigger("DJump");

            onWall = false;

            //to allow control for a brief period after double jump
            doubleJumpControl = true;
            StartCoroutine(DoubleJumpControl());

            rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
            player.SetFlutter(false);
            player.SetDoubleJump(true);
        }

        if (player.transform.parent != null)
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
        //player.rb.useGravity = false;
        //player.rb.velocity = Vector3.zero;
        StartCoroutine(GroundPoundFloat());
        //player.rb.AddForce(transform.up * -DropForce, ForceMode.Impulse); // Force Down
        
        anim.SetBool("GroundPound", true);
    }

    IEnumerator GroundPoundFloat()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("start drop");
        //Debug.Log("entered coroutine");
        player.rb.isKinematic = false;
        //player.rb.useGravity = true;
        player.rb.AddForce(transform.up * -DropForce, ForceMode.Impulse); // Force Down
    }

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
        Vector3 midRaycastHalf = new Vector3(0.5f * transform.localScale.x, 0.1f, 0.5f * transform.localScale.z);

        //bool midCast = Physics.BoxCast(minRaycastLocation, toeRaycastHalf, transform.forward, out faceHit, Quaternion.Euler(0, 2 * Mathf.PI, 0), 0.5f * transform.localScale.z + 0.1f);
        bool midCast = Physics.Raycast(midRaycastLocation, transform.forward, out hit, 0.5f * transform.localScale.z + 0.1f);
   
        //if all three
        if (toeCast && midCast && topOfHead)
        {
            onWall = true;
            wallNormal = hit.normal;
        }
        else if (toeCast && !midCast && !topOfHead)
        {
            onWall = false;
            //call function to move player up on top of platform if we want
        }
        else if (toeCast && midCast && !topOfHead)
        {
            onWall = false;
            //ledge grab if we have it
        }
        else
        {
            onWall = false;
        }

        yield return new WaitForSecondsRealtime(wallCheckRate);

        StartCoroutine(CheckWall());
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
    }

    IEnumerator DoubleJumpControl()
    {
        yield return new WaitForSeconds(0.75f);
        doubleJumpControl = false;
    }

}
