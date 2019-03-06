using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    //Must have an object with a trigger collider, and which has a tag set to 'Talkative' for this script to function.

    //When touching such an object, press 'e' (or whatever is set) to talk.

    private bool canTalk = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //checks if a person is touching something which can talk, and if they are pressing the talk button.
        if (Input.GetKeyDown(KeyCode.E))//change KeyCode.E to KeyCode.whatever to change talk button.
            if (canTalk)
            {
                Talk();
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Talkative"))
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Talkative"))
        {
            canTalk = false;
        }
    }

    void Talk()
    {
        Debug.Log("Talking");
    }

}
