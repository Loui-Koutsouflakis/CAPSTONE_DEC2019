using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSteerVolume : MonoBehaviour
{
    public bool canSteer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player_Lumi")
        {
            Debug.Log("Player can grapple Dragon Head");

            canSteer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player_Lumi")
        {
            Debug.Log("Player can no longer grapple Dragon Head");

            canSteer = false;
        }
    }
}
