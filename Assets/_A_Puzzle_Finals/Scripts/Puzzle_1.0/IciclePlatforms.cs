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
    private Icicles icicleScriptRef;
    private IceBall iceBallScript;
    CameraPaths camerPaths;

    private readonly float timer = 0.6f;

    private void Awake()
    {
        iceBallScript = FindObjectOfType<IceBall>();
        camerPaths = GetComponent<CameraPaths>();
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
        //Call falling script
        camerPaths.StartMeUp();
        icicleScriptRef.MakeIcicleFall();
        StartCoroutine(PauseAnimation());
        this.enabled = false;
    }

    private IEnumerator PauseAnimation()
    {
        yield return new WaitForSeconds(timer);

        iceBallScript.PlayIceBallAnimations();
    }
}
