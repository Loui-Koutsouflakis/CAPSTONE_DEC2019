using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpin : MonoBehaviour
{
    public List<GameObject> r_Rings; //The different rings in Cyclone Attack Mode. Top ring to bottom ring is put in 0 to r_Ring.count
    public float r_RotationSpeed;
    public float r_Friction; //smaller value = less friction suggested to keep at one
    public float r_Smoothness; //try adjusting this first

    private float r_SpeedMultilpier;
    private float r_RotationValue;
    private Quaternion r_RotateFrom;
    private Quaternion r_RotateTo;

	void Update ()
    {
        RingSpinning();
	}

    //Deals with the spinning rings.
    void RingSpinning()
    {
        for (int i = 0; i < r_Rings.Count; i++)
        {
            r_RotationValue += r_RotationSpeed * r_Friction;
            r_RotateFrom = r_Rings[i].transform.rotation;
            r_RotateTo = Quaternion.Euler(0, r_RotationValue, 0);

            r_Rings[i].transform.rotation = Quaternion.Lerp(r_RotateFrom, r_RotateTo, Time.deltaTime * r_Smoothness);
        }

    }
}
