
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RayCast_IK : MonoBehaviour
{



    FullBodyBipedIK fBIK;
    public BipedIK playerIK;


    //Rotation of Target Transform should be: 

    //RHand x: 0, y: 150, z:320
    //LHand x: -10, y:90, Z:-140

    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Transform leftLegTarget;
    public Transform rightLegTarget;

    public Transform grapplePoint;


    public float bipediIKWeigh = 0.2f;
    public float fBIKWeigh = 0.1f;


    LineRenderer rope;

    public List<Vector3> allRopeSections = new List<Vector3>();


    public GameObject player;
    public float smoothTime;
    public float ikVelocity;

    Rigidbody rb;
    PlayerClass pc;

    bool _IKOff = false;

    Transform tetherPoint;

    bool touchingWall = false;

    RaycastHit wall;
    RaycastHit lWall;
    RaycastHit rWall;
    LayerMask p_LayerMask = 1 << 9;
    public float wallDist;
    //Strings to change limb direction == "rHand", "lHand", "rFoot", "lFoot"



    // Start is called before the first frame update
    void Start()
    {
        fBIK = GetComponent<FullBodyBipedIK>();
        fBIK.solver.IKPositionWeight = 0;

        smoothTime = 5f;
        ikVelocity = 0;
        rb = GetComponentInParent<Rigidbody>();
        pc = GetComponentInParent<PlayerClass>();
        rope = pc.debugLine;

        playerIK.solvers.rightHand.target = rightHandTarget;
        playerIK.solvers.leftHand.target = leftHandTarget;
        p_LayerMask = ~p_LayerMask;
    }


    //These are the raycasts that determine if the player is near a wall

    #region raycasts
    public bool isInCloseF()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + (player.transform.forward / 2) + new Vector3(0, 0.5f, 0);
        Debug.DrawLine(lineStart, vectorToSearch, Color.magenta);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
    }

    Quaternion ReturnRotation(RaycastHit hit)
    {
        Quaternion rotation = Quaternion.LookRotation(transform.position, hit.point);
        return rotation;
    }

    public bool isInCloseL()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + (player.transform.right / -2f + player.transform.forward / 4) + new Vector3(0, 0.5f, 0);
        Debug.DrawLine(lineStart, vectorToSearch, Color.green);
        return Physics.Linecast(lineStart, vectorToSearch, out lWall, p_LayerMask);
    }

    public bool isInCloseR()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + (player.transform.right / 2 + player.transform.forward / 4) + new Vector3(0, 0.5f, 0);
        Debug.DrawLine(lineStart, vectorToSearch, Color.blue);
        return Physics.Linecast(lineStart, vectorToSearch, out rWall, p_LayerMask);
    }


    #endregion



    //simple function to turn off the individual IK limb
    void ReturnHand(IKSolverLimb limb)
    {
        if (!_IKOff)
            return;


        limb.SetIKPositionWeight(0);
        limb.SetIKRotationWeight(0);

    }

    //if you want both turned off
    public void ReturnBothHands()
    {
        ReturnHand(playerIK.solvers.rightHand);
        ReturnHand(playerIK.solvers.leftHand);

    }



    //GRAPPLE

    public void IK_Grapple()
    {
        tetherPoint = pc.attachedGrapplePoint;
        rightHandTarget.position = tetherPoint.position;



        fBIK.solver.IKPositionWeight = 0.5f;
        fBIK.solver.rightHandEffector.target = rightHandTarget;
        fBIK.solver.rightHandEffector.positionWeight = fBIKWeigh;
        fBIK.solver.rightArmChain.bendConstraint.direction = rightHandTarget.position;
        fBIK.solver.rightArmChain.pull = 0.5f;





        rope.enabled = true;


    }



    public void IK_EndGrapple()
    {
        rope.enabled = false;
        playerIK.solvers.rightHand.SetIKPositionWeight(0);
        fBIK.solver.rightArmChain.pull = 0;
        StartCoroutine(EndGrapCoroutine(fBIK.solver.IKPositionWeight));



    }

    IEnumerator EndGrapCoroutine(float weight)
    {
        while (fBIK.solver.IKPositionWeight > 0)
        {
            if (pc.playerCurrentMove == MovementType.grapple)
            {
                IK_Grapple();
                yield break;
            }


            weight -= 0.01f;
            fBIK.solver.IKPositionWeight = weight;
            fBIK.solver.rightHandEffector.positionWeight = weight;
            yield return null;
        }

    }


    // Update is called once per frame
    void Update()
    {
        //rb.velocity.magnitude > 2f &&

        if (pc.IsGrounded())
        {
            fBIK.solver.IKPositionWeight = 0;

            if (isInCloseF() && rb.velocity.magnitude > 2f)
            {
                rightHandTarget.position = wall.point + player.transform.right / 5 + new Vector3(0, 0.3f, 0);
                float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
                playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);
                playerIK.solvers.rightHand.SetIKRotationWeight(0.5f);


                leftHandTarget.position = wall.point - player.transform.right / 5 - new Vector3(0, -0.3f, 0);
                //float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
                playerIK.solvers.leftHand.SetIKRotationWeight(0.5f);

                playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);


            }



            if (rb.velocity.magnitude < 0.001f)
            {
                _IKOff = true;
                if (!isInCloseF() && isInCloseL())
                {

                    leftHandTarget.position = lWall.point + new Vector3(0, 0.25f, 0);

                    //leftHandTarget.rotation = ReturnRotation(lWall);

                    float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);

                    playerIK.solvers.leftHand.SetIKPositionWeight(llimbWeight);
                    playerIK.solvers.leftHand.SetIKRotationWeight(1);
                    if (playerIK.solvers.leftHand.GetIKPositionWeight() > 0.7)
                        ikVelocity = 0;
                    //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

                }


                if (!isInCloseF() && isInCloseR())
                {
                    rightHandTarget.position = rWall.point + new Vector3(0, 0.25f, 0);
                    float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
                    playerIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);
                    playerIK.solvers.rightHand.SetIKRotationWeight(1);

                    if (playerIK.solvers.rightHand.GetIKPositionWeight() > 0.7)
                        ikVelocity = 0;

                    // playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);

                }
               
            }




        }

        else
        {

            if (pc.playerCurrentMove == MovementType.grapple)
            {

                rightHandTarget.position = pc.attachedGrapplePoint.position;
                rope.SetPosition(0, tetherPoint.position);
                rope.SetPosition(1, grapplePoint.position);
                rope.startWidth = 0.1f;
                rope.endWidth = 0.1f;


                fBIK.solver.IKPositionWeight = 0.5f;
                fBIK.solver.rightHandEffector.target = rightHandTarget;
                fBIK.solver.rightHandEffector.positionWeight = fBIKWeigh;
                fBIK.solver.rightArmChain.bendConstraint.direction = rightHandTarget.position;
                fBIK.solver.rightArmChain.pull = 0.5f;



            }
            else if (fBIK.solver.IKPositionWeight > 0)
            {
                //fBIK.solver.IKPositionWeight = 0; 
            }
        }


        if (!isInCloseF())
        {
            if (!isInCloseR())
                ReturnHand(playerIK.solvers.rightHand);

            if (!isInCloseL())
                ReturnHand(playerIK.solvers.leftHand);

        }





    }



}