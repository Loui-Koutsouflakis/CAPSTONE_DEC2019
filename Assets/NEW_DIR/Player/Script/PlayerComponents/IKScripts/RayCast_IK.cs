//to turn off grappling, go to line 197 of player controller

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RayCast_IK : MonoBehaviour
{
    [Header("Grapple IK On/Off")]
    public bool toggleGrapple = true;


    [Header("Read only attached grapple point")]
   public Transform grapplePoint;

    FullBodyBipedIK fBIK;
    BipedIK bPIK;

    [Header("Grapple Hook Animation Curve---Found under Lumi's right hand")]
    public GrappleAnimationCurve grapAnimCurve;

    [Header("Rope Width")]
    public float ropeWidth = 0.05f; 
    //Rotation of Target Transform should be: 

    //RHand x: 0, y: 150, z:320
    //LHand x: -10, y:90, Z:-140

    [Header("IK Targets --- found IKTargets in Lumi prefab")]
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Transform leftLegTarget;
    public Transform rightLegTarget;

   

    [Header ("IK weights")]
    public float bipediIKWeigh = 0.2f;
    public float fBIKWeigh = 0.1f;
    public float fBIKWeighEase = 0.5f;



    LineRenderer rope;


    [Header ("Lumi_IKRigged goes in here")]
    public GameObject player;

    [Header ("Rope Lerp values")]
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
    public LayerMask p_LayerMask;

  
     float wallDist;
    //Strings to change limb direction == "rHand", "lHand", "rFoot", "lFoot"



    // Start is called before the first frame update
    void Start()
    {
        if (grapAnimCurve != null)
        grapAnimCurve.hook.SetActive(false);
        

        fBIK = GetComponent<FullBodyBipedIK>();
        fBIK.solver.IKPositionWeight = 0;

        bPIK = GetComponent<BipedIK>();
        smoothTime = 5f;
        ikVelocity = 0;
        rb = GetComponentInParent<Rigidbody>();
        pc = GetComponentInParent<PlayerClass>();
        rope = pc.debugLine;

        bPIK.solvers.rightHand.target = rightHandTarget;
        bPIK.solvers.leftHand.target = leftHandTarget;
       
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
        ReturnHand(bPIK.solvers.rightHand);
        ReturnHand(bPIK.solvers.leftHand);

    }



    //GRAPPLE

    public void IK_Grapple()
    {
        tetherPoint = pc.tetherPoint.GetTetherLocation();
        rightHandTarget.position = tetherPoint.position;

        StartCoroutine(StartGrapCoroutine(0, toggleGrapple));

      

    }


    public void EnableRope()
    {
        rope.enabled = true;

        if (grapAnimCurve != null)
        { 

        grapAnimCurve.throwPosition = grapplePoint;

            grapAnimCurve.SetHookPoint(rightHandTarget);


            grapAnimCurve.DetatchHook(true);


        grapAnimCurve.hook.SetActive(true);

       // grapAnimCurve.hook.transform.rotation = grapplePoint.transform.rotation;

         grapAnimCurve.AnimateHook();

        }
        rope.SetPosition(0, grapAnimCurve.hook.transform.position);
       
        rope.SetPosition(1, grapplePoint.position);

        Debug.Log("Rope Enabled");
    }


    public void IK_EndGrapple()
    {
        rope.enabled = false;
        if (grapAnimCurve != null)
        { 
        grapAnimCurve.hook.SetActive(false);
        grapAnimCurve.DetatchHook(false);

        }

        bPIK.solvers.rightHand.SetIKPositionWeight(0);
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

    IEnumerator StartGrapCoroutine(float weight, bool gt)
    {
        if (gt == false)
            yield break; 


        while (fBIK.solver.IKPositionWeight < fBIKWeighEase)
        {
            if (pc.playerCurrentMove != MovementType.grapple)
            {
                yield break;
            }


            weight += 0.00000001f;
            fBIK.solver.IKPositionWeight = weight;
            fBIK.solver.rightHandEffector.positionWeight = weight;
            yield return null;
        }


        GrappleUpdate(toggleGrapple);

    }

    //this is an easy way of turning on and off the IKGRappling system
    void GrappleUpdate(bool gt)
    {
        if (gt == false)
            return;

        fBIK.solver.IKPositionWeight = 0.2f;
        fBIK.solver.rightHandEffector.target = rightHandTarget;
        fBIK.solver.rightHandEffector.positionWeight = fBIKWeigh;
        fBIK.solver.rightArmChain.bendConstraint.direction = rightHandTarget.position;
        fBIK.solver.rightArmChain.pull = 0.5f;

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
                float rlimbWeight = Mathf.SmoothDamp(bPIK.solvers.rightHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
                bPIK.solvers.rightHand.SetIKPositionWeight(0.8f);
                bPIK.solvers.rightHand.SetIKRotationWeight(0.5f);


                leftHandTarget.position = wall.point - player.transform.right / 5 - new Vector3(0, -0.3f, 0);
                //float llimbWeight = Mathf.SmoothDamp(playerIK.solvers.leftHand.GetIKPositionWeight(), 0.9f, ref ikVelocity, smoothTime * Time.deltaTime);
                bPIK.solvers.leftHand.SetIKRotationWeight(0.5f);

                bPIK.solvers.leftHand.SetIKPositionWeight(0.8f);


            }



            if (rb.velocity.magnitude < 0.001f)
            {
                _IKOff = true;
                if (!isInCloseF() && isInCloseL())
                {

                    leftHandTarget.position = lWall.point + new Vector3(0, 0.25f, 0);

                    //leftHandTarget.rotation = ReturnRotation(lWall);

                    float llimbWeight = Mathf.SmoothDamp(bPIK.solvers.leftHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);

                    bPIK.solvers.leftHand.SetIKPositionWeight(llimbWeight);
                    bPIK.solvers.leftHand.SetIKRotationWeight(1);
                    if (bPIK.solvers.leftHand.GetIKPositionWeight() > 0.7)
                        ikVelocity = 0;
                    //playerIK.solvers.leftHand.SetIKPositionWeight(0.8f);

                }


                if (!isInCloseF() && isInCloseR())
                {
                    rightHandTarget.position = rWall.point + new Vector3(0, 0.25f, 0);
                    float rlimbWeight = Mathf.SmoothDamp(bPIK.solvers.rightHand.GetIKPositionWeight(), 0.8f, ref ikVelocity, smoothTime * Time.deltaTime);
                    bPIK.solvers.rightHand.SetIKPositionWeight(rlimbWeight);
                    bPIK.solvers.rightHand.SetIKRotationWeight(1);

                    if (bPIK.solvers.rightHand.GetIKPositionWeight() > 0.7)
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


                if (rope.enabled)
                {
                    rope.SetPosition(0, grapAnimCurve.hook.transform.position);
                    rope.SetPosition(1, grapplePoint.position);
                    rope.startWidth = ropeWidth;
                    rope.endWidth = ropeWidth;

                    Vector3 ropeVec = rope.GetPosition(0) - rope.GetPosition(1);
                    float ropeDistance = ropeVec.magnitude;

                    rope.material.SetTextureScale("_MainTex", new Vector2(ropeDistance, ropeWidth));

                }


                GrappleUpdate(toggleGrapple); 



            }
            else if (fBIK.solver.IKPositionWeight > 0)
            {
                //fBIK.solver.IKPositionWeight = 0; 
            }
        }


        if (!isInCloseF())
        {
            if (!isInCloseR())
                ReturnHand(bPIK.solvers.rightHand);

            if (!isInCloseL())
                ReturnHand(bPIK.solvers.leftHand);

        }





    }



}