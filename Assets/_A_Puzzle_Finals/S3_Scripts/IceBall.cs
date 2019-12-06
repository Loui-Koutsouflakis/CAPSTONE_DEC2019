﻿//Created by Dylan LeClair 11/7/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : MonoBehaviour
{
    //Object & component references variables
    private Animator animator;
    private AudioSource source;
    private IceBaricade iceBaricade;

    //Distiguish between animation stages
    public int stageIndex;
    private enum Stage { one, two, three }
    private Stage index = Stage.one;

    private void Awake()
    {
        iceBaricade = FindObjectOfType<IceBaricade>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    public void PlayIceBallAnimations()
    {
        source.time = 1.0f;
        switch (index)
        {
            case Stage.one:
                animator.SetInteger("Index", 1); 
                index = Stage.two;
                stageIndex = 2;
                break;
            case Stage.two:
                animator.SetInteger("Index", 2);
                index = Stage.three;
                stageIndex = 3;
                break;
            case Stage.three:
                animator.SetInteger("Index", 3);
                this.enabled = false;
                break;
            default:
                break;
        }
    }

    public void GoToBaricade()
    {
        iceBaricade.PlayBreak();
    }
}