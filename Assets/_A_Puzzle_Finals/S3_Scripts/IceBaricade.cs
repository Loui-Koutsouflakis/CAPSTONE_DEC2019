//Created by Dylan LeClair 11/11/19

//Gamobject must have animator & audio source attached

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBaricade : MonoBehaviour
{
    //Components reference variables
    private Animator animator;
    private AudioSource source;

    private void Start()
    {
        //Get component references
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    public void PlayBreak()
    {
        //Play animation
        animator.SetBool("Break", true);

        //Time sound with animation
        source.time = 0.7f;
        source.PlayDelayed(1.2f);

        StartCoroutine(CleanUp());
    }

    private IEnumerator CleanUp()
    {
        //Wait for animation to finish, then clean un unnecessary assets
        yield return new WaitForSecondsRealtime(5.0f);

        gameObject.SetActive(false);
    }
}
