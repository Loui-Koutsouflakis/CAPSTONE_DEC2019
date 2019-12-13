//Created by Dylan LeClair 12/12/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticles : MonoBehaviour
{
    //particle reference
    public ParticleSystem impact;

    public void Impacting()
    {
        //Play particle
        impact.Play();
    }
}
