﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Created By Sebastian Borkowski
public class RockThrowingGolemThrowBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
   
    public float RotationSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 TargetRotation = new Vector3(PlayerPosition.position.x, animator.transform.position.y, PlayerPosition.position.z);
        var r = Quaternion.LookRotation(TargetRotation - animator.transform.position);
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, RotationSpeed * Time.deltaTime);
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
