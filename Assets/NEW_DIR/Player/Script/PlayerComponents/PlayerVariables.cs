//Edit * Kyle 10/14/19 added movementThreshold and slowWalk variables for PlayerGroundMovementScript
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is a handy reference 
[AddComponentMenu("Player Scripts/Player Variables", 3)]
public class PlayerVariables : MonoBehaviour
{
//Change Variables in script between test plays to adjust values.
    [Header("Ground Movement")]
    public float walkAccel = 60;
    public float climbAccel = 12;
    public float walkMax = 6;
    public float rotateSpeed = 120;
    public float jumpForce = 5.5f;
    public float movementThreshold = 0.8f;
    public float slowWalk = 15;

    [Header("Air Movement")]
    public float jumpMultiplier = 3f;
    public float fallMultiplier = 3f;
    public float fallTime = 2.8f;
    public float peakTime = 0.3f;
    public float peakHeightMultiplier = 0.8f;
    public float doubleJumpForce = 5.8f;
    public float wallMultiplier = 0.5f;
    public float airForwardSpeed = 10f;
    public float airSideSpeed = 5f;
    public float airRotateSpeed = 120f;
    public float wallJumpVertical = 7;
    public float wallJumpHorizontal = 5;
    public float testAirMax = 6f;
    public float doubleJumpMax = 4f;
    public float longJumpMax = 8;
    public float highJumpAirMax = 1;

    [Header("Crouch Movement")]
    public float crouchAccel = 15;
    public float crouchMax = 6;
    public float crouchRotateSpeed = 120;
    public float longJumpUpForce = 4.8f; // upwards force.
    public float longJumpForwardForce = 7.5f; // forward force.
    public float highJumpForce = 7.9f; // crouch jump upwards force.

    [Header("Grapple Movement")]
    public float maxSwingSpeed = 20;
    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;
    public float launchSpeed;
    public float boost = 20;
    public float grappleMax = 12;

    [Header("Swimming Movement")]
    public float swimSpeed = 9;
    public float swimMax = 3;
    public float swimRotateSpeed = 100;



    protected float EaseIn(float t)
    {
        return t * t;
    }

    protected Rigidbody rb;
    protected Camera cammy;
    private RaycastHit footHit;
    protected Animator anim;
    protected PlayerClass player;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerClass>();
        anim = GetComponentInParent<PlayerClass>().GetAnimator();

        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cammy = camObject.GetComponent<Camera>();
    }
    //private void Awake()
    //{
    //    player = FindObjectOfType<PlayerClass>();
    //    rb = player.rb;
    //    anim = player.GetAnimator();
    //}



    //************************************************************Settled upon variables*********************************************************************
    //[Header("Ground Movement")]
    //public float walkAccel = 60;
    //public float climbAccel = 12;
    //public float walkMax = 6;
    //public float rotateSpeed = 120;
    //public float jumpForce = 5.5f;


    //[Header("Air Movement")]
    //public float jumpMultiplier = 3f;
    //public float fallMultiplier = 3f;
    //public float fallTime = 2.8f;
    //public float peakTime = 0.3f;
    //public float peakHeightMultiplier = 0.8f;
    //public float doubleJumpForce = 5.8f;
    //public float wallMultiplier = 0.5f;
    //public float airForwardSpeed = 10f;
    //public float airSideSpeed = 5f;
    //public float wallJumpVertical = 7;
    //public float wallJumpHorizontal = 5;
    //public float testAirMax = 6f;
    //public float doubleJumpMax = 3.5f;
    //public float longJumpMax = 20;
    //public float highJumpAirMax = 1;

    //[Header("Crouch Movement")]
    //public float crouchAccel = 15;
    //public float crouchMax = 6;
    //public float crouchRotateSpeed = 120;
    //public float longJumpUpForce = 4.8f; // upwards force.
    //public float longJumpForwardForce = 12; // forward force.
    //public float highJumpForce = 7.9f; // crouch jump upwards force.

    //[Header("Grapple Movement")]
    //public float maxSwingSpeed;
    //public float forwardSpeed = 100;
    //public float sidewaysSpeed = 8;
    //public float jumpSpeed = 10;
    //public float launchSpeed;
    //public float boost = 20;
}
