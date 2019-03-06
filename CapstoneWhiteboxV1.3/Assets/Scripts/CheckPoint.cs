using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public bool hasEntered; 
    public Transform cBirdTarget;
	// Use this for initialization
	void Start () {

        hasEntered = false; 

        if (!cBirdTarget)
        {
            //I hate this and know there's a way to automatically populate this. 
            Debug.LogError("No cBirdTarget");
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    //So this function checks a couple things. 
    //1. It tracks whether the player has hit the checkpoint
    //2. It sets a bool so that the player after hitting the checkpoint and logging it in the checkPointCount, doesn't keep logging if the player hits in repeatedly
    //3. It adds to checkPointCount which the bird uses for its guidence system. 


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player hit checkpoint");
        
                if(!hasEntered)
                {
                    if(other.GetComponent<playerCheckPoint>())
                     {
                        other.GetComponent<playerCheckPoint>().checkPointCount++;
                        hasEntered = true; 
                     }
                    else
                         {
                         Debug.Log("No playerCheckPoint script on player");
                         }


                }
                else
                {
                Debug.Log("Player has already hit checkpoint");
                }
        }
        else
        {
            Debug.Log("Don't recognize what hit the checkpoint");
        }
    }
}
