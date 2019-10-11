using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created By Sebastian Borkowski: 10/10/2018.
// Last Updated: 10/10/2019.
public class RockThrowingGolem : MonoBehaviour
{
    // TO DO LIST //
    // 1. Radius Checks [] 
    // 2. Wake Up Function [], Back To Sleep Function []
    // 3. Throw Function [], Charge Function [], Melee Function [], Tired Function []
    // 4. Death Function []
    // 5. Set animations with KEYFRAMES to call functions in this script!!


    // Projectile Section
    public GameObject Rock;
    public Transform ProjectileSpawn;

    // Player Section
    public Transform Player;
    public Vector3 AttackTarget;

    // Animation Section


    // Bool Section
    public bool isAwake = false;
    public bool isSleeping = true;

    public bool isCharging = false;
    public bool isThrowing = false;
    public bool isMeleeing = false;
    public bool isTired = false;

    public bool isDead = false;

    // Ranges Section
    public float LongRangeAttackRadius;
    public float MiddleRangeAttackRadius;
    public float CloseRangeAttackRadius;

    // Speed Section
    public float WalkSpeed;
    public float ChargeSpeed;
    public float ProjectileSpeed;

    private int AttackChoice;

    // Awake is called before Start
    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Wake Up / Throw Rock or Charge Player Radius Check.
        if (Vector3.Distance(gameObject.transform.position, Player.position) < LongRangeAttackRadius )
        {
            //WakeUp();
        }

        // Charge Player Radius Check.
        if (Vector3.Distance(gameObject.transform.position, Player.position) < MiddleRangeAttackRadius)
        {
            if (isCharging == false)
            {
                //StartCoroutine(ChargeAttack());
                Charge();
            }
        }
        else
        {
            isCharging = false;
        }

        // Melee Radius Check.
        if (Vector3.Distance(gameObject.transform.position, Player.position) < CloseRangeAttackRadius )
        {
            Melee();
        }
    }

    // Late Update is called once at the end of each frame
    void LateUpdate()
    {
        
    }

    // Wake Up Function.
    void WakeUp() // when LongRangeAttackRadius is triggered
    {
        // Start WakeUp Animation 

        // Call either ThrowAttack or ChargeAttack.
        AttackChoice = Random.Range(0, 100);

        if (AttackChoice >= 80)
        {
            // Call Charge Function.
            Charge();
        }
        else
        {
            // Call ThrowRock Function.
        }
    }

    // Rock Throw Function.
    void ThrowRock()
    {
        // Start Animation for thowing the rock.


    }

    void SpawnRock()
    {
        // Spawn Rock with desired speed  while aiming at the player.

    }

    // Charge Player Function.
    void Charge()
    {
        // Start Animation for charge.

        // Choose max distance to charge.
        AttackTarget = new Vector3(Player.position.x, transform.position.y, Player.position.z);
        StartCoroutine(ChargeAttack());
        // Keep charging Animation going until hit or until max distance is hit.

    }

    IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isCharging = true;

        if(isCharging == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, AttackTarget, ChargeSpeed * Time.deltaTime);
            transform.LookAt(AttackTarget);

        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            isCharging = false;
        }
    }

    // Melee Function.
    void Melee()
    {
        // Start Animation for melee.
        
    }

    void ActivateMeleeColliders()
    {
        // Activate Colliders.
    }

    void DeactivateMeleeColliders()
    {
        // Deactivate Colliders.
    }

    // Tired Function.
    void Tired()
    {
        // Start Animation for being Tired.

        // On KEYFRAME end of animation go back to normal.

    }

    // Go To Sleep Function.
    void Sleep()
    {
        // Choose Random position within Spawn Range to go to.

        // Once the new position is reached, Call Sleep Animation.

    }
}
