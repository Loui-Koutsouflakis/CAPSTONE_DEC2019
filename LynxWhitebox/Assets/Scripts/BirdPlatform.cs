using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BirdPlatform : MonoBehaviour, Interact
{

    //public Bird bird;
    public Transform startPosition;
    public Transform finalLocation;

    public float moveSpeed;
    public float maxDistance;

    public bool canMove;
    public bool move;

    // Start is called before the first frame update
    void Start()
    {
        //bird = GameObject.FindWithTag("Bird").GetComponent<Bird>();
        startPosition.transform.position = transform.position; // set the default location
        canMove = false;
        move = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("LeftBumper"))
        {
            if (canMove)// && Vector3.Distance(bird.transform.position, startPosition.transform.position) <= 3 )
            {
                move = true;
            }
            else if (!canMove)
            {
                move = false;
            }

        }
            if (move)// && Vector3.Distance(bird.transform.position, startPosition.transform.position) <= 3 )
            {
                Move();
            }
            else if (!move)
            {
                ResetPosition();
            }
    }

    void Interact.InteractWithMe()
    {
        Debug.Log("You can see the platform");
        //bird.destination = transform.position;
        canMove = true;
    }

    public void DontInteractWithMe()
    {
        Debug.Log("You can't see the platform");
       // bird.destination = Vector3.zero;
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
