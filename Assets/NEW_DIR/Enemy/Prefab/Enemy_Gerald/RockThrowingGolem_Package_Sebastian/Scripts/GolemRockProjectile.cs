using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRockProjectile : MonoBehaviour
{
    public float timer;
    public Animator anim;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 0)
        {
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        //GetComponent<Animation>().Play("CINEMA_4D_Main");
        anim.SetBool("Contact!!", true);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
        anim.SetBool("Contact!!", false);
        //GetComponent<Animation>().Rewind("CINEMA_4D_Main");
    }
}
