using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BirdPlatform : MonoBehaviour, Interact
{

    public Bird bird;
    public Transform startPosition;
    public Transform finalLocation;

    public float moveSpeed;
    public float maxDistance;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        bird = GameObject.FindWithTag("Bird").GetComponent<Bird>();
        startPosition.transform.position = transform.position; // set the default location
        canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)// && Vector3.Distance(bird.transform.position, startPosition.transform.position) <= 3 )
        {
            Move();
        }
        else
        {
            ResetPosition();
        }
    }

    void Interact.InteractWithMe()
    {
        Debug.Log("You can see the platform");
        bird.destination = transform.position;
        canMove = true;
    }

    public void DontInteractWithMe()
    {
        Debug.Log("You can't see the platform");
        bird.destination = Vector3.zero;

        //ResetPosition();
        canMove = false;
    }

    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, finalLocation.transform.position, moveSpeed * Time.deltaTime);
    }

    public void ResetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPosition.transform.position, moveSpeed * Time.deltaTime);
    }

}
