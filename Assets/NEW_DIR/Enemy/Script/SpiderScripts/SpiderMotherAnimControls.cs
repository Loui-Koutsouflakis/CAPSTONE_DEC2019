using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMotherAnimControls : MonoBehaviour
{
    private Animator animator;
    private MotherSpider spiderMother;

    private void Awake()
    {
        if (GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
        }
        else
        {
            Debug.Log("Missing animator on mother spider.");
        }

        if(GetComponentInParent<MotherSpider>() != null)
        {
            spiderMother = GetComponentInParent<MotherSpider>();
        }
        else
        {
            Debug.Log("Can not find spiderMother script.");
        }
    }

    public void TriggerReveal()
    {
        animator.SetTrigger("Reveal");
    }

    public void TriggerBurry()
    {
        animator.SetTrigger("Burry");
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TriggerShootEggSack()
    {
        animator.SetTrigger("ShootEggSack");
    }

    public void SetVaulnerableBool(bool vaulnerable)
    {
        animator.SetBool("Vaulnerable", vaulnerable);
    }

    public void SpawnEggSack()
    {
        spiderMother.SpawnEggSack();
    }

    public void ColliderToggle()
    {
        spiderMother.ToggleColliders();
    }

    public void MoveToChaseState()
    {
        spiderMother.SetState(SpiderState.ChasePlayer);
    }

    public void AttackToVulnerable()
    {
        spiderMother.SetState(SpiderState.Vulnerable);

    }
}
