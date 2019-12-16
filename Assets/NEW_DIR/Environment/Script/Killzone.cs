﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    public PlayerClass playerScript;
    public Vector3 spawnPoint;
    private HudManager hudManager;
    private void Start()
    {
        hudManager = playerScript.GetHManager();
        playerScript = FindObjectOfType<PlayerClass>();
        //spawnPoint = playerScript.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 14)
        {
            spawnPoint = playerScript.GetLastKnownPos();
            playerScript.gameObject.transform.position = spawnPoint;
            playerScript.rb.velocity = Vector3.zero;
            if (playerScript.GetDamagable())
            {
                playerScript.SetHealth(-1);
                hudManager.HealthDown();
                playerScript.SetDamagable(false);
                StartCoroutine(ResetDamagable());
            }
        }
    }

    IEnumerator ResetDamagable()
    {
        yield return new WaitForSeconds(0.3f);
        playerScript.SetDamagable(true);
    }
}
