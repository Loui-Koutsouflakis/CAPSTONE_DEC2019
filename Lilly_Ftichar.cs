using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilly_Ftichar : MonoBehaviour {
    float timer;
	// Use this for initialization
	void Start ()
    {
        timer = 0.0f;
        Debug.Log("I'm gonna sing the doom song now.");
        
	}

    void Update()
    {
        if (timer <= 0)
        {
            Debug.Log("Doom");
            timer = 0.5f;
        }

        timer -= Time.deltaTime;
    }
}
