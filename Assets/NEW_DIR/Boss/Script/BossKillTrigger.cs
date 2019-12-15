using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKillTrigger : MonoBehaviour
{
    public static BossRespawnHandler respawner;

    private void Start()
    {
        respawner = GameObject.Find("BossRespawnHandler").GetComponent<BossRespawnHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && !other.isTrigger && !BossV2.steering && !BossV2.dropIsAnimating 
            && !BossV2.grabIsAnimating && !BossV2.steerAdjusting && !BossRespawnHandler.respawning)
        {
            StartCoroutine(respawner.Respawn());
        }
    }
}
