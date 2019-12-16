using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RockThrowingGolemChargeBehavour : StateMachineBehaviour
{
    private Transform PlayerPosition;
    private Vector3 AttackTarget;
    private Vector3 AT;
    public int ChargeSpeed;
    public int RotationSpeed;

    // Cliff and Wall checker
    public LayerMask Layer;
    public LayerMask GroundLayer;
    private float PlusY = 0.2f; // 0.2
    private float TopY = 4; // 4
    private float RightX = 1; // 1
    private float LeftX = 1; // 1
    private float FRL = 4; // 4  // Far Range Length
    private float SRL = 1; // 1  // Short Range Length
    private float MaxHieghtDifference = 3; // 3
    private Vector3 EndPoint;
    private Vector3 AIMid;
    private Vector3 AITop;
    private Vector3 AILeft;
    private Vector3 AIRight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        AttackTarget = new Vector3(PlayerPosition.position.x, animator.transform.position.y, PlayerPosition.position.z);
        AT = AttackTarget - animator.transform.position;

        if(PlayerPosition.position.y - animator.transform.position.y >= MaxHieghtDifference)
        {
            animator.SetBool("IsAboveAI", true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerPosition == null)
        {
            PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        }

        var atp = animator.transform.position;
        var r = Quaternion.LookRotation(AT, Vector3.up); //AttackTarget - atp
        animator.transform.position = Vector3.MoveTowards(atp, AttackTarget, ChargeSpeed * Time.deltaTime);
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
        Debug.DrawRay(AIMid, atTDVf * FH.distance, Color.red);
        Debug.DrawRay(AITop, atTDVf * FH.distance, Color.red);
        Debug.DrawRay(AIRight, atTDVf * FH.distance, Color.red);
        Debug.DrawRay(AILeft, atTDVf * FH.distance, Color.red);

        RaycastHit secondHit;
        EndPoint = AIMid + atTDVf * FRL;
        if (Physics.Raycast(EndPoint, Vector3.down, out secondHit, SRL, GroundLayer))
        {
            animator.SetBool("Ground", true);
        }
        else
        {
            animator.SetBool("ThereIsAWall", true);
            animator.SetBool("Ground", false);
        }
        Debug.DrawRay(EndPoint, Vector3.down * secondHit.distance, Color.red);

        if (Vector3.Distance(atp, AttackTarget) < 2)
        {
            animator.SetBool("IsTired", true);
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
