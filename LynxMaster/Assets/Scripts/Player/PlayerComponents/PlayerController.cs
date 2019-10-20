//created by Luke Fentress - controller script to handle player movement types
//created by Luke Fentress - controller script to handle player movement types
//edited by AT - 19-08-09 - added air controller functionallity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Player Controller", 1)]

public class PlayerController : MonoBehaviour
{

    public PlayerClass player;
    public HudManager h_Manager;
    public Transform meteorPool;

    [Header("STATE MACHINE NOT ANIMATION")]
    [SerializeField]
    Animator stateMachine;

    //public Animator anim;

    public ParticleSystem psJump;
    public ParticleSystem psRun;
    public bool jumpParticleIsPlaying;
    //annoying temporary check until we slay the beast that is the grappleComponent tether bool
    bool isTethered = false;
    bool isCrouching = false;
    public LayerMask p_Layer = 1 << 9;
    [SerializeField]
    private bool canMultiJump = true;
    //for the swimming script
    public float waterCheckDist = 100;
    RaycastHit water;

    #region Enemy Stuff
    public int spiderWebs = 0;
    public bool paused = false;
    #endregion

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
        StartCoroutine(PlatformCheck());
        StartCoroutine(LandingCheck());
        p_Layer = ~p_Layer;
        //debugging
        player.debugLine.GetComponent<LineRenderer>().enabled = false;
    }

    private void Update()
    {
   
        //// if (isUnderWater())
        //     if (water.collider.tag == "Water")
        //     {
        //         player.SetGrounded(false);
        //         player.SetSwimming(true);
        //         player.SetMovementType(MovementType.swim);
        //     }
        //player.vel = player.rb.velocity;
        //Debug.Log(player.vel);
    }

    public void ShowHud()
    {
        h_Manager.HudButtonDown();
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


            stateMachine.SetTrigger("Jump");
            stateMachine.SetBool("Grounded", false);

            //Particle stuff from Tony
            psRun.Stop();
            psJump.Stop();
            psJump.Play();
            jumpParticleIsPlaying = false;
        }
        else if (player.playerCurrentMove == MovementType.air && canMultiJump) // bool to be able to turn off ability to double jump/wall jump
        {
            player.GetAirComponent().Jump();
            stateMachine.SetTrigger("DJump");
        }
        else if (player.playerCurrentMove == MovementType.crouch)
        {
            player.GetCrouchComponent().Jump();
            player.SetMovementType(MovementType.air);
            player.SetGrounded(false);
           // player.SetSwimming(false);
            //Particle stuff from Tony
            psRun.Stop();
            psJump.Stop();
            psJump.Play();
            stateMachine.SetTrigger("Jump");
        }
        else if (player.playerCurrentMove == MovementType.grapple)
        {
            DetatchGrapple();
        }

        //else if(player.playerCurrentMove == MovementType.swim)
        //{
        //    player.GetSwimComponent().swim();
        //}
    }

    public void Grapple()
    {
        if (player.isGrappling || player.IsGrounded() || player.tetherPoint == null)
        {
            Debug.Log("Cant grapple");
            return;
        }
        player.isGrappling = true;
        //stateMachine.SetTrigger("GrappleTrigger");
        player.SetMovementType(MovementType.grapple);
        player.GetGrappleComponent().Grapple();
    }

    public void DetatchGrapple()
    {
        //player.isGrappling = false; moved to grapplecomponent

        player.GetGrappleComponent().DetatchGrapple();
        //stateMachine.SetTrigger("FallTrigger");
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

    public void SpeedUp()
    {
        if(player.playerCurrentMove == MovementType.grapple)
        {
            player.GetGrappleComponent().SpeedUp();
        }
    }

    #region check ground functions

    private readonly Vector3 halves = new Vector3(0.25f, 0.25f, 0.25f);
    private readonly float groundCheckRate = 0.01f;
    private RaycastHit footHit;

    public void GroundMe()
    {
        // We can probably move this into the ground check>??? to reduce the casts
        if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y, p_Layer))
        {

            if (footHit.collider.gameObject != null && !isCrouching)
            {
                if (footHit.collider.gameObject.tag == "MovingPlatform") //swtich to layer check not tag
                {
                    transform.parent = footHit.transform.parent;
                }
                if(footHit.collider.gameObject.tag == "Spiderlings")
                    footHit.collider.gameObject.GetComponent<Spiderlings>().Die();
                player.SetGrounded(true);
                player.SetFlutter(true);
                player.SetMovementType(MovementType.move);
                stateMachine.SetBool("Grounded", true);
                //stateMachine.SetTrigger("GroundTrigger");                
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
            else if (footHit.collider.gameObject != null && isCrouching)
            {
                if (footHit.collider.gameObject.tag == "MovingPlatform") //swtich to layer check not tag
                {
                    transform.parent = footHit.transform.parent;
                }
                player.SetGrounded(true);
                player.SetFlutter(true);
                player.SetMovementType(MovementType.crouch);
                stateMachine.SetBool("Grounded", true);
                //stateMachine.SetBool("Crouching", true);
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
    

    //public bool isUnderWater()
    //{
    //    Vector3 lineStart = player.transform.position;
    //    Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y + waterCheckDist, lineStart.z);
    //    Debug.DrawLine(lineStart, vectorToSearch, Color.black);
    //    return Physics.Linecast(lineStart, vectorToSearch, out water, p_Layer);
    //}

    public IEnumerator CheckGround()
    {
        //if (isUnderWater() && water.collider.tag != "Water" || !isUnderWater())
        //{
        if (player.GetGroundCheck()) //fix to the bug where will only get partial jumps sometimes turns off setting grounded directly after a jump
        {
            if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y, p_Layer))
            {
                GroundMe();
            }
            else
            {
                player.SetGrounded(false);

                //if(transform.parent != null)
                //{
                //    transform.parent = null;
                //}
                   // player.SetSwimming(false);

                stateMachine.SetBool("Grounded", false);
                
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
        }

        yield return new WaitForSecondsRealtime(groundCheckRate);


        StartCoroutine(CheckGround());
        //}
    }

    //breaks parenting if was on moving platform, needed a longer time to prevent jittering
    public IEnumerator PlatformCheck()
    {
        if (!Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y, p_Layer))
        {
            if (transform.parent != null)
            {
                transform.parent = null;
            }
        }
        
        yield return new WaitForSecondsRealtime(0.6f);
        
        StartCoroutine(PlatformCheck());
}



    #endregion



    #region Fall Check


    ///LUKE FALLING CHECK
    [SerializeField]
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

    public IEnumerator LandingCheck()
    {
        if (player.GetGroundCheck()) //fix to the bug where will only get partial jumps sometimes turns off setting grounded directly after a jump
        {
            if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y + 0.1f, p_Layer))
            {
                if (stateMachine.GetFloat("YVelocity") < 0)
                {                   
                    stateMachine.SetTrigger("Land");
                    Debug.Log("landing");
                }
            }
        }
        yield return new WaitForSecondsRealtime(0.06f);
               
        StartCoroutine(LandingCheck());       
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
            {
                closestPoint = null;
                player.SetTetherPoint(null);
            }

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