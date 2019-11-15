//Created by Dylan LeClair 11/7/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : MonoBehaviour
{
    private Animator animator;

    private enum Stage { one, two, three }
    private Stage index = Stage.one;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIceBallAnimations()
    {
        switch (index)
        {
            case Stage.one:
                animator.SetInteger("Index", 1); 
                index = Stage.two;
                break;
            case Stage.two:
                animator.SetInteger("Index", 2);
                index = Stage.three;
                break;
            case Stage.three:
                animator.SetInteger("Index", 3);
                this.enabled = false;
                break;
            default:
                break;
        }
    }
}
