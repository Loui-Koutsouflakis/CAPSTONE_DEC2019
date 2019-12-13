//Created by Dylan LeClair 12/13/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeIcicles : MonoBehaviour, Interact
{
    private Animator animator;
    private AudioSource source;
    private new Collider collider;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
    }
    public void DontInteractWithMe()
    {
        //Unused
    }

    public void InteractWithMe()
    {
        animator.SetBool("Smash", true);
        source.Play();
        collider.enabled = false;
    }
}
