using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollow : MonoBehaviour
{
    public Transform player;
    public Transform queen;
    public Transform myTransform;
    public bool TargetLock = true;

    void Update()
    {
        if (TargetLock == true)
        {
            transform.LookAt(player);
            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }
        else
        {
            transform.LookAt(queen);
            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }
    }
}

