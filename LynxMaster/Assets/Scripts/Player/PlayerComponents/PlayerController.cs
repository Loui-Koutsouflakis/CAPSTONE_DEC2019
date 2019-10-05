//created by Luke Fentress - controller script to handle player movement types
//created by Luke Fentress - controller script to handle player movement types
//edited by AT - 19-08-09 - added air controller functionallity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PlayerClass player;

    [Header("STATE MACHINE NOT ANIMATION")]
    [SerializeField]
    Animator stateMachine;

    public ParticleSystem psJump;
    public ParticleSystem psRun;
    public bool jumpParticleIsPlaying;

    //annoying temporary check until we slay the beast that is the grappleComponent tether bool
    bool isTethered = false;
    bool isCrouching = false;
    public bool GetIsCrouching()
    {
        return isCrouching;
    }

    private void Awake()
    {
        player.InitializePlayer();
    }

    private void Start()
    {
        StartCoroutine(CheckGround());
        StartCoroutine(CheckSphere());
        StartCoroutine(FallCheck());


        //debugging
        player.debugLine.GetComponent<LineRenderer>().enabled = false;
    }
     

    public void Jump()
    {
        //player.GetMoveComponent().Jump();
        //currently using if statement for this
        //however should set up a "currentMovementComponet" then just call player.currentMovementComponent.Jump()

        if (player.playerCurrentMove == MovementType.move)
        {
            player.GetMoveComponent().Jump();
            //set movement type to air
            player.SetMovementType(MovementType.air);
            //sets grounded to false outside of
            player.SetGrounded(false);


            stateMachine.SetTrigger("JumpTrigger");
            //Particle stuff from Tony
            psRun.Stop();
            psJump.Stop();
            psJump.Play();
            jumpParticleIsPlaying = false;
        }
        else if (player.playerCurrentMove == MovementType.air)
        {
            player.GetAirComponent().Jump();
        }
        else if (player.playerCurrentMove == MovementType.crouch)
        {
            player.GetCrouchComponent().Jump();
            player.SetMovementType(MovementType.air);
            player.SetGrounded(false);
            //Particle stuff from Tony
            psRun.Stop();
            psJump.Stop();
            psJump.Play();
        }
        else if (player.playerCurrentMove == MovementType.grapple)
        {
            DetatchGrapple();
        }

    }

    public void Grapple()
    {
        if (player.isGrappling || player.IsGrounded() || player.tetherPoint == null)
        {
            Debug.Log("Cant grapple");
            return;
        }
        player.isGrappling = true;
        stateMachine.SetTrigger("GrappleTrigger");
        player.SetMovementType(MovementType.grapple);
        player.GetGrappleComponent().Grapple();
    }

    public void DetatchGrapple()
    {
        //player.isGrappling = false; moved to grapplecomponent

        player.GetGrappleComponent().DetatchGrapple();
        stateMachine.SetTrigger("FallTrigger");
        player.SetMovementType(MovementType.air);


        //this would be the fall state, but since we don't yet have an air controller
        //player.SetMovementType("move");
    }

    //man this is alot of jumping back and forth to do something simple lol
    public void Crouch()
    {
        if (player.playerCurrentMove == MovementType.move)
        {
            player.SetMovementType(MovementType.crouch);
            isCrouching = true;
            player.SetCrouching(isCrouching);
        }
        else if(player.playerCurrentMove == MovementType.air)
        {
            player.GetAirComponent().GroundPound();
            player.SetMovementType(MovementType.crouch);
            isCrouching = true;

        }
    }

    public void deCrouch()
    {
        player.SetMovementType(MovementType.move);
        isCrouching = false;
        player.SetCrouching(isCrouching);
    }


    #region check ground functions

    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);
    private readonly float groundCheckRate = 0.1f;
    private RaycastHit footHit;

    public void GroundMe()
    {
        if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y))
        {
            if (footHit.collider.gameObject != null && !isCrouching)
            {
                player.SetGrounded(true);
                player.SetFlutter(true);
                player.SetMovementType(MovementType.move);
                player.GetAnimator().SetBool("Grounded", true);
                stateMachine.SetTrigger("GroundTrigger");
                if(player.playerCurrentMove == MovementType.move)
                {
                    psRun.Play();
                }
                else
                {
                    psRun.Stop();
                }
                if (!jumpParticleIsPlaying)
                {
                    psJump.Play();
                    jumpParticleIsPlaying = true;
                }

            }
            else if (footHit.collider.gameObject != null && isCrouching)
            {
                player.SetGrounded(true);
                player.SetFlutter(true);
                player.SetMovementType(MovementType.crouch);
                player.GetAnimator().SetBool("Crouching", true);
                if (player.playerCurrentMove == MovementType.move)
                {
                    psRun.Play();
                }
                else
                {
                    psRun.Stop();
                }
                if (!jumpParticleIsPlaying)
                {
                    psJump.Play();
                    jumpParticleIsPlaying = true;
                }
            }
            else if (footHit.collider.gameObject.tag == "MovingPlatform")
            {
                transform.parent = footHit.transform.parent;
                if (player.playerCurrentMove == MovementType.move)
                {
                    psRun.Play();
                }
                else
                {
                    psRun.Stop();
                }
                if (!jumpParticleIsPlaying)
                {
                    psJump.Play();
                    jumpParticleIsPlaying = true;
                }
            }
        }
    }

    public IEnumerator CheckGround()
    {
        if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y))
        {
            GroundMe();
        }
        else
        {
            player.SetGrounded(false);
            player.GetAnimator().SetBool("Grounded", false);
            //Debug.Log("not on ground");
            if (player.playerCurrentMove == MovementType.grapple)
            {

            }

            else
            {
                player.SetMovementType(MovementType.air);
            }
            //this is an issue for the fall trigger. We can't put it here since it'll as of now conflict with the Grapple Trigger
        }

        yield return new WaitForSecondsRealtime(groundCheckRate);


        StartCoroutine(CheckGround());
    }



    #endregion



    #region Fall Check


    ///LUKE FALLING CHECK

    bool isFalling;

    public bool IsPlayerFalling()
    {
        return isFalling;
    }

    float playerHeight;

    IEnumerator FallCheck()
    {
        if (player.IsGrounded())
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(FallCheck());
            yield break;

        }


        playerHeight = player.transform.position.y;
        yield return new WaitForSeconds(0.1f);

        if (playerHeight > player.transform.position.y)
        {
            player.isFalling = true;
            //Debug.Log("Player Falling");
        }
        else
            player.isFalling = false;

        StartCoroutine(FallCheck());


    }

    #endregion


    #region sphereOverlap


    //we can do this better in the editor, setting up the physics matrix
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            objectsInsideSphere.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            objectsInsideSphere.Remove(other);
    }

    //how often it'll trigger the coroutine
    private readonly float checkSphereRate = 0.2f;

    //how big the sphere
    private readonly float sphereSize = 10f;

    //list of objects
    public List<Collider> objectsInsideSphere;

    //SUPER IMPORTANT MAKE SURE THE LAYER IS CORRECT
    int layerMask = 1 << 11;


    public Collider closestPoint;

    public List<Collider> interactPoints = new List<Collider>();
    public List<float> distancesToObjects = new List<float>();

    public IEnumerator CheckSphere()
    {

        //objectsInsideSphere = Physics.OverlapSphere(this.transform.position, 10f, layerMask);



        if (objectsInsideSphere.Count > -1)
        {

            foreach (Collider objec in objectsInsideSphere)
            {

                Vector3 heading = objec.transform.position - transform.position;
                float dot = Vector3.Dot(heading, transform.forward);

                if (CheckHeight(player.transform.position.y) && IsInFront(dot))
                {
                    objec.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    interactPoints.Add(objec);

                    float dis = Vector3.Distance(transform.position, objec.transform.position);
                    distancesToObjects.Add(dis);

                }

            }

            if (interactPoints.Count > 0)
            {
                interactPoints[GetClosestDistances()].GetComponent<MeshRenderer>().material.color = Color.green;
                closestPoint = interactPoints[GetClosestDistances()];
                player.SetTetherPoint(closestPoint);
            }
            else
                closestPoint = null;


            yield return new WaitForSecondsRealtime(checkSphereRate);
            distancesToObjects.Clear();
            interactPoints.Clear();
            StartCoroutine(CheckSphere());
        }



    }


    int GetClosestDistances()
    {
        int distanceIterator = 0;

        float initialDistance = distancesToObjects[0];

        for (int i = 0; i < distancesToObjects.Count; i++)
        {
            if (distancesToObjects[i] < initialDistance)
            {
                initialDistance = distancesToObjects[i];
                distanceIterator = i;
            }


        }


        //distances.Clear();
        return distanceIterator;
    }

    bool IsInFront(float dot)
    {
        if (dot > 0)
            return true;

        return false;
    }

    bool CheckHeight(float height)
    {
        if (height < transform.position.y)
            return false;

        return true;
    }



    #endregion
}