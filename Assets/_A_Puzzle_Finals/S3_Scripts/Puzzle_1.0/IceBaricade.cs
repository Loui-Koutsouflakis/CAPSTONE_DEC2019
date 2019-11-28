//Created by Dylan LeClair 11/11/19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBaricade : MonoBehaviour
{
    private Animator animator;
    private AudioSource source;

    private float timer =5.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    public void PlayBreak()
    {
        animator.SetBool("Break", true);

        source.time = 0.7f;
        source.PlayDelayed(1.2f);
        StartCoroutine(CleanUp());
    }

    private IEnumerator CleanUp()
    {
        yield return new WaitForSecondsRealtime(timer);

        gameObject.SetActive(false);
    }
}
