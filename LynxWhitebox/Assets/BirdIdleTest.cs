//Created by Dylan LeClair, May 17/19

//Empty gameobject childed to the player must be created and tagged as "FollowPoint"
//Player gameobject in scene must be tagged as "Player"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdIdleTest : MonoBehaviour
{
    private GameObject idleFollowPoint;
    private GameObject player;

    private readonly float speed = 6;

    private void Start()
    {
        idleFollowPoint = GameObject.FindGameObjectWithTag("FollowPoint");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        LookDirection();
        Follow();
    }

    private void LookDirection()
    {
        transform.LookAt(player.transform.position);
    }

    private void Follow()
    {
        transform.position = Vector3.MoveTowards(transform.position, idleFollowPoint.transform.position, speed * Time.deltaTime);
    }
}
