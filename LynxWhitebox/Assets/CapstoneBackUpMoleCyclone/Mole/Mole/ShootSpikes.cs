using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Lilly's Scripts")]

public class ShootSpikes : MonoBehaviour
{
    public GameObject SpikeSpawn;
    public GameObject ShootSpike;

    #region Private variables
    private GameObject player;
    private List<GameObject> waves;
    private int spikesShot;
    private int numberWavesSpawn = 60;
    private bool MoleAttackAllow;
    private float AttackTimerStart;
    private float AttackTimerStart2;
    private float AttackTimer = 1;
    private float AttackTimer2 = 5;
    private float ShootSpikesTurnSpeed = 10;
    #endregion

    void Start ()
    {
        spikesShot = 0;
        waves = new List<GameObject>();
        AttackTimerStart2 = AttackTimer2;
        AttackTimerStart = AttackTimer;
        MoleAttackAllow = false;
        spikesShot = 0;
        if (waves.Count == 0)
        {
            for (int i = 0; i < numberWavesSpawn; i++)
            {
                //spawn 60 waveObjects
                waves.Add((GameObject)Instantiate(ShootSpike, SpikeSpawn.transform.position, transform.rotation, transform));
                
            }
        }
    }
	


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

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" 
            && Physics.Raycast(transform.position,transform.TransformDirection(Vector3.up), 10))
        {
            Death();
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
                if (spikesShot < waves.Count)
                {
                    AttackTimer -= Time.deltaTime;
                    waves[spikesShot].transform.rotation = gameObject.transform.rotation;
                    waves[spikesShot].SetActive(true);
                    spikesShot++;
                }
                AttackTimer2 -= Time.deltaTime;
            }
            else
            {
                spikesShot = 0;
                AttackTimer = AttackTimerStart;
                AttackTimer2 = AttackTimerStart2;
            }

        }
    }

    void Death()
    {
        gameObject.SetActive(false);
    }

}
