using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTGIdleCalculationBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    private float DistanceCheckAI;

    public float LongRangeMax, LongRangeMin;
    public float MiddleRangeMax, MiddleRangeMin;
    public float ShortRangeMax, ShortRangeMin;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        animator.SetBool("HasExitedRange", false);
        animator.SetBool("IsInThrowRange", false);
        animator.SetBool("IsInChargeRange", false);
        animator.SetBool("IsInMeleeRange", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        
        DistanceCheckAI = Vector3.Distance(animator.transform.position, PlayerPosition.position);

        //---- Use Switch Statements ----\\

        if (DistanceCheckAI > LongRangeMax)
        {
            animator.SetBool("HasExitedRange", true);
        }

        if (DistanceCheckAI < LongRangeMax && DistanceCheckAI > LongRangeMin)
        {
            animator.SetBool("IsInThrowRange", true);
        }

        if (DistanceCheckAI < MiddleRangeMax && MiddleRangeMin > LongRangeMin)
        {
            animator.SetBool("IsInChargeRange", true);
        }

        if (DistanceCheckAI < ShortRangeMax && ShortRangeMin > LongRangeMin)
        {
            animator.SetBool("IsInMeleeRange", true);
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
