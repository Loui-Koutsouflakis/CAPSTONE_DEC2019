﻿//created by Luke Fentress - controller script to handle player movement types
//created by Luke Fentress - controller script to handle player movement types
//edited by AT - 19-08-09 - added air controller functionallity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Player Controller", 1)]

public class PlayerController : MonoBehaviour
{
    public PlayerClass player;

    private HudManager h_Manager;
    public TransitionManager t_Manager;
    public Transform teleportPsParent;
    public ParticleSystem[] teleport;

    private Animator anim;
       
    public LayerMask p_Layer = 1 << 9;
    
    //annoying temporary check until we slay the beast that is the grappleComponent tether bool
    bool isTethered = false;
    bool isCrouching = false;

    [SerializeField]
    private bool canMultiJump = true;   

    //Tether and pull particle used in SphereOverlap region
    public ParticleSystem tetherParticle;
    ParticleSystem.EmissionModule tetherParticleEmission;
    public ParticleSystem pullParticle;
    ParticleSystem.EmissionModule pullParticleEmission;

    //run and jump particles
    public ParticleSystem psJump;
    public ParticleSystem psRun;
    public bool jumpParticleIsPlaying;

    //meshes of the model
    [SerializeField]
    SkinnedMeshRenderer[] lumiParts;
    private int timesThrough = 0;

    private RayCast_IK playerIK;

    //for testing parenting on boss level only
    public bool bossLevel = false;


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
        anim = player.GetAnimator();
        h_Manager = player.GetHManager();
        tetherParticleEmission = tetherParticle.emission;
        tetherParticleEmission.enabled = false;
        pullParticleEmission = pullParticle.emission;
        pullParticleEmission.enabled = false;
        playerIK = GetComponentInChildren<RayCast_IK>();
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

    #region Inputs
    public void ShowHud()
    {
        h_Manager.HudButtonDown();
    }    

    public void Jump()
    {
        //jump executed depend on the current movement component
        if (player.playerCurrentMove == MovementType.move)
        {
            player.GetMoveComponent().Jump();
            //set movement type to air
            player.SetMovementType(MovementType.air);
            //sets grounded to false outside of
            player.SetGrounded(false);

            //stateMachine.SetTrigger("Jump");
            anim.SetBool("Grounded", false);

            //Particle stuff from Tony
            psRun.Stop();
            psJump.Stop();
            psJump.Play();
            jumpParticleIsPlaying = false;
        }
        else if (player.playerCurrentMove == MovementType.air && canMultiJump) // bool to be able to turn off ability to double jump/wall jump
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
            anim.SetTrigger("GrappleJump");
            DetatchGrapple();
            player.GenericAddForce(transform.forward + transform.up, 3); //adds more force when jumping out of a swing
        }
    }

    public void GrappleBeam()
    {
        //Debug.Log("working");
        //just in case, should not have grapple and pull points close enough together in level to cause errors
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

        //by limiting what angle the player approaches the tether point, can prevent the minimize in circles around the tether point
        Vector3 tetherVector = player.tetherPoint.transform.position - player.transform.position;
        Vector3 tetherProjection = Vector3.ProjectOnPlane(tetherVector, player.transform.up);
        float angleBetween = Vector3.Angle(player.transform.forward, tetherProjection);
        Debug.DrawLine(transform.position, transform.position + tetherProjection.normalized * 10, Color.black, 10);
        //Debug.Log(angleBetween);
        if(angleBetween > 45)
        {
            //Debug.Log("to far");
            return;
        }       

        player.isGrappling = true;
        anim.SetBool("Grapple", true);
        //stateMachine.SetTrigger("GrappleTrigger");
        
        player.SetMovementType(MovementType.grapple);
        player.GetGrappleComponent().Grapple();
        playerIK.IK_Grapple();
    }

    //check with Luke to see of there's a way to do this on a delay, to allow throw animation to play
    //IEnumerator StartGrapple()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    player.ikGrapple = true;
    //    playerIK.IK_Grapple();
    //}

    

    public void DetatchGrapple()
    {
        anim.SetBool("Grapple", false);
        
        player.GetGrappleComponent().DetatchGrapple();
        player.GetComponentInChildren<RayCast_IK>().IK_EndGrapple();
        
        //stateMachine.SetTrigger("FallTrigger");
        player.SetMovementType(MovementType.air);
        //player.ikGrapple = false;
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
            if(!player.GetGroundPounding())
            {
                player.GetAirComponent().GroundPound();
                player.SetGroundPounding(true);
                //player.SetMovementType(MovementType.crouch);
                isCrouching = true;
            }
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
            //can only speed up if below a certain angle off of tether point
            if(player.attachedGrapplePoint != null)
            {
                Vector3 tetherVector = player.attachedGrapplePoint.transform.position - player.transform.position;
                Vector3 tetherProjection = Vector3.ProjectOnPlane(tetherVector, player.transform.right);
                float angleBetween = Vector3.Angle(player.transform.forward, tetherProjection);
                if (angleBetween > 45 && angleBetween < 135)
                {
                    Debug.Log(angleBetween);
                    player.GetGrappleComponent().SpeedUp();
                }
            }            
        }
    }
    #endregion

    //ability for player to take damage
    #region Player Damage
    //only using collision ckecks for taking damage from enemies
    private void OnCollisionEnter(Collision collision) //to take damage from hitting enemy
    {
        if (collision.gameObject.layer == 10)//enemy layer
        {
            if(footHit.collider.gameObject == null || footHit.collider.gameObject.layer != 10) //will not take damage when  
            {
                if (player.GetDamagable())
                {
                    player.SetHealth(-1); //reduces health on player class
                    h_Manager.HealthDown(); //reduces health on hud
                    player.GenericAddForce((collision.gameObject.transform.position - player.transform.position).normalized, 3); //knocks player away from enemy
                    StartCoroutine(DamageFlashOff());
                    player.SetDamagable(false); //provides a brief period of invulnerability 
                    if (player.GetHealth() <= 0)
                    {
                        //death animation
                    }
                }
            }
            else if (collision.gameObject.name == "TeleportNext")
            {

                t_Manager.StartCoroutine(t_Manager.SceneTransition(1));

                teleportPsParent.position = transform.position;

                foreach (ParticleSystem ps in teleport)
                {
                    ps.Play();
                }

                gameObject.SetActive(false); // REPLACE WITH DISSOLVE EFFECT
            }
            else if (collision.gameObject.name == "TeleportPrev")
            {
                t_Manager.StartCoroutine(t_Manager.SceneTransition(-1));

                teleportPsParent.position = transform.position;

                foreach (ParticleSystem ps in teleport)
                {
                    ps.Play();
                }

                gameObject.SetActive(false); // REPLACE WITH DISSOLVE EFFECT
            }
        }
        
    }

    //damage flash on/off are repeating coroutines that turn off/on mesh
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
        //once the loops has iterated enough times, turn off the flashing and allows the player to be damaged again
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
    #endregion

    //Ground checks
    #region check ground functions

    private readonly Vector3 halves = new Vector3(0.25f, 0.25f, 0.25f);
    private readonly float groundCheckRate = 0.01f;
    private RaycastHit footHit;

    public IEnumerator CheckGround()
    {
        if (player.GetGroundCheck()) //fix to the bug where will only get partial jumps sometimes turns off setting grounded directly after a jump
        {
            if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y, p_Layer))
            {
                //GroundMe();
                if (Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y, p_Layer))
                {
                    //to jump on enemies
                    if (footHit.collider.gameObject.tag == "EnemyWeakSpot")
                    {
                        StartCoroutine(footHit.collider.GetComponent<IKillable>().CheckHit(player.GetGroundPounding())); //also check to see if enemy is damagable (bool) so will not continue to check if not damagable
                        Debug.Log("Hitting Spider");
                        //if (!player.GetGroundPounding())//move this to the enemies side of things
                        //    player.GenericAddForce(transform.up, 5);
                    }
                    //for ground pounding checks
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

                    //to parent to moving platforms
                    if (footHit.collider.gameObject.tag == "MovingPlatform") //swtich to layer check not tag
                    {
                        transform.parent = footHit.transform;
                        if (footHit.collider.GetComponent<Interact>() != null)
                        {
                            footHit.collider.GetComponent<Interact>().InteractWithMe();
                        }
                    }


                    //to bounce off bouncy things
                    if (footHit.collider.gameObject.tag == "Bouncy")
                    {
                        //bounce higher if holding the jump button
                        if (Input.GetButton("AButton"))
                        {
                            //Debug.Log("jump");
                            player.GenericAddForce(player.transform.up, 15);
                        }
                        else
                        {
                            //Debug.Log("bounce");
                            player.GenericAddForce(player.transform.up, 10);
                        }
                        player.SetBouncing(true);
                    }

                    //walk on triggers
                    if (footHit.collider.GetComponent<Interact>() != null && footHit.collider.gameObject.layer == 17) //triggers walk on layer
                    {
                        footHit.collider.GetComponent<Interact>().InteractWithMe();
                    }


                    if (player.GetBouncing())
                    {
                        anim.SetBool("Grounded", false);
                        anim.SetTrigger("Jump");
                        player.SetGrounded(false);
                    }
                    //probably need to put below in an else
                    if (footHit.collider.gameObject != null && !isCrouching)
                    {
                        player.SetGrounded(true);
                        player.SetFlutter(true);
                        player.SetMovementType(MovementType.move);
                        anim.SetBool("Grounded", true);
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
                        player.SetGrounded(true);
                        player.SetFlutter(true);
                        player.SetMovementType(MovementType.crouch);
                        anim.SetBool("Grounded", true);
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
            else
            {
                player.SetGrounded(false);
                //if(transform.parent != null)
                //{
                //    transform.parent = null;
                //}
                anim.SetBool("Grounded", false);

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
    }

    //allows movement again after groundpouding
    IEnumerator GroundPoundStop()
    {
        yield return new WaitForSeconds(1.1f);
        player.EnableControls();
    }

    //breaks parenting if was on moving platform, needed a longer time to prevent jittering
    //may be able to move it back to check ground functions once we get flat platforms
    public IEnumerator PlatformCheck()
    {
        if (!Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y, p_Layer))
        {
            if (transform.parent != null & !bossLevel)
            {
                transform.parent = null;
            }
        }
        yield return new WaitForSecondsRealtime(0.6f);

        StartCoroutine(PlatformCheck());
    }
    #endregion

    //Fall checks
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
                if (anim.GetFloat("YVelocity") < 0)
                {                   
                    anim.SetTrigger("Land");
                    Debug.Log("landing");
                }
            }
        }
        yield return new WaitForSecondsRealtime(0.06f);
               
        StartCoroutine(LandingCheck());       
    }
    #endregion

    //Grapple/pull points checks
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
    //adds all grapple objects (tether points and pullable objects) to a list when they enter the sphere collider 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable")) 
            objectsInsideSphere.Add(other);
        else if (other.gameObject.layer == LayerMask.NameToLayer("Pullable")) 
            pullableObjectsInSphere.Add(other);
    }

    //removes the objects when they leave the sphere
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
    
    //for TetherPoints
    public List<Collider> interactPoints = new List<Collider>();
    public List<float> distancesToObjects = new List<float>();

    public Collider closestPoint = null;
    
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
            //checks for tether points within the shere which are also in front and above player and adds all those to a second list
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
            //finds the closest point to make the currently active tether point
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
        else
        {
            closestPoint = null;
        }

        //checks for pull points within the shere which are also in front and above player and adds all those to a second list
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
            //finds the closest point to make the currently active pull point
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

    //compares distances from a list that is fed in and returns the closest
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

    //checks if an object is in front of player
    bool IsInFront(float dot)
    {
        if (dot > 0)
            return true;

        return false;
    }

    //checks to see if object is above player
    bool CheckHeight(float height)
    {
        if (height < transform.position.y)
            return false;

        return true;
    }
    #endregion
}