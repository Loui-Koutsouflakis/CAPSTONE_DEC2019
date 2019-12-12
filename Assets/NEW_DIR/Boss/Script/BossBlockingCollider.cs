using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBlockingCollider : MonoBehaviour
{
    [SerializeField]
    private bool leftHand;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9 && !collision.collider.isTrigger)
        {
            if(leftHand)
            {
                
            }
            else
            {

            }
        }
    }
}
