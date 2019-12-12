using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Created By Sebastian Borkowski
public class RockThrowingGolemIdleBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    

    public float WakeUpRadius;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        animator.SetBool("HasArrivedAtNewSleepPos", false);
        animator.SetBool("isInWakeUpRange", false);
        animator.SetBool("HasExitedRange", false);
        animator.SetBool("HasNotExitedRange", false);
        animator.SetBool("IsInMeleeRange", false);
        animator.SetBool("IsStuck", false);
        animator.SetBool("IsInChargeRange", false);
        animator.SetBool("IsTired", false);
        animator.SetBool("IsInThrowRange", false);
        animator.SetBool("StartNewSleepPosFunction", false);

    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (Vector3.Distance(animator.transform.position, PlayerPosition.position) < WakeUpRadius)
        {
            animator.SetBool("isInWakeUpRange", true);
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
