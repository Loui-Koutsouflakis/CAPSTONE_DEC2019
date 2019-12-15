using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Particles : MonoBehaviour
{
    public ParticleSystem particles;
    // Start is called before the first frame update
    private void OnTriggerStay(Collider c)
    {
        if(c.gameObject.layer == 14) particles.Play();
    }
    private void OnTriggerExit(Collider c)
    {
        if (c.gameObject.layer == 14) particles.Stop();
    }
}
