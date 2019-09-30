using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is a handy reference 

public class PlayerVariables : MonoBehaviour
{
    [Header("Ground Movement")]
    public float walkAccel = 60;
    public float climbAccel = 12;
    public float walkMax = 10;
    public float rotateSpeed = 120;


    [Header("Air Movement")]
    public float jumpMultiplier = 3f;
    public float fallMultiplier = 8f;
    public float peakTime = 0.3f;
    public float peakHeightMultiplier = 0.8f;
    public float doubleJumpForce = 7;
    public float wallMultiplier = 0.5f;
    public float airForwardSpeed = 10f;
    public float airSideSpeed = 5f;
    public float wallJumpVertical = 7;
    public float wallJumpHorizontal = 7;
    public float testAirMax = 7;

    [Header("Crouch Movement")]
    public float crouchAccel = 15;
    public float crouchMax = 6;
    public float crouchRotateSpeed = 120;
    public float longJumpUpForce = 5; // upwards force.
    public float longJumpForwardForce = 17; // forward force.
    public float highJumpForce = 12; // crouch jump upwards force.

    [Header("Grapple Movement")]
    public float maxSwingSpeed;
    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;
    public float launchSpeed;
    public float boost = 20;



    //To be done later
    //PlayerClass player;
    //Rigidbody rb;
    //Animator anim;

    //private void Awake()
    //{
    //    player = FindObjectOfType<PlayerClass>();
    //    rb = player.rb;
    //    anim = player.GetAnimator();
    //}
}
