using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RockThrowingGolemChargeBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    public Vector3 AttackTarget;
    public int ChargeSpeed;
    public int RotationSpeed;

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
        Layer = LayerMask.GetMask("Environment");
        GroundLayer = LayerMask.GetMask("Environment");
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        AttackTarget = new Vector3(PlayerPosition.position.x, animator.transform.position.y, PlayerPosition.position.z);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Layer = ~Layer; // collide with everything ecept the selected layer.
        
        var r = Quaternion.LookRotation(AttackTarget - animator.transform.position);
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, RotationSpeed * Time.deltaTime);
        //animator.transform.position = Vector3.MoveTowards(animator.transform.position, AttackTarget, ChargeSpeed * Time.deltaTime);

        if (Vector3.Distance(animator.transform.position, AttackTarget) < 3)
        {
            animator.SetBool("IsTired", true);
        }

        // Cliff and Wall
        RaycastHit firstHit;
        AI = new Vector3(animator.transform.position.x, animator.transform.position.y + PlusY, animator.transform.position.z);
        if (Physics.Raycast(AI, animator.transform.TransformDirection(Vector3.forward), out firstHit, FirstRayLength, Layer))
        {
            animator.SetBool("ThereIsAWall", true);
            Debug.Log(firstHit.collider);

        }
        Debug.DrawRay(AI, animator.transform.TransformDirection(Vector3.forward) * firstHit.distance, Color.red);

        RaycastHit secondHit;
        EndPoint = AI + animator.transform.TransformDirection(Vector3.forward) * FirstRayLength;
        if (Physics.Raycast(EndPoint, Vector3.down, out secondHit, SecondRayLength, GroundLayer))
        {
            //Debug.Log("ground");
        }
        else
        {
            animator.SetBool("ThereIsWall", true);
            //Debug.Log("AIR");
        }
        Debug.DrawRay(EndPoint, Vector3.down * secondHit.distance, Color.red);
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
