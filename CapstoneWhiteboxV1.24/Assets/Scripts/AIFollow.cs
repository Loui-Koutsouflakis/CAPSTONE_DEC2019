using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollow : MonoBehaviour
{
    public Transform target;
    public Transform myTransform;

    void Update()
    {

        transform.LookAt(target);
        transform.Translate(Vector3.forward * 2 * Time.deltaTime);

    }
}
}
