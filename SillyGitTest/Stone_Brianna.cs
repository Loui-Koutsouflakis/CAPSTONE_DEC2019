using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone_Brianna : MonoBehaviour {

    static string FirstName;
    static string LastName;


	// Use this for initialization
	void Start ()
    {
        FirstName = "Brianna";
        LastName = "Stone";
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("This script belongs to " + FirstName + " " + LastName);
	}
}
