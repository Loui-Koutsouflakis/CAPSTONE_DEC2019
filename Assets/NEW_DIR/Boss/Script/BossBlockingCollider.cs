using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBlockingCollider : MonoBehaviour
{
    [SerializeField]
    private bool leftHand;
    [SerializeField]
    private BossV2 bossRef;

    readonly float initBlocking = 50f;

    private void Awake()
    {
        bossRef = GameObject.Find("Boss_Shell_v2").GetComponent<BossV2>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9 && !collision.collider.isTrigger)
        {
            if(leftHand)
            {
                bossRef.leftHandBlocking = initBlocking;
            }
            else
            {
                bossRef.rightHandBlocking = initBlocking;
            }
        }
    }
}
