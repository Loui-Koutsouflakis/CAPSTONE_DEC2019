//Created by Dylan LeClair 11/11/19

//Platform gameoject reference must be set in inspector

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMovingPlatform : MonoBehaviour
{
    //Distiguish between active and inactive moving platforms
    public GameObject movingPlatform;

    //Camera path reference
    private CameraPaths cameraPaths;

    private void Start()
    {
        //Get camera path component
        cameraPaths = GetComponent<CameraPaths>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Set moving platform active & start camera path for inactive moving platform
        if (other.gameObject.layer == 14)
        {
            cameraPaths.StartMeUp();
            movingPlatform.SetActive(true);
        }
    }
}
