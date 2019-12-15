using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckpoint : MonoBehaviour
{
    public static BossRespawnHandler respawner;
    public Transform spawnPoint;

    private void Start()
    {
        respawner = GameObject.Find("BossRespawnHandler").GetComponent<BossRespawnHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9 && !other.isTrigger)
        {
            respawner.respawn = spawnPoint.position;
        }
    }
}
