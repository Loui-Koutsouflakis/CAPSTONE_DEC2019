using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK; 

public class RayCast_IK : MonoBehaviour
{

    public BipedIK playerIK;
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public GameObject player;
    public float smoothTime = 0.0001f;
    public float ikVelocity = 5;



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
        playerIK.solvers.rightHand.target = rightHandTarget;
        playerIK.solvers.leftHand.target = leftHandTarget;
        p_LayerMask = ~p_LayerMask;
    }

    //raycasts

    #region
    public bool isInCloseF()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + player.transform.forward + new Vector3(0, 0.5f, -0.6f);
        Debug.DrawLine(lineStart, vectorToSearch, Color.magenta);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
    }

    public bool isInCloseL()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + new Vector3(0.6f, 0.5f, 0) - player.transform.right;
        Debug.DrawLine(lineStart, vectorToSearch, Color.green);
        return Physics.Linecast(lineStart, vectorToSearch, out lWall, p_LayerMask);
    }

    public bool isInCloseR()
    {
        Vector3 lineStart = player.transform.position + new Vector3(0, 0.5f, 0);
        Vector3 vectorToSearch = player.transform.position + player.transform.right + new Vector3(-0.6f, 0.5f, 0);
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


    IEnumerator ReachOutLeftLimbCoroutine(Transform position, float weight)
    {
        while (playerIK.solvers.leftHand.GetIKPositionWeight() <= (weight - 0.4f))
        {
            float limbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), weight, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.leftHand.target = position;
            playerIK.solvers.leftHand.SetIKPositionWeight(limbWeight);
            yield return null;
        }

    }

    void ReachOutLimb(IKSolverLimb limb, Transform position, float weight)
    {
        limb.target = position;
        limb.SetIKPositionWeight(weight);

    }


    void PutOutHands(Vector3 wallHit, float weight)
    {
        rightHandTarget.position = wallHit + new Vector3(0.2f, 0.25f, 0);
        StartCoroutine(ReachOutRightLimbCoroutine(rightHandTarget, 1));
        leftHandTarget.position = wallHit - new Vector3(0.2f, -0.25f, 0);
        StartCoroutine(ReachOutLeftLimbCoroutine(leftHandTarget, 1));

    }

    void PutOutAHand(IKSolverLimb limb, Vector3 wallHit, float weight)
    {
        if (limb == (playerIK.solvers.rightHand))
        {

            rightHandTarget.position = wallHit + new Vector3(0, 0.25f, 0);
            StartCoroutine(ReachOutRightLimbCoroutine(rightHandTarget, 1));
            // ReachOutLimb(playerIK.solvers.rightHand, rightHandTarget, weight);
        }
        else
        {
            leftHandTarget.position = wallHit + new Vector3(0, 0.25f, 0);
            StartCoroutine(ReachOutLeftLimbCoroutine(leftHandTarget, 1));
            //ReachOutLimb(playerIK.solvers.leftHand, leftHandTarget, weight);

        }
    }

    void ReturnHands(IKSolverLimb limb)
    {

        limb.SetIKPositionWeight(0);

    }


    // Update is called once per frame
    void Update()
    {
        if (isInCloseF())
        {
            rightHandTarget.position = wall.point + new Vector3(0.2f, 0.25f, 0);
            float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);

            //playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);
            leftHandTarget.position = wall.point - new Vector3(0.2f, -0.25f, 0);
            float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.leftHand.SetIKPositionWeight(llimbWeight);

            //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);




        }

        if (isInCloseL())
        {

            leftHandTarget.position = lWall.point + new Vector3(0, 0.25f, 0);
            float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.leftHand.SetIKPositionWeight(llimbWeight);
            //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

        }


        if (isInCloseR())
        {
            rightHandTarget.position = rWall.point + new Vector3(0, 0.25f, 0);
            float rlimbWeight = Mathf.SmoothDamp(playerIK.solvers.rightHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
            playerIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);

            // playerIK.solvers.rightHand.SetIKPositionWeight(0.8f);

        }


        if (!isInCloseF() && !isInCloseR())
            ReturnHands(playerIK.solvers.rightHand);


        if (!isInCloseF() && !isInCloseL())
            ReturnHands(playerIK.solvers.leftHand);

        //if (!isInCloseF() )
        //{
        //    ReturnHands(playerIK.solvers.rightHand);
        //    ReturnHands(playerIK.solvers.leftHand);

        //}




    }



}
