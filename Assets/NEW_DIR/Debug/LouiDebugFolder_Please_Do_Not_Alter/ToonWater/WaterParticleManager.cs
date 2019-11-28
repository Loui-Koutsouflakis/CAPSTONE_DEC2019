using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticleManager : MonoBehaviour
{
    public ParticleSystem[] splashPs;
    public ParticleSystem[] dropletPs;
    //public ParticleSystem[] splashB;
    //public ParticleSystem[] splashC;
    int psIndex;
    readonly Vector3 additiveScaleM = new Vector3(0.08f, 0.08f, 0.08f);
    readonly Vector3 additiveScaleV = new Vector3(0.04f, 0.04f, 0.04f);
    readonly Vector3 baseScale = new Vector3(0.3f, 0.3f, 0.3f);

    private void Start()
    {
        if (dropletPs.Length != splashPs.Length) // || splashB.Length != splashPs.Length || splashC.Length != splashPs.Length ||
            //dropletPs.Length != splashB.Length || splashB.Length != splashC.Length || splashB.Length != dropletPs.Length)
        {
            Debug.Log("<color=red> Water Particle Arrays are not all the same length. This will throw a runtime exception if not corrected. </color>");
        }
    }

    public void Splash(Vector3 location, float mass, float velocity)
    {
        splashPs[psIndex].Stop();
        dropletPs[psIndex].Stop();
        //splashB[psIndex].Stop();
        //splashC[psIndex].Stop();
        splashPs[psIndex].transform.position = location;
        splashPs[psIndex].transform.localScale = baseScale + (additiveScaleM * mass) + (velocity * additiveScaleV);
        splashPs[psIndex].Play();
        dropletPs[psIndex].transform.position = location;
        dropletPs[psIndex].transform.localScale = baseScale + (additiveScaleM * mass) + (velocity * additiveScaleV);
        dropletPs[psIndex].Play();
        //splashB[psIndex].transform.localScale = baseScale + (additiveScaleM * mass) + (velocity * additiveScaleV);
        //splashB[psIndex].Play();
        //splashC[psIndex].transform.localScale = baseScale + (additiveScaleM * mass) + (velocity * additiveScaleV);
        //splashC[psIndex].Play();

        psIndex = psIndex >= splashPs.Length - 1 ? 0 : psIndex + 1;
    }
}
