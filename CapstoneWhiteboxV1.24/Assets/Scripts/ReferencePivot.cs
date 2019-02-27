using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferencePivot : MonoBehaviour
{
    public Transform Reference;

    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, Reference.transform.rotation.eulerAngles.y, transform.rotation.z);
    }
}
