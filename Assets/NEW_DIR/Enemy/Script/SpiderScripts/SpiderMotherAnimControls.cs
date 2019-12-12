using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMotherAnimControls : MonoBehaviour
{
    public AudioClip footFall;

    private Animator animator;
    private MotherSpider spiderMother;
    private new AudioSource audio;


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
        audio = GetComponent<AudioSource>();
        audio.clip = footFall;
    }

    public void StepSound()
    {

        audio.pitch = Random.Range(0.5f, 1.5f);
        audio.Play();
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

    public void Attack()
    {
        spiderMother.gameObject.layer = 10;
    }

    public void TriggerShootEggSack()
    {
        animator.SetTrigger("ShootEggSack");
    }

    public void TriggerHit()
    {
        animator.SetTrigger("Hit");
        if(spiderMother.hitPoints >= 1) StartCoroutine(StopVulnerable());
    }

    public IEnumerator StopVulnerable()
    {
        yield return new WaitForSecondsRealtime(1f);
        SetVaulnerableBool(false);
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
        spiderMother.SetState(SpiderMotherState.ChasePlayer);
    }

    public void AttackToVulnerable()
    {
        //SetVaulnerableBool(true);
        //StartCoroutine(spiderMother.VulnerableTimer());
        spiderMother.SetState(SpiderMotherState.Vulnerable);

    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Death");
    }

    public void Deactivate()
    {
        StartCoroutine(spiderMother.DeactivateSpider());
    }

    public void SetToHide()
    {
        if(spiderMother.currentState != SpiderMotherState.Hiding)
        {
            spiderMother.SetState(SpiderMotherState.Hiding);
        }
    }
}
