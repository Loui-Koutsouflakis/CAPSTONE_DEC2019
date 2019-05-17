// Wall Climbing Enemy
// Created by Brianna Stone 02/23/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class WallEnemy : MonoBehaviour
{

    public int maxHP = 3;
    private int currentHP;

    Vector3 newPos;

    int range = 4;
    int dest = 0;
    int lookSpeed = 3;

    public GameObject player;

    private NavMeshAgent agent;
    public Transform[] wayPoints;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHP = maxHP;
        NextPoint();
    }

    void Update()
    {
        // Reset enemy path after each path has finished
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            NextPoint();
        }

        // Detect if player is within range

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
            var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);

            Dispense();
        }
    }

    public void NextPoint()
    {
        // Use this for randomized wandering around a Navmesh Surface

        //newPos = Random.insideUnitSphere * 100;
        //agent.destination = newPos;


        // Use this for waypoint travel

        if (wayPoints.Length == 0)
        {
            return;
        }

        agent.destination = wayPoints[dest].transform.position;

        dest = (dest + 1) % wayPoints.Length;
    }

    public void Dispense()
    {
        GetComponent<ProjectileShoot>().ShootProjectile();
    }

    public void KillEnemy()
    {
        currentHP--;

        if (currentHP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
