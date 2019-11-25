//Created by Dylan LeClair 11/11/19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBaricade : MonoBehaviour
{
    private Animator animator;

    private float timer =5.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayBreak()
    {
        animator.SetBool("Break", true);

        StartCoroutine(CleanUp());
    }

    private IEnumerator CleanUp()
    {
        yield return new WaitForSecondsRealtime(timer);

        gameObject.SetActive(false);
    }
}
