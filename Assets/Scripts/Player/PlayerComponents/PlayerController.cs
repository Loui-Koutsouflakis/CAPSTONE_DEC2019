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
    [Header("STATE MACHINE NOT ANIMATION")]
    //[SerializeField]
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

    //Tether and pull particle used in SphereOverlap region
    public ParticleSystem tetherParticle;
    ParticleSystem.EmissionModule tetherParticleEmission;
    public ParticleSystem pullParticle;
    ParticleSystem.EmissionModule pullParticleEmission;

    [SerializeField]
    SkinnedMeshRenderer[] lumiParts;
    private int timesThrough =0;


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
        player.SetDamagable(true);
        stateMachine = player.GetAnimator();
        tetherParticleEmission = tetherParticle.emission;
        tetherParticleEmission.enabled = false;
        pullParticleEmission = pullParticle.emission;
        pullParticleEmission.enabled = false;

        lumiParts = GetComponentsInChildren<SkinnedMeshRenderer>();


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

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.O))
    //    {
    //        StartCoroutine(DamageFlashOff());
    //    }
    //}

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


            //stateMachine.SetTrigger("Jump");
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
            //animation trigger moved to air script
            //stateMachine.SetTrigger("DJump");
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
            //stateMachine.SetTrigger("Jump");
        }
        else if (player.playerCurrentMove == MovementType.grapple)
        {
            DetatchGrapple();
        }
    }

    public void GrappleBeam()
    {
        Debug.Log("working");
        //just in case, should not have grapple and pull points close enough together to cause errors
        if (closestPoint != null && closestPullPoint == null)
        {
            Grapple();
        }
        else if (closestPullPoint != null && closestPoint == null)
        {
            Pull();
        }
        else if (closestPoint != null && closestPullPoint != null)
        {
            if (Vector3.Distance(transform.position, closestPoint.transform.position) <= Vector3.Distance(transform.position, closestPullPoint.transform.position))
            {
                Grapple();
            }
            else
            {
                Pull();
            }
        }
        else
            Debug.Log("nothing here");
    }

    public void Pull()
    {        
        closestPullPoint.GetComponent<Interact>().InteractWithMe();
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

        //WILL OPTOMIZE
        player.GetComponentInChildren<RayCast_IK>().IK_Grapple();
    }

    public void DetatchGrapple()
    {
        //player.isGrappling = false; moved to grapplecomponent

        player.GetGrappleComponent().DetatchGrapple();
        player.GetComponentInChildren<RayCast_IK>().IK_EndGrapple();

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
            player.SetGroundPounding(true);
            //player.SetMovementType(MovementType.crouch);
            isCrouching = true;
        }
    }

    public void deCrouch()
    {
        if(player.playerCurrentMove == MovementType.crouch)
        {
            player.SetMovementType(MovementType.move);
        }
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

    private void OnCollisionEnter(Collision collision) //to take damage from hitting player
    {
        if (collision.gameObject.layer == 10)//enemy layer
        {
            if(footHit.collider.gameObject == null || footHit.collider.gameObject.layer != 10)
            {
                if (player.GetDamagable())
                {
                    player.SetHealth(-1);
                    h_Manager.HealthDown();
                    player.GenericAddForce((collision.gameObject.transform.position - player.transform.position).normalized, 3);
                    StartCoroutine(DamageFlashOff());
                    player.SetDamagable(false);
                    if (player.GetHealth() <= 0)
                    {
                        //death animation
                    }
                }
            }
        }
    }

    IEnumerator DamageFlashOff()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (SkinnedMeshRenderer skin in lumiParts)
        {
            skin.enabled = false;
            //Vector4 color = skin.material.color;
            //color = new Vector4(color.x, color.y, color.z, 0.5f);
            //skin.material.color = color; 
        }

        StartCoroutine(DamageFlashOn());
    }

    IEnumerator DamageFlashOn()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (SkinnedMeshRenderer skin in lumiParts)
        {
            skin.enabled = true;
            //Vector4 color = skin.material.color;
            //color = new Vector4(color.x, color.y, color.z, 1.0f);
            //skin.material.color = color;
        }
        timesThrough += 1;
        if(timesThrough <= 5)
        {
            StartCoroutine(DamageFlashOff());
        }
        else
        {
            player.SetDamagable(true);
            timesThrough = 0;
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
                if(player.GetGroundPounding())
                {
                    if (footHit.collider.GetComponent<Interact>() != null)
                    {
                        if(footHit.collider.gameObject.layer == 16 || footHit.collider.gameObject.layer == 17) //will trigger the groundpound or walkon layers
                        {
                            footHit.collider.GetComponent<Interact>().InteractWithMe();
                        }
                    }
                    player.DisableControls();
                    StartCoroutine(GroundPoundStop());
                    player.SetGroundPounding(false);
                }

                if (footHit.collider.gameObject.tag == "MovingPlatform") //swtich to layer check not tag
                {
                    transform.parent = footHit.transform;
                    if(footHit.collider.GetComponent<Interact>() != null)
                    {
                        footHit.collider.GetComponent<Interact>().InteractWithMe();
                    }
                }

                if(footHit.collider.gameObject.tag == "EnemyWeakSpot")
                {
                    footHit.collider.gameObject.GetComponent<IKillable>().CheckHit();
                    player.GenericAddForce(transform.up, 5);
                }

                if (footHit.collider.GetComponent<Interact>() != null && footHit.collider.gameObject.layer == 17) //triggers walk on layer
                {
                    footHit.collider.GetComponent<Interact>().InteractWithMe();                 
                }


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
                if (player.GetGroundPounding())
                {
                    if (footHit.collider.GetComponent<Interact>() != null)
                    {
                        if (footHit.collider.gameObject.layer == 16 || footHit.collider.gameObject.layer == 17) //will trigger the groundpound or walkon layers
                        {
                            footHit.collider.GetComponent<Interact>().InteractWithMe();
                        }
                    }
                    player.DisableControls();
                    StartCoroutine(GroundPoundStop());
                    player.SetGroundPounding(false);
                }

                if (footHit.collider.gameObject.tag == "MovingPlatform") //swtich to layer check not tag
                {
                    transform.parent = footHit.transform;
                    if (footHit.collider.GetComponent<Interact>() != null)
                    {
                        footHit.collider.GetComponent<Interact>().InteractWithMe();
                    }
                }

                if (footHit.collider.gameObject.tag == "EnemyWeakSpot")
                {
                    footHit.collider.gameObject.GetComponent<IKillable>().CheckHit();
                    player.GenericAddForce(transform.up, 5);
                }

                if (footHit.collider.GetComponent<Interact>() != null && footHit.collider.gameObject.layer == 17) //triggers walk on layer
                {
                    footHit.collider.GetComponent<Interact>().InteractWithMe();
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
    
    IEnumerator GroundPoundStop()
    {
        yield return new WaitForSeconds(1.1f);
        player.EnableControls();
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
            if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, 0.3f, p_Layer))
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

    //These uses a bool amITether to determine if the object is a tehterpoint or pullpoint.
    private void TurnOnParticle(bool amITether, Vector3 particlePosition)
    {
        if (amITether)
        {
            tetherParticle.transform.position = particlePosition;
            tetherParticleEmission.enabled = true;
            return;
        }

        pullParticle.transform.position = particlePosition;
        pullParticleEmission.enabled = true;


    }

    private void TurnOffParticle(bool amITether)
    {
        if (amITether)
        {
            tetherParticleEmission.enabled = false;
            return;
        }

        pullParticleEmission.enabled = false;
    }



    //we can do this better in the editor, setting up the physics matrix
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            objectsInsideSphere.Add(other);
        else if (other.gameObject.layer == LayerMask.NameToLayer("Pullable"))
            pullableObjectsInSphere.Add(other);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            objectsInsideSphere.Remove(other);
        else if (other.gameObject.layer == LayerMask.NameToLayer("Pullable"))
            pullableObjectsInSphere.Remove(other);
    }

    //how often it'll trigger the coroutine
    private readonly float checkSphereRate = 0.2f;

    //how big the sphere
    private readonly float sphereSize = 10f;

    //list of objects
    public List<Collider> objectsInsideSphere;
    public List<Collider> pullableObjectsInSphere;

    //SUPER IMPORTANT MAKE SURE THE LAYER IS CORRECT
    int layerMask = 1 << 11;


    public Collider closestPoint = null;

    //for TetherPoints
    public List<Collider> interactPoints = new List<Collider>();
    public List<float> distancesToObjects = new List<float>();

    //for Pullable objects
    public List<Collider> pullPoints = new List<Collider>();
    public List<float> distanceToPullPoints = new List<float>();

    public Collider closestPullPoint = null;

    public IEnumerator CheckSphere()
    {

        //objectsInsideSphere = Physics.OverlapSphere(this.transform.position, 10f, layerMask);               
        //TetherPoints
        if (objectsInsideSphere.Count > 0)
        {
            foreach (Collider objec in objectsInsideSphere)
            {
                Vector3 heading = objec.transform.position - transform.position;
                float dot = Vector3.Dot(heading, transform.forward);

                if (CheckHeight(player.transform.position.y) && IsInFront(dot))
                {
                    interactPoints.Add(objec);

                    float dis = Vector3.Distance(transform.position, objec.transform.position);
                    distancesToObjects.Add(dis);
                }
            }

            if (interactPoints.Count > 0)
            {
                closestPoint = interactPoints[GetClosestDistances(distancesToObjects)];
                TurnOnParticle(true, closestPoint.transform.position);
                player.SetTetherPoint(closestPoint);
            }
            else
            {
                closestPoint = null;
                player.SetTetherPoint(null);
                TurnOffParticle(true);
            }

            distancesToObjects.Clear();
            interactPoints.Clear();

        }
        
            


        if (pullableObjectsInSphere.Count > 0)
        {
            //Pullables
            foreach (Collider objec in pullableObjectsInSphere)
            {
                Vector3 heading = objec.transform.position - transform.position;
                float dot = Vector3.Dot(heading, transform.forward);
                Vector3 reverseHeading = transform.position - objec.transform.position;
                float reverseDot = Vector3.Dot(reverseHeading, objec.transform.forward);

                if (IsInFront(dot) && IsInFront(reverseDot)) //will only check if the object is in front of lumi and lumi is in front of the object
                {
                    pullPoints.Add(objec);

                    float dis = Vector3.Distance(transform.position, objec.transform.position);
                    distanceToPullPoints.Add(dis);
                }
            }

            if (pullPoints.Count > 0)
            {
                closestPullPoint = pullPoints[GetClosestDistances(distanceToPullPoints)];
                TurnOnParticle(false, closestPullPoint.transform.position);
            }
            else
            {
                closestPullPoint = null;
                TurnOffParticle(false);
            }

            pullPoints.Clear();
            distanceToPullPoints.Clear();
        }
        
           


        //generic
        yield return new WaitForSecondsRealtime(checkSphereRate);         
        StartCoroutine(CheckSphere());
    }


    int GetClosestDistances(List<float> list)
    {
        int distanceIterator = 0;

        float initialDistance = list[0];

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] < initialDistance)
            {
                initialDistance = list[i];
                distanceIterator = i;
            }
        }
        //distances.Clear();
        return distanceIterator;
    }

    int GetClosestPullDistances()
    {
        int distanceIterator = 0;

        float initialDistance = distanceToPullPoints[0];

        for (int i = 0; i < distanceToPullPoints.Count; i++)
        {
            if (distanceToPullPoints[i] < initialDistance)
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