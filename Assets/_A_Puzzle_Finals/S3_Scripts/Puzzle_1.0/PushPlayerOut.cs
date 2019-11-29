//Created by Dylan LeClair 11/27/19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayerOut : MonoBehaviour
{
    private PlayerClass playerScript;
    private Collider platformCollider;
    private MultiPurposePlatform platformScript;

    private RaycastHit checkForPlayer;
    private LayerMask player = 1 << 14;

    private bool hasBeenDamaged;
    private float distance = 1.6f;
    private float timer = 0.5f;

    private void Awake()
    {
        playerScript = FindObjectOfType<PlayerClass>();
        platformCollider = GetComponent<Collider>();
        platformScript = GetComponent<MultiPurposePlatform>();
    }

    private void Update()
    {
        if (Physics.BoxCast(platformCollider.bounds.center, transform.localScale, transform.TransformDirection(Vector3.down), Quaternion.identity, 
            distance, player, QueryTriggerInteraction.UseGlobal))
        {
            playerScript.DisableControls();
            playerScript.GenericAddForce(transform.forward, 1.0f);

            if (!hasBeenDamaged && !platformScript.GetDirection())
            {
                playerScript.SetHealth(-1);
                hasBeenDamaged = true;
            }

            StartCoroutine(EnableControls());
        }
    }

    private IEnumerator EnableControls()
    {
        yield return new WaitForSeconds(timer);

        playerScript.EnableControls();
        hasBeenDamaged = false;
    }
}
