//Created by Dylan LeClair 12/16/2019

//Small icicle gameobject must have rigidbody attached
//All icicles must have animator and audio source attached
//Icicle reference must be set in inspector "Small or Large"

//Player gameobject must be tagged "Player"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IciclesController : MonoBehaviour, Interact
{
    //Distiguish between large and small icicles
    public enum IcicleIdetifier { small, large, GroundPound };
    public IcicleIdetifier icicleSize;

    //Object & component reference variables
    public GameObject icicleRef;
    public ParticleSystem particleRef;

    private Animator animator;
    private CameraPaths camerPaths;
    private new Collider collider;
    private IceBall iceBallScript;
    private Icicles icicleScriptRef;
    private AudioSource source;
    private GameObject playerRef;
    private Rigidbody rigidbodyRef;

    //Distance variables
    private float distanceToPlayer;
    private readonly float thresholdDistance = 20.0f;

    //Helper variables
    private bool hasPlayed = false;

    //Cache wait time
    private readonly WaitForSeconds smallIciclesCleanUp = new WaitForSeconds(1.0f);
    private readonly WaitForSeconds timeIceBallMove = new WaitForSeconds(0.6f);

    private void Awake()
    {
        //Get small icicles references
        if (icicleSize == IcicleIdetifier.small)
        {
            rigidbodyRef = GetComponent<Rigidbody>();
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }
        //Get groundpound references
        if(icicleSize == IcicleIdetifier.GroundPound)
        {
            camerPaths = GetComponent<CameraPaths>();
            collider = GetComponent<Collider>();
            iceBallScript = FindObjectOfType<IceBall>();

            //Get reference to icicle script
            icicleScriptRef = icicleRef.GetComponent<Icicles>();
        }
    }

    private void Start()
    {
        //Get references for all icicles
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        if (rigidbodyRef)
        {
            //Keep icicles in the air
            rigidbodyRef.useGravity = false;
            rigidbodyRef.isKinematic = true;
        }
    }

    private void Update()
    {
        //Keep track of distance from player to small icicles
        distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);

        //Check if small icicles should fall
        if (icicleSize == IcicleIdetifier.small && distanceToPlayer < thresholdDistance)
        {
            //Call falling script
            MakeIcicleFall();
        }
    }

    public void InteractWithMe()
    {
        if (icicleSize == IcicleIdetifier.GroundPound && !hasPlayed)
        {
            //Check for final iceball stage
            if (iceBallScript.stageIndex == 3)
            {
                //Play final camera
                this.camerPaths.hasEndCam = true;
                this.camerPaths.stayTime = 0;
            }
            //Smash icicle
            animator.SetBool("Smash", true);
            source.Play();

            //Play cutscene & drop large icicle
            camerPaths.StartMeUp();
            icicleScriptRef.MakeIcicleFall();
            StartCoroutine(CorutineHandler(timeIceBallMove));

            //Clean-up particles
            particleRef.Stop();

            //Ensure sequence only plays once
            hasPlayed = true;
        }
    }

    public void DontInteractWithMe()
    {
        //Unused
    }

    public void MakeIcicleFall()
    {
        if (icicleSize == IcicleIdetifier.small)
        {
            //Drop small icicles
            rigidbodyRef.useGravity = true;
            rigidbodyRef.isKinematic = false;
        }
        if (icicleSize == IcicleIdetifier.large)
        {
            //Drop large icicles
            animator.SetBool("Shake", true);

            //Turn off script
            this.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Play smashing animation and sound
        if (icicleSize == IcicleIdetifier.small)
        {
            animator.SetBool("Smash", true);

            source.Play();

            StartCoroutine(CorutineHandler(smallIciclesCleanUp));
        }
    }

    private IEnumerator CorutineHandler(WaitForSeconds timer)
    {
        //Time events
        yield return timer;

        if(timer == smallIciclesCleanUp)
        {
            gameObject.SetActive(false);
        }
        if(timer == timeIceBallMove)
        {
            iceBallScript.PlayIceBallAnimations();
            collider.enabled = false;
        }
    }
}
