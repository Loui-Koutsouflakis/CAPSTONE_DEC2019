//Created by Dylan LeClair 10/28/2019

//Platforms above large icicles must be on "Interacable" layer
//Large icicle references must be set in inspector

//IceBall object must be tagged "IceBall"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IciclePlatforms : MonoBehaviour, Interact
{
    //Object & scripts reference variables
    public GameObject icicleRef;
    public ParticleSystem particleRef;

    private CameraPaths camerPaths;
    private IceBall iceBallScript;
    private Icicles icicleScriptRef;

    //Helper variables
    private bool hasPlayed = false;

    //Cache wait time
    private readonly WaitForSeconds timer = new WaitForSeconds(0.6f);

    private void Awake()
    {
        //Get reference to scripts & components
        camerPaths = GetComponent<CameraPaths>();
        iceBallScript = FindObjectOfType<IceBall>();
    }

    private void Start()
    {
        //Get reference to icicle script
        icicleScriptRef = icicleRef.GetComponent<Icicles>();
    }

    void Interact.DontInteractWithMe()
    {
        //Unused
    }

    void Interact.InteractWithMe()
    {
        if(!hasPlayed)
        {
            //Check for final iceball stage
            if(iceBallScript.stageIndex == 3)
            {
                //Play final camera
                this.camerPaths.hasEndCam = true;
                this.camerPaths.stayTime = 0;
            }
            //Play cutscene & drop large icicle
            camerPaths.StartMeUp();
            icicleScriptRef.MakeIcicleFall();
            StartCoroutine(PauseAnimation());

            //Clean-up particles
            particleRef.Stop();

            //Ensure sequence only plays once
            hasPlayed = true;
        }
    }

    private IEnumerator PauseAnimation()
    {
        //Time iceball rolling to be after icicle drop
        yield return timer;

        iceBallScript.PlayIceBallAnimations();
    }
}
