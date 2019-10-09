// Created by Loui Koutsouflakis:   2019-07-19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public Rigidbody rb;
    public Collider[] colliders;
    public MeshRenderer[] renderers;

    public bool alive; // currently not handling any conditional blocks, but may end up being useful
    public bool seesPlayer;
    public bool canTakeDamage;
    public int hitPoints;
    public float timeUntilDespawn;
    public float timeUntilDamagable;

    public Collider playerTrigger;

    public void Spawn(EnemyType type)
    {
        enemyType = type;

        foreach(Collider col in colliders)
        {
            col.enabled = true;
        }

        foreach(MeshRenderer rend in renderers)
        {
            rend.enabled = true;
        }

        // Handle enemy-specific spawn procedures here
        // If we decide to use different pools for each enemy, 
        // then we will delete this block
        switch (enemyType)
        {
            case EnemyType.Shmenemy:

                hitPoints = 1; // example

                break;

            case EnemyType.Todds:

                hitPoints = 2;

                break;

            case EnemyType.Tod:
                GetComponent<SpiderMother>().enabled = true;
                GetComponent<SpiderMother>().locationSpawned = transform.position;
                hitPoints = 3;
                break;
        }

        alive = true;
        canTakeDamage = true;
    }

    public void Despawn()
    {
        alive = false;
        seesPlayer = false;
        canTakeDamage = false;

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        foreach (MeshRenderer rend in renderers)
        {
            rend.enabled = false;
        }
    }

    public void TakeDamage()
    {
        if (canTakeDamage)
        {
            if (hitPoints > 1)
            {
                StartCoroutine(DamageSequence());
            }
            else
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    public IEnumerator DeathSequence()
    {
        // Handle particles, animations, sounds, whatever timed sequence for death is needed
        // Using this method is not mandatory, you can use the Animator events for this as well
        // Just ensure that whatever you use is as consistent as possible across all enemies

        yield return new WaitForSecondsRealtime(timeUntilDespawn);

        Despawn();
    }

    public IEnumerator DamageSequence()
    {
        canTakeDamage = false;

        // Handle particles, animations, sounds, whatever timed sequence for damage is needed, same as above

        yield return new WaitForSecondsRealtime(timeUntilDamagable);

        canTakeDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Use a trigger that follows the player body tagged as PlayerRadius
        // Any enemy that enters this trigger will "see" the player
        // to be used for any enemy that actively pursues the player

        if(other.gameObject.tag == "PlayerRadius")
        {
            playerTrigger = other;
            seesPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerRadius")
        {
            playerTrigger = null;
            seesPlayer = false;
        }
    }
}
