using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTGFindNewSleepPosBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    public float WakeUpRadius;
 
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isInWakeUpRange", false);
        animator.SetBool("HasNotExitedRange", false);
        animator.SetBool("StartNewSleepPosFunction", true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("StartNewSleepPosFunction", false);
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        if (Vector3.Distance(animator.transform.position, PlayerPosition.position) < WakeUpRadius)
        {
            animator.SetBool("isInWakeUpRange", true);
            
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("StartNewSleepPosFunction", false);
    }

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
