using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Lilly's Scripts")]

public class ShootSpikes : MonoBehaviour
{
    public GameObject SpikeSpawn;
    public GameObject ShootSpike;
    //public float ShootSpikesMoleTurnSpeed;
    public float AttackTimer;
    public float AttackTimer2;
    public float ShootSpikesTurnSpeed;

    private GameObject player;
    private int ShootSpikesShot;
    private bool MoleAttackAllow;
    private float AttackTimerStart;
    private float AttackTimerStart2;

    // Use this for initialization
    void Start ()
    {
        AttackTimerStart2 = AttackTimer2;
        AttackTimerStart = AttackTimer;
        MoleAttackAllow = false;
        ShootSpikesShot = 0;
	    	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Attack();
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            MoleAttackAllow = true;
            player = collider.gameObject;

            Vector3 lookPos = collider.gameObject.transform.position - transform.position;
            lookPos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * ShootSpikesTurnSpeed);
        }
    }


    void OnTriggerExit(Collider collider)
    {
        MoleAttackAllow = false;
    }

    void Attack()
    {
        if (MoleAttackAllow == true)
        {
            if (AttackTimer2 > 0)
            {
                if (AttackTimer > 0)
                {
                    AttackTimer -= Time.deltaTime;
                    Instantiate(ShootSpike, SpikeSpawn.transform.position, transform.rotation);
                }
                AttackTimer2 -= Time.deltaTime;
            }
            else
            {
                AttackTimer = AttackTimerStart;
                AttackTimer2 = AttackTimerStart2;
            }

        }

        //if(MoleAttackAllow == true)
        //{
        //    switch (ShootSpikesShot)
        //    {
        //        case 0:
        //            Instantiate(ShootSpike, SpikeSpawn.transform.position, transform.rotation );
        //            ShootSpikesShot++;
        //            break;

        //        case 1:
        //            Vector3 temp = SpikeSpawn.transform.position;
        //            temp.Set(temp.x, temp.y, temp.z + 0.3f);
        //            Instantiate(ShootSpike, temp, transform.rotation);
        //            ShootSpikesShot++;
        //            break;
        //        case 2:
        //            Vector3 temp2 = SpikeSpawn.transform.position;
        //            temp2.Set(temp2.x, temp2.y, temp2.z + 0.6f);
        //            Instantiate(ShootSpike, temp2, transform.rotation);
        //            ShootSpikesShot++;
        //            break;
        //        case 3:
        //            if(AttackTimer > 0)
        //            {
        //                AttackTimer -= Time.deltaTime;
        //            }
        //            else if(AttackTimer <= 0)
        //            {
        //                ShootSpikesShot = 0;
        //                AttackTimer = AttackTimerStart;
        //            }
        //            break;
        //    }       
        //}        
    }

}
