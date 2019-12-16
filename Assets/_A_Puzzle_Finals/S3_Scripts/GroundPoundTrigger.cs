//Created by Dylan LeClair 12/13/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundTrigger : MonoBehaviour, Interact
{
    //Object & scripts reference variables
    public GameObject icicleRef;
    public GameObject movingPlatform;
    public ParticleSystem particleRef;

    private Animator animator;
    private AudioSource source;
    private CameraPaths camerPaths;
    private new Collider collider;
    private IceBall iceBallScript;
    private Icicles icicleScriptRef;

    //Helper variables
    private bool hasPlayed = false;

    //Cache wait time
    private readonly WaitForSeconds timer = new WaitForSeconds(0.6f);

    private void Awake()
    {
        //Get reference to scripts & components
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        camerPaths = GetComponent<CameraPaths>();
        collider = GetComponent<Collider>();
        iceBallScript = FindObjectOfType<IceBall>();
    }

    private void Start()
    {
        //Get reference to icicle script
        icicleScriptRef = icicleRef.GetComponent<Icicles>();

        //Avoid error on other triggers
        if(!movingPlatform)
        {
            movingPlatform = null;
        }
    }

    void Interact.DontInteractWithMe()
    {
        //Unused
    }

    void Interact.InteractWithMe()
    {
        if (!hasPlayed)
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
            StartCoroutine(PauseAnimation());

            //Clean-up particles
            particleRef.Stop();

            //Ensure sequence only plays once
            hasPlayed = true;

            //Activate moving platform
            if(icicleRef.tag == "PlatformCreation")
            {
                movingPlatform.SetActive(true);
            }
        }
    }

    private IEnumerator PauseAnimation()
    {
        //Time iceball rolling to be after icicle drop
        yield return timer;

        iceBallScript.PlayIceBallAnimations();
        collider.enabled = false;
    }
}
