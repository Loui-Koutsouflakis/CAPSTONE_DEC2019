using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class Collectable : MonoBehaviour {

    public GameObject bird;
    private Rigidbody body;
    public bool isTethered; 

	// Use this for initialization
	void Start () {

        if(GetComponent<Rigidbody>())
        {
            body = GetComponent<Rigidbody>();
        }
        bird = GameObject.FindWithTag("Bird");
		
	}
	
	// Update is called once per frame
	void Update () {

        if(isTethered)
        {
            body.isKinematic = true; 
            Vector3 birdVector = new Vector3(bird.transform.position.x, bird.transform.position.y - 0.2f, bird.transform.position.z - 0.2f);
            transform.position = Vector3.MoveTowards(transform.position, birdVector, bird.GetComponent<NavMeshAgent>().speed);
            gameObject.tag = "Collected";
        }
        else
        {
            body.isKinematic = false;

        }

    }
}
