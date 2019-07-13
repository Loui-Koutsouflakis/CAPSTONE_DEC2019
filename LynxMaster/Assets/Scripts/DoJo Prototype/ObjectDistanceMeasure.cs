//Written by Mike Elkin 07/10/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteInEditMode]
[AddComponentMenu("Mike's Scripts/MultiPurposePlatform", 16)]

public class ObjectDistanceMeasure : MonoBehaviour
{
    [SerializeField]
    Transform object_1;
    [SerializeField]
    Transform object_2;
    [SerializeField]
    bool measure;
    float distance = 0;

    

    private void Update()
    {
        if (measure)
        {
            distance = CalcDistance();
            Debug.Log("Distance = " + distance);
        }
    }

    float CalcDistance()
    {
        return Vector3.Distance(object_1.position, object_2.position);
    }

}
