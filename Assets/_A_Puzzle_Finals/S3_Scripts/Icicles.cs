//Created by Dylan LeClair 10/28/2019

//Small icicle gameobject must have rigidbody attached
//All icicles must have animator and audio source attached
//Icicle reference must be set in inspector "Small or Large"

//Player gameobject must be tagged "Player"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicles : MonoBehaviour
{
    //Distiguish between large and small icicles
    public enum IcicleIdetifier { small, large };
    public IcicleIdetifier icicleSize;

    //Object & component reference variables
    private Animator animator;
    private AudioSource source;
    private GameObject playerRef;
    private Rigidbody rigidbodyRef;

    //Distance variables
    private float distanceToPlayer;
    private readonly float thresholdDistance = 20.0f;

    //Cache wait time
    private readonly WaitForSeconds timer = new WaitForSeconds(1.0f);

    private void Awake()
    {
        //Get rigidbody reference from small icicles
        if(icicleSize == IcicleIdetifier.small)
        {
            rigidbodyRef = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        //Get reference to player object && components
        playerRef = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        if(rigidbodyRef)
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
        if(icicleSize == IcicleIdetifier.small && distanceToPlayer < thresholdDistance)
        {
            //Call falling script
            MakeIcicleFall();
        }
    }

    public void MakeIcicleFall()
    {
        if(icicleSize == IcicleIdetifier.small)
        {
            //Drop small icicles
            rigidbodyRef.useGravity = true;
            rigidbodyRef.isKinematic = false;
        }
        if(icicleSize == IcicleIdetifier.large)
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

            StartCoroutine(CleanUp());
        }
    }

    private IEnumerator CleanUp()
    {
        //Clean up small icicles after animation has played
        yield return timer;

        gameObject.SetActive(false);
    }
}
