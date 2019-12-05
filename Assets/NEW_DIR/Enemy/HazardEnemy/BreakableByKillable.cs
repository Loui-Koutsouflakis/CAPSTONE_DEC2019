using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableByKillable : MonoBehaviour, IKillable
{
    [SerializeField]
    private Rigidbody[] parts;

    [SerializeField]
    private Collider[] partColliders;

    [SerializeField]
    private Collider hitCollider;

    [SerializeField]
    private Animator[] anims;

    private bool alive;
    private float impulseMin = -3f;
    private float impulseMax = 3f;
    private Vector3 impulseBaton;

    private void Start()
    {
        alive = true;

        foreach(Collider col in partColliders)
        {
            col.enabled = false;
        }

        foreach(Rigidbody rb in parts)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    //
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        StartCoroutine(CheckHit(false));
    //    }
    //}
    //

    public IEnumerator CheckHit(bool isGroundPound)
    {
        yield return 0f;

        if (alive)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator TakeDamage()
    {
        yield return 0f;
    }

    public IEnumerator Die()
    {
        alive = false;
        hitCollider.enabled = false;

        foreach (Animator anim in anims)
        {
            anim.enabled = false;
        }

        foreach(Rigidbody rb in parts)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            impulseBaton.x = Random.Range(impulseMin, impulseMax);
            impulseBaton.y = Random.Range(impulseMin, impulseMax);
            impulseBaton.z = Random.Range(impulseMin, impulseMax);
            rb.AddForce(impulseBaton, ForceMode.Impulse);
            rb.angularVelocity = impulseBaton;
        }

        yield return new WaitForSeconds(0.01f);

        foreach (Collider col in partColliders)
        {
            col.enabled = true;
        }

        yield return new WaitForSeconds(2f);

        foreach (Rigidbody rb in parts)
        {
            Destroy(rb.gameObject, 4f);
        }
    }
}
