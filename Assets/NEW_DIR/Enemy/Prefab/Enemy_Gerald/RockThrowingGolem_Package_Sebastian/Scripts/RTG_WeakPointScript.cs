using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTG_WeakPointScript : MonoBehaviour,IKillable
{
    public Animator animator;
    public RockThrowingGolem RTG;
    public GameObject RTG_Prefab;
    public IEnumerator CheckHit(bool x)
    {
        //Player.GetComponent<Rigidbody>().AddForce(KnockBackForce, ForceMode.Impulse);
        animator.SetTrigger("IsHit");
        
        if (RTG.health > 0)
        {
            StartCoroutine(TakeDamage());
        }
        else if (RTG.health == 0)
        {
            StartCoroutine(Die());
        }
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator TakeDamage()
    {
        RTG.health -= 1;
        yield return new WaitForSeconds(0.1f);
    }
    public IEnumerator Die()
    {
        RTG.LeftFootWalking_PS = null;
        RTG.RightFootWalking_PS = null;
        RTG_Prefab.SetActive(false);
        yield return new WaitForSeconds(0.1f);
    }
}
