using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loui : MonoBehaviour {

    string myName;

	// Use this for initialization
	void Start () {
        myName = "Loui";
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("We all need a little space sometimes, " + myName + ". ");
        }
	}
}
