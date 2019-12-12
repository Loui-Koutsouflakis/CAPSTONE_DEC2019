using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBlockingCollider : MonoBehaviour
{
    [SerializeField]
    private bool leftHand;

    public static BossV2 bossRef;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9 && !collision.collider.isTrigger)
        {
            if(leftHand)
            {
                bossRef.leftHandBlocking = 5f;
            }
            else
            {
                bossRef.rightHandBlocking = 5f;
            }
        }
    }
}
