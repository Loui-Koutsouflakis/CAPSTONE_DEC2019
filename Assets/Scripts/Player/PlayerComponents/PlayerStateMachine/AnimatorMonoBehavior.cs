using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMonoBehavior : MonoBehaviour
{
    public void StartRoutine(Animator anim)
    {
        StartCoroutine(IdleStop(anim));
    }
    IEnumerator IdleStop(Animator anim)
    {
        yield return new WaitForSeconds(8);
        anim.SetBool("Idle", false);
    }
}
