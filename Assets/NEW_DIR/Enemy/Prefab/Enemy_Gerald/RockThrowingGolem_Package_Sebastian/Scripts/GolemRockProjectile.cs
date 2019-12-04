using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRockProjectile : MonoBehaviour
{
    public float timer;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 0)
        {
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        GetComponent<Animation>().Play("CINEMA_4D_Main");
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }
}
