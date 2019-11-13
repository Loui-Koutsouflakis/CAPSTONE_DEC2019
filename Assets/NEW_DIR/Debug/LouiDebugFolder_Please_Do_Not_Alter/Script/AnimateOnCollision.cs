using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnCollision : MonoBehaviour
{
    public Animator animator;
    public string triggerName;
    public bool useTrigger;
    public bool usePlayerLayer;

    const string kbodyName = "KBody";

    private void OnCollisionEnter(Collision collision)
    {
        if (!useTrigger)
        {
            if (collision.gameObject.name == kbodyName)
            {
                animator.SetTrigger(triggerName);
            }
            else if(usePlayerLayer && collision.gameObject.layer == 9)
            {
                animator.SetTrigger(triggerName);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger)
        {
            if (other.gameObject.name == kbodyName)
            {
                animator.SetTrigger(triggerName);
            }
            else if (usePlayerLayer && other.gameObject.layer == 9)
            {
                animator.SetTrigger(triggerName);
            }
        }
    }
}
