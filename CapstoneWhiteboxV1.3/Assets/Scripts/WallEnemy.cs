// Wall Climbing Enemy
// Created by Brianna Stone 02/23/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class WallEnemy : MonoBehaviour {

    public int maxHP = 3;
    private int currentHP;

    Vector3 newPos;

    // Testing other mechanics, ignore x & z for now
    //float x;
    //float z;

    int range = 4;
    int dest = 0;

    public GameObject player;
    //public GameObject spawn;

    private NavMeshAgent agent;
    public Transform[] wayPoints;    

	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHP = maxHP;
        NextPoint();
    }
	
	void Update ()
    {
        // Random wandering -- doesn't perform well on small area
        // Will do more with this later

        //newPos = Random.insideUnitSphere * 20;
        //agent.destination = newPos;

        // Waypoints to make enemy crawl around pillar
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            NextPoint();
        }

        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            agent.isStopped = true;
        }

        if (Vector3.Distance(transform.position, player.transform.position) > range)
        {
            agent.isStopped = false;
        }
	}

    private void FixedUpdate()
    {
        if (agent.isStopped == true)
        {
            Dispense();
        }
    }

    public void NextPoint()
    {
        if (wayPoints.Length == 0)
        {
            return;
        }

        agent.destination = wayPoints[dest].transform.position;

        dest = (dest + 1) % wayPoints.Length;
    }

    public void Dispense()
    {
        GetComponent<Shoot>().ShootProjectile();
    }

    public void KillEnemy()
    {
        currentHP--;

        if (currentHP <= 0)
        {
            Destroy(gameObject, 1.0f);
        }
    }
}
