using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Created By Sebastian Borkowski
public class RTGIdleCalculationBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    public float DistanceCheckAI;

    public float RotationSpeed;
    public float LongRangeMax, LongRangeMin;
    public float MiddleRangeMax, MiddleRangeMin;
    public float ShortRangeMax, ShortRangeMin;

    // Cliff and Wall checker
    public LayerMask Layer;
    public LayerMask GroundLayer;
    public float PlusY; // 1
    public float FirstRayLength; // 5
    public float SecondRayLength; // 2
    public Vector3 EndPoint;
    public Vector3 AI;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HasExitedRange", false);
        animator.SetBool("IsInThrowRange", false);
        animator.SetBool("IsInChargeRange", false);
        animator.SetBool("IsInMeleeRange", false);
        animator.SetBool("IsTired", false);
        animator.SetBool("Ground", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 TargetRotation = new Vector3(PlayerPosition.position.x, animator.transform.position.y, PlayerPosition.position.z);
        var r = Quaternion.LookRotation(TargetRotation - animator.transform.position);
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, RotationSpeed * Time.deltaTime);

        // Cliff and Wall
        RaycastHit firstHit;
        AI = new Vector3(animator.transform.position.x, animator.transform.position.y + PlusY, animator.transform.position.z);
        if (Physics.Raycast(AI, animator.transform.TransformDirection(Vector3.forward), out firstHit, FirstRayLength, Layer))
        {
            animator.SetBool("ThereIsAWall", true);
            Debug.Log(firstHit.collider);
        }
        else
        {
            animator.SetBool("ThereIsAWall", false);
        }
        Debug.DrawRay(AI, animator.transform.TransformDirection(Vector3.forward) * firstHit.distance, Color.red);

        RaycastHit secondHit;
        EndPoint = AI + animator.transform.TransformDirection(Vector3.forward) * FirstRayLength;
        if (Physics.Raycast(EndPoint, Vector3.down, out secondHit, SecondRayLength, GroundLayer))
        {
            animator.SetBool("Ground", true);
        }
        else
        {
            animator.SetBool("Ground", false);
        }
        Debug.DrawRay(EndPoint, Vector3.down * secondHit.distance, Color.red);

        DistanceCheckAI = Vector3.Distance(animator.transform.position, PlayerPosition.position);
        
        if (DistanceCheckAI < ShortRangeMax && DistanceCheckAI > ShortRangeMin)
        {
            animator.SetBool("IsInMeleeRange", true);
        }

        if (DistanceCheckAI < MiddleRangeMax && DistanceCheckAI > MiddleRangeMin)
        {
            animator.SetBool("IsInChargeRange", true);
        }

        if (DistanceCheckAI < LongRangeMax && DistanceCheckAI > LongRangeMin)
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
