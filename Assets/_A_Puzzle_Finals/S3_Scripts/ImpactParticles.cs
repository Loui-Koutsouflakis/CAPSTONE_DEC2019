//Created by Dylan LeClair 12/12/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticles : MonoBehaviour
{
    //particle reference
    public ParticleSystem impact;

    //Destroy helper
    private bool hasPlayed = false;

    private void Update()
    {
        //Make sure the particle is active
        if(impact)
        {
            //Check to see if particle should be destroyed
            if(hasPlayed == true && !impact.IsAlive())
            {
                Destroy(impact);
            }
        }
    }

    public void Impacting()
    {
        //Play particle & flip destroy helper
        impact.Play();

        hasPlayed = true;
    }
}
