using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBaricade : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayBreak()
    {
        animator.SetBool("Break", true);
    }
}
