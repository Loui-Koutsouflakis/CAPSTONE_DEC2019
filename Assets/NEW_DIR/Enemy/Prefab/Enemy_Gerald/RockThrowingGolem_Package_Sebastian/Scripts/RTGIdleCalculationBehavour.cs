﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Created By Sebastian Borkowski
public class RTGIdleCalculationBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    private float DistanceCheckAI;


    private PlayerCamera cam;

    public float RotationSpeed;
    public float LongRangeMax, LongRangeMin;
    public float MiddleRangeMax, MiddleRangeMin;
    public float ShortRangeMax, ShortRangeMin;

    public float MaxHieghtDifference; // 3

    // Cliff and Wall checker
    public LayerMask Layer;
    public LayerMask GroundLayer;
    private float PlusY = 0.2f; // 0.2
    private float TopY = 4; // 4
    private float RightX = 1; // 1
    private float LeftX = 1; // 1
    private float FRL = 4; // 4
    private float SecondRayLength = 1; // 1
    private Vector3 EndPoint;
    private Vector3 AIMid;
    private Vector3 AITop;
    private Vector3 AILeft;
    private Vector3 AIRight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        cam = FindObjectOfType<PlayerCamera>();
        animator.SetBool("HasExitedRange", false);
        animator.SetBool("IsInThrowRange", false);
        animator.SetBool("IsInChargeRange", false);
        animator.SetBool("IsInMeleeRange", false);
        animator.SetBool("IsAboveAI", false);
        animator.SetBool("IsTired", false);
        animator.SetBool("Ground", false);

        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var atp = animator.transform.position;

        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        if (PlayerPosition == null)
        {
            PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Vector3 TargetRotation = new Vector3(PlayerPosition.position.x, atp.y, PlayerPosition.position.z);
        var r = Quaternion.LookRotation(TargetRotation - atp);
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, RotationSpeed * Time.deltaTime);

        // Cliff and Wall
        RaycastHit FH;
        // Ray Positions
        AIMid = new Vector3(atp.x, atp.y + PlusY, atp.z);
        AITop = new Vector3(atp.x, atp.y + TopY, atp.z);
        AILeft = new Vector3(atp.x + LeftX, atp.y + PlusY, atp.z);
        AIRight = new Vector3(atp.x - RightX, atp.y + PlusY, atp.z);
        // RayInfo
        var atTDVf = animator.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(AIMid, atTDVf, out FH, FRL, Layer) || Physics.Raycast(AILeft, atTDVf, out FH, FRL, Layer) || Physics.Raycast(AIRight, atTDVf, out FH, FRL, Layer) || Physics.Raycast(AITop, atTDVf, out FH, FRL, Layer))
        {
            animator.SetBool("ThereIsAWall", true);
        }
        else
        {
            animator.SetBool("ThereIsAWall", false);
        }
        Debug.DrawRay(AIMid, atTDVf * FH.distance, Color.red);
        Debug.DrawRay(AITop, atTDVf * FH.distance, Color.red);
        Debug.DrawRay(AIRight, atTDVf * FH.distance, Color.red);
        Debug.DrawRay(AILeft, atTDVf * FH.distance, Color.red);

        RaycastHit secondHit;
        EndPoint = AIMid + atTDVf * FRL;
        if (Physics.Raycast(EndPoint, Vector3.down, out secondHit, SecondRayLength, GroundLayer))
        {
            animator.SetBool("Ground", true);
        }
        else
        {
            animator.SetBool("Ground", false);
        }
        Debug.DrawRay(EndPoint, Vector3.down * secondHit.distance, Color.red);

        DistanceCheckAI = Vector3.Distance(atp, PlayerPosition.position);
        
        if (DistanceCheckAI < ShortRangeMax && DistanceCheckAI > ShortRangeMin)
        {
            animator.SetBool("IsInMeleeRange", true);
        }

        if (DistanceCheckAI < MiddleRangeMax && DistanceCheckAI > MiddleRangeMin)
        {
            if (PlayerPosition.position.y - atp.y >= MaxHieghtDifference)
            {
                animator.SetBool("IsInThrowRange", true);
            }
            else
            {
                animator.SetBool("IsInChargeRange", true);
            }
        }

        if (DistanceCheckAI < LongRangeMax && DistanceCheckAI > LongRangeMin && !cam.isCinema)
        {
            animator.SetBool("IsInThrowRange", true);
        }

        if (DistanceCheckAI > LongRangeMax)
        {
            animator.SetBool("HasExitedRange", true);
            animator.SetBool("IsInMeleeRange", false);
            animator.SetBool("IsInChargeRange", false);
            animator.SetBool("IsInThrowRange", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
