using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Created By Sebastian Borkowski
public class RTGIdleCalculationBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    public float DistanceCheckAI;

    public float speed;
    public float LongRangeMax, LongRangeMin;
    public float MiddleRangeMax, MiddleRangeMin;
    public float ShortRangeMax, ShortRangeMin;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HasExitedRange", false);
        animator.SetBool("IsInThrowRange", false);
        animator.SetBool("IsInChargeRange", false);
        animator.SetBool("IsInMeleeRange", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        
        var r = Quaternion.LookRotation(PlayerPosition.position - animator.transform.position);
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, speed * Time.deltaTime);

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
