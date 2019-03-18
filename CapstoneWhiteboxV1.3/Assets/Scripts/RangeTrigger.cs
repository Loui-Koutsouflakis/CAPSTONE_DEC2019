using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTrigger : MonoBehaviour {

    public GameObject thing;
    public bool canCollect;
    public Transform perch; 


    private void Start()
    {
        canCollect = false;


    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Collectable") 
        {
            Debug.Log("Collect is in radious");
            canCollect = true;
            perch = other.gameObject.transform.GetChild(0);
            thing = other.gameObject; 

            
        }

    }
}
