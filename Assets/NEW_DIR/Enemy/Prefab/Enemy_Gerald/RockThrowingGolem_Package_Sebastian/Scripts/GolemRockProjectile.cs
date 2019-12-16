using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRockProjectile : MonoBehaviour
{
    public float timer;
    public Animator anim;
    public HandleSfx SFX;
    
    private void OnCollisionEnter(Collision collision)
    {
        var CGL = collision.gameObject.layer;
        if (CGL == 9 || CGL == 0 || CGL == 16 || CGL == 10)
        {
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        SFX.PlayOneShotByName("Boom");
        anim.SetBool("Contact!!", true);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
        anim.SetBool("Contact!!", false);
        //GetComponent<Animation>().Rewind("CINEMA_4D_Main");
    }
}
