using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

//this is an example script template. 

public class Collectable : MonoBehaviour, Interact {

    //public Bird bird;
    public Material blue;
    private Color color;



  

	// Use this for initialization
	void Start () {


        // color = blue.color; 
        color = Color.gray;
        blue.color = color;

        //bird = GameObject.FindWithTag("Bird").GetComponent<Bird>();
		
	}



   

    void Interact.InteractWithMe()
    {
        Debug.Log("You can see the collectable");
        //bird.destination = transform.position;
        blue.color = Color.red; 
    }

    public void DontInteractWithMe()
    {
        Debug.Log("You can't see the collectable");
        //bird.destination = Vector3.zero;
        //bird.canPickTarget = true; 
        blue.color = color; 
        
    }







}
