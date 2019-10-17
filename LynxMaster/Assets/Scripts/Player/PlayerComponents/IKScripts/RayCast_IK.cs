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


    public GameObject player;
    public float smoothTime;
    public float ikVelocity;

    Rigidbody rb;
    PlayerClass pc;


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
        smoothTime = 5f;
        ikVelocity = 0;
        rb = GetComponentInParent<Rigidbody>();
        pc = GetComponentInParent<PlayerClass>();

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

    IEnumerator ReachOutRightLimbCoroutine(Transform position, float weight)
    {
        while (playerIK.solvers.rightHand.GetIKPositionWeight() <= (weight - 0.4f))
        {
            float limbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), weight, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.rightHand.target = position;
            playerIK.solvers.rightHand.SetIKPositionWeight(limbWeight);
            yield return null;
        }

    }



    void ReturnHand(IKSolverLimb limb)
    {
        limb.SetIKPositionWeight(0);

    }

    public void ReturnBothHands()
    {
        Debug.Log("Hey");
        ReturnHand(playerIK.solvers.rightHand);
        ReturnHand(playerIK.solvers.leftHand);

    }

    // Update is called once per frame
    void Update()
    {

        if (isInCloseF() && rb.velocity.magnitude > 2f)
        {
            rightHandTarget.position = wall.point + new Vector3(0.2f, 0.3f, 0);
            //float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);

            //playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);
            leftHandTarget.position = wall.point - new Vector3(0.2f, -0.3f, 0);
            // float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.leftHand.SetIKPositionWeight(0.85f);

            //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

        }


        if (rb.velocity.magnitude < 0.001f && pc.playerCurrentMove == MovementType.move)
        {

            //if (isInCloseL())
            //{

            //    leftHandTarget.position = lWall.point + new Vector3(0, 0.25f, 0);
            //    float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);

            //    playerIK.solvers.leftHand.SetIKPositionWeight(llimbWeight);
            //    if (playerIK.solvers.leftHand.GetIKPositionWeight() > 0.7)
            //        ikVelocity = 0;
            //    //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

            //}


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


        if(pc.playerCurrentMove == MovementType.grapple)
        {
            leftLegTarget.position = -player.transform.up - player.transform.right;
            fBIK.solver.leftFootEffector.target = leftLegTarget;
            fBIK.solver.leftFootEffector.positionWeight = 1; 

           rightLegTarget.position = -player.transform.up + player.transform.right;
            fBIK.solver.rightFootEffector.target = rightLegTarget;
            fBIK.solver.rightFootEffector.positionWeight = 1;


        }



    }



}


