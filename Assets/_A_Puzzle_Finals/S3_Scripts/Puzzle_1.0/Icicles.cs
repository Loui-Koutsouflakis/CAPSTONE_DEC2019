//Created by Dylan LeClair 10/28/2019

//Small icicle gameobject must have rigidbody attached
//Icicle reference must be set in inspector

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
    private GameObject playerRef;
    private Rigidbody rigidbodyRef;
    private Animator animator;
    private AudioSource source;

    //Distance variables
    private float distanceToPlayer;
    private readonly float thresholdDistance = 20.0f;
    private Vector3 sinkDistance = new Vector3(0.0f, -2.0f, 0.0f);

    //CleanUp
    private float timer = 1.0f;

    private void Awake()
    {
        if(icicleSize == IcicleIdetifier.small)
        {
            rigidbodyRef = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        //Get reference to player object && animotor
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
            //Drop icicles
            rigidbodyRef.useGravity = true;
            rigidbodyRef.isKinematic = false;
        }
        if(icicleSize == IcicleIdetifier.large)
        {
            animator.SetBool("Shake", true);

            this.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        animator.SetBool("Smash", true);

        //source.time = 0.5f;
        source.Play();

        StartCoroutine(CleanUp());
    }

    private IEnumerator CleanUp()
    {
        yield return new WaitForSecondsRealtime(timer);

        gameObject.SetActive(false);
    }
}
