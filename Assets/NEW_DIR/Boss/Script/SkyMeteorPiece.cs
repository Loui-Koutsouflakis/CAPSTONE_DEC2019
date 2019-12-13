using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyMeteorPiece : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] pieces;

    [SerializeField]
    private MeshCollider[] pieceColliders;

    [SerializeField]
    private CapsuleCollider capsuleTrig;

    [SerializeField]
    private BossV2 boss;

    const string rammerName = "Rammer";

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == rammerName)
        {
            boss.health--;

            capsuleTrig.enabled = false;

            foreach(MeshCollider col in pieceColliders)
            {
                col.enabled = true;
            }

            foreach(Rigidbody rb in pieces)
            {
                rb.isKinematic = false;
            }
        }
    }
}
