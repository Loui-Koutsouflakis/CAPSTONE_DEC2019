using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformStart : MonoBehaviour
{
    public MultiPurposePlatform platform; 
    // Start is called before the first frame update
    void Start()
    {
        platform.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 14)
        {
            if (!platform.enabled)
            {
                platform.enabled = true;
            }
        }
    }
    
}
