﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is a handy reference 

public class PlayerVariables : MonoBehaviour
{
//Change Variables in script between test plays to adjust values.
    [Header("Ground Movement")]
    public float walkAccel = 60;
    public float climbAccel = 12;
    public float walkMax = 5;
    public float rotateSpeed = 120;
    public float jumpForce = 5.5f;


    [Header("Air Movement")]
    public float jumpMultiplier = 3f;
    public float fallMultiplier = 3f;
    public float fallTime = 2.8f;
    public float peakTime = 0.3f;
    public float peakHeightMultiplier = 0.8f;
    public float doubleJumpForce = 7;
    public float wallMultiplier = 0.5f;
    public float airForwardSpeed = 10f;
    public float airSideSpeed = 5f;
    public float wallJumpVertical = 7;
    public float wallJumpHorizontal = 7;
    public float testAirMax = 3.5f;
    public float longJumpMax = 50;
    public float highJumpAirMax = 1;

    [Header("Crouch Movement")]
    public float crouchAccel = 15;
    public float crouchMax = 6;
    public float crouchRotateSpeed = 120;
    public float longJumpUpForce = 4; // upwards force.
    public float longJumpForwardForce = 12; // forward force.
    public float highJumpForce = 8.5f; // crouch jump upwards force.

    [Header("Grapple Movement")]
    public float maxSwingSpeed;
    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;
    public float launchSpeed;
    public float boost = 20;

    protected float EaseIn(float t)
    {
        return t * t;
    }

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



    //************************************************************Settled upon variables*********************************************************************
    //[Header("Ground Movement")]
    //public float walkAccel = 60;
    //public float climbAccel = 12;
    //public float walkMax = 5;
    //public float rotateSpeed = 120;
    //public float jumpForce = 5.5f;


    //[Header("Air Movement")]
    //public float jumpMultiplier = 3f;
    //public float fallMultiplier = 3f;
    //public float fallTime = 2.8f;
    //public float peakTime = 0.3f;
    //public float peakHeightMultiplier = 0.8f;
    //public float doubleJumpForce = 7;
    //public float wallMultiplier = 0.5f;
    //public float airForwardSpeed = 10f;
    //public float airSideSpeed = 5f;
    //public float wallJumpVertical = 7;
    //public float wallJumpHorizontal = 7;
    //public float testAirMax = 3.5f;
    //public float longJumpMax = 20;
    //public float highJumpAirMax = 1;

    //[Header("Crouch Movement")]
    //public float crouchAccel = 15;
    //public float crouchMax = 6;
    //public float crouchRotateSpeed = 120;
    //public float longJumpUpForce = 5; // upwards force.
    //public float longJumpForwardForce = 12; // forward force.
    //public float highJumpForce = 8.5f; // crouch jump upwards force.

    //[Header("Grapple Movement")]
    //public float maxSwingSpeed;
    //public float forwardSpeed = 100;
    //public float sidewaysSpeed = 8;
    //public float jumpSpeed = 10;
    //public float launchSpeed;
    //public float boost = 20;
}
