
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RayCast_IK : MonoBehaviour
{



    FullBodyBipedIK fBIK;
    public BipedIK playerIK;
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


    #region raycasts
    public bool isInCloseF()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + (player.transform.forward / 2) + new Vector3(0, 0.5f, 0);
        Debug.DrawLine(lineStart, vectorToSearch, Color.magenta);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
    }

    public bool isInCloseL()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + (player.transform.right / -3.5f + player.transform.forward / 3) + new Vector3(0, 0.5f, 0);
        Debug.DrawLine(lineStart, vectorToSearch, Color.green);
        return Physics.Linecast(lineStart, vectorToSearch, out lWall, p_LayerMask);
    }

    public bool isInCloseR()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + (player.transform.right / 4 + player.transform.forward / 3) + new Vector3(0, 0.5f, 0);
        Debug.DrawLine(lineStart, vectorToSearch, Color.blue);
        return Physics.Linecast(lineStart, vectorToSearch, out rWall, p_LayerMask);
    }


    #endregion




    void ReturnHand(IKSolverLimb limb)
    {
        if (!_IKOff)
            return;


        limb.SetIKPositionWeight(0);

    }

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

    IEnumerator StartGrapple(float weight)
    {
        while (playerIK.solvers.rightHand.GetIKPositionWeight() < weight)
        {
            float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);
            yield return null;
        }

    }

    public void IK_EndGrapple()
    {
        Debug.Log("Yo");
        rope.enabled = false;
        fBIK.solver.IKPositionWeight = 0;
        fBIK.solver.rightHandEffector.positionWeight = 0;
        playerIK.solvers.rightHand.SetIKPositionWeight(0);
        fBIK.solver.rightArmChain.pull = 0;


    }

    private void DisplayRope()
    {
        //This is not the actual width, but the width use so we can see the rope
        float ropeWidth = 0.2f;

        rope.startWidth = ropeWidth;
        rope.endWidth = ropeWidth;


        //Update the list with rope sections by approximating the rope with a bezier curve
        //A Bezier curve needs 4 control points
        Vector3 A = tetherPoint.position;
        Vector3 D = grapplePoint.position;

        //Upper control point
        //To get a little curve at the top than at the bottom
        Vector3 B = A + -tetherPoint.up * (-(A - D).magnitude * 0.1f);
        //B = A;

        //Lower control point
        Vector3 C = D + -grapplePoint.up * ((A - D).magnitude * 0.5f);

        //Get the positions
        //BezierCurve.GetBezierCurve(A, B, C, D, allRopeSections);


        //An array with all rope section positions
        Vector3[] positions = new Vector3[allRopeSections.Count];

        for (int i = 0; i < allRopeSections.Count; i++)
        {
            positions[i] = allRopeSections[i];
        }

        //Just add a line between the start and end position for testing purposes
        //Vector3[] positions = new Vector3[2];

        //positions[0] = whatTheRopeIsConnectedTo.position;
        //positions[1] = whatIsHangingFromTheRope.position;


        //Add the positions to the line renderer
        rope.positionCount = positions.Length;

        rope.SetPositions(positions);
    }


    // Update is called once per frame
    void Update()
    {
        //rb.velocity.magnitude > 2f &&

        if (pc.IsGrounded() && isInCloseF())
        {
            rightHandTarget.position = wall.point + player.transform.right / 5 + new Vector3(0, 0.3f, 0);
            float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);

            //playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);
            leftHandTarget.position = wall.point - new Vector3(0.2f, -0.3f, 0);
            // float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);

            //LEFT_HAND---NOT WORKING
            playerIK.solvers.leftHand.SetIKPositionWeight(0.6f);

            //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

        }

        if (pc.IsGrounded())
        {

            if (rb.velocity.magnitude < 0.001f)
            {
                _IKOff = true;
                if (isInCloseL())
                {

                    leftHandTarget.position = lWall.point + new Vector3(0, 0.25f, 0);
                    float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);

                    playerIK.solvers.leftHand.SetIKPositionWeight(llimbWeight);
                    if (playerIK.solvers.leftHand.GetIKPositionWeight() > 0.7)
                        ikVelocity = 0;
                    //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

                }


                if (isInCloseR())
                {
                    rightHandTarget.position = rWall.point + new Vector3(0, 0.25f, 0);
                    float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
                    playerIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);
                    if (playerIK.solvers.rightHand.GetIKPositionWeight() > 0.7)
                        ikVelocity = 0;

                    // playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);

                }

            }



            if (!isInCloseF() && !isInCloseR())
                ReturnHand(playerIK.solvers.rightHand);

            if (!isInCloseF() && !isInCloseL())
                ReturnHand(playerIK.solvers.leftHand);

            //if (!isInCloseF() )
            //{
            //    ReturnHands(playerIK.solvers.rightHand);
            //    ReturnHands(playerIK.solvers.leftHand);

            //}

        }


        if (pc.playerCurrentMove == MovementType.grapple)
        {

            rightHandTarget.position = pc.attachedGrapplePoint.position;
            rope.SetPosition(0, tetherPoint.position);
            rope.SetPosition(1, grapplePoint.position);
            rope.startWidth = 0.1f;
            rope.endWidth = 0.1f;

            // DisplayRope();


        }



        //if (pc.playerCurrentMove == MovementType.grapple)
        //{
        //    tetherPoint = pc.tetherPoint.transform.position;


        //    rightHandTarget.position = tetherPoint;
        //    float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);

        //    //BIPED_IK
        //    //playerIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);

        //    //FB__IK
        //    fBIK.solver.rightHandEffector.target = rightHandTarget;
        //    fBIK.solver.rightHandEffector.positionWeight = rlimbWeight; 

        //    // leftLegTarget.position = -player.transform.up - player.transform.right;
        //    //playerIK.solvers.leftFoot.target = leftLegTarget;
        //    //playerIK.solvers.leftFoot.IKPositionWeight = 0.8f;

        //    //rightLegTarget.position = -player.transform.up + player.transform.right;
        //    //playerIK.solvers.rightFoot.target = rightLegTarget;
        //    //playerIK.solvers.rightFoot.IKPositionWeight = 0.8f;
        //    //playerIK.solvers.spine.target = rightLegTarget;
        //    //playerIK.solvers.spine.IKPositionWeight = 0.8f;

        //}
        //else
        //{
        //    playerIK.solvers.leftFoot.IKPositionWeight = 0;

        //    playerIK.solvers.rightFoot.IKPositionWeight = 0;
        //}



    }



}