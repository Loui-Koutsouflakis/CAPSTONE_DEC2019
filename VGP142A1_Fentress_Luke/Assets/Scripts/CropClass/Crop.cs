using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop: MonoBehaviour 
{

    public float health;
    public float points;

    public bool triggered; 


    public bool crowEating; 
    private bool eaten;

    public GameObject tag1;
    public GameObject tag2;
    public GameObject tag3;
    public GameObject tag4;

    public GameObject alert; 

    public List<GameObject> cropTag;
    public GameObject[] cropArray; 

    public GameObject player;
    //private float pScore; 

    public GameObject Gopher;
    public bool canGopherAttack;
    public bool gopherAttacking; 


    void Start()
    {
        // pScore = player.GetComponent<Score>().playerScore;
       
    }

        // Update is called once per frame
        void Update()
    {
        IsEaten();
        if(crowEating)
        {
            Debug.Log("crow eating");
        }
        if(eaten)
        {
            Debug.Log("I've been eaten");
            //pScore = pScore - points; 
        }

    }


    bool IsEaten()
    {

        if(crowEating)
        {
            Debug.Log(GetComponent<Score>().playerScore);
            health = health - Time.deltaTime;
            Debug.Log(health);
            //pScore = pScore - health; 
            if(health < 0)
            {
                return eaten = true;
            }
        }

        return false; 
    }
}
