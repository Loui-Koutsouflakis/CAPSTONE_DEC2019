//Created by Dylan LeClair 11/11/19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMovingPlatform : MonoBehaviour
{
    public GameObject movingPlatform;

    private CameraPaths cameraPaths;

    private void Start()
    {
        cameraPaths = GetComponent<CameraPaths>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            cameraPaths.StartMeUp();
            movingPlatform.SetActive(true);
        }
    }
}
