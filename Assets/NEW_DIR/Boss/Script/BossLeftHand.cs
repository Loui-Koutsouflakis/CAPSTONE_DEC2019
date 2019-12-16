using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLeftHand : MonoBehaviour
{
    public Animator leftHandAnim;

    private void Start()
    {
        if(leftHandAnim == null)
        {
            leftHandAnim = GetComponent<Animator>();
        }
    }

    public void TriggerLeftHandGrab()
    {
        leftHandAnim.SetTrigger("Grab");
    }

    public void TriggerLeftHandReverseGrab()
    {
        leftHandAnim.SetTrigger("ReverseGrab");
    }
}
