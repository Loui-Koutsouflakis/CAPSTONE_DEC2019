using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bird : MonoBehaviour
{

    public GameObject player;

    //where the bird is heading for in idle state
    public GameObject birdTarget;
    public bool isIdle;

    //how often the bird switches targets in idle state
    private float targetTimer;
    public bool canPickTarget;


    //isLooking
    //gameObject[] allCollectables


    [Header("List of CheckPoints. Populate in order!!!")]
    public List<GameObject> CheckPointList;

    //this tells the bird which checkpoint to go to 
    private GameObject currentCheckPoint;

    //super important, it keeps track of the player's position in the level
    public int playerCheckCount; 


    //AI stuff
    public float birdOffsetHeight;
    NavMeshAgent AIBird;


    //Swing stuff - now obsolete
    public bool canSwing; 
    public GameObject swingPoint;
    public bool shittyWayToDoThis;


    //Collecting
    public GameObject playerRadious; 
    public bool canCollect;
    private GameObject collectable;
    //public bool isTethered;
    public Queue <Vector3> collectQueue; 





    // Start is called before the first frame update
    void Start()
    {
        collectQueue = new Queue<Vector3>();
        isTethered = false; 
        shittyWayToDoThis = true; 
        isIdle = true; 
        targetTimer = 0;
        canPickTarget = true;
        canSwing = false; 

        //allCollectables = new GameObject[] GameObject.FindObjectWithTheTag("Collectable")


        if(!player)
        {
            Debug.Log("You forgot to add a player");
        }
        else
        {
            playerCheckCount = player.GetComponent<playerCheckPoint>().checkPointCount; 
        }


        if(GetComponent<NavMeshAgent>())
        {
            AIBird = GetComponent<NavMeshAgent>();
        }
        else
        {
            Debug.Log("You forgot to add a navmesh agent");
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        //this is when the bird is in the idle state. It will first pick a target around the player (automatically, no need to set it up in the editor).
        PickBirdTarget();

        //After a randomly elapsed time the bird will pick another target. 
        TargetTimerReset();

        //Then it will follow the target. 
        MoveToRotatingWayPoint();

        //Right now this tracks the isIdle state so that the bird can move to it's guidence system. Orginally the stuff below was in it, 
        //but it wasn't working as intended. 
        MoveToNextCheckPoint();

        //On trigger with Player's BirdRangeTrigger collider, the bird automatically heads to the target object and picks it up 
        FindStuff();

        //Temporary way for the bird to fly to a child of the player (swing target), which the player swings from. Now obsolete.
        //MoveToSwingPoint();


        if (Input.GetButtonDown("LeftBumper"))
        {
            if(collectable.GetComponent<Collectable>().isTethered)
            {
                collectable.GetComponent<Collectable>().isTethered = false;
            }
            else{
            isIdle = false;
            }
        }
     



       
    }

    private void FixedUpdate()
    {
        AIBird.speed = player.GetComponent<Rigidbody>().velocity.magnitude + 5;

    }


    void TargetTimerReset()
    {
        if(!canPickTarget)
        {
            int randomTargetTime = Random.Range(5, 10);
            targetTimer += Time.deltaTime;
            if(targetTimer > randomTargetTime)
            {
                canPickTarget = true;
                targetTimer = 0; 
            }
            
        }

    }


    void PickBirdTarget()
    {
        if(canPickTarget)
        {
            GameObject[] birdTargetArray = GameObject.FindGameObjectsWithTag("BirdTarget");
            int birdTargetChoice = Random.Range(0, birdTargetArray.Length);
            birdTarget = birdTargetArray[birdTargetChoice];
            canPickTarget = false; 


         }
    }

    void MoveToRotatingWayPoint()
    {
        if(isIdle)
        {
        birdOffsetHeight = birdTarget.transform.position.y;
        AIBird.baseOffset = birdOffsetHeight * 5;
        AIBird.SetDestination(birdTarget.transform.position);
        }



    }


    void MoveToNextCheckPoint()
    {
       
       
            if (!isIdle && !canCollect)
            {
                if (CheckPointList.Count > 0)
                {
                    playerCheckCount = player.GetComponent<playerCheckPoint>().checkPointCount;

                    //why? I don't know. I hate lists. 
                    GameObject[] checkPointArray = CheckPointList.ToArray();


                    //this is super important. It relates to how many times the player has hit the checkpoint
                    if (playerCheckCount < checkPointArray.Length)
                    {
                        currentCheckPoint = checkPointArray[playerCheckCount];

                        Vector3 checkPointPos = currentCheckPoint.GetComponent<CheckPoint>().cBirdTarget.position;


                        AIBird.SetDestination(checkPointPos);

                        Vector3 distanceToCheckPoint = checkPointPos - transform.position;


                        if (distanceToCheckPoint.magnitude < 2)
                        {
                            Debug.Log("Reached checkpoint");

                            PickBirdTarget();
                            isIdle = true;

                        }
                    }
                    else
                    {
                        Debug.Log("Level end");
                        isIdle = true;
                        PickBirdTarget();
                    }


                    //this was the part that wasn't working out of update. It would just check it once. 

                }


        }


    }

    //If anyone wants to play around with this, you have to assign a gameobject to be the swingPoint
    void MoveToSwingPoint()
    {

        //if (canSwing == true)
        //{
        //    if(shittyWayToDoThis)
        //    {

        //    Debug.Log("Heading to swing point");
        //    //float swingHeight = swingPoint.transform.position.y;
        //    //AIBird.baseOffset = swingHeight;
        //    //AIBird.SetDestination(swingPoint.transform.position);
        //    AIBird.enabled = false;
        //    float step = 10 * Time.deltaTime;
        //    transform.position = Vector3.MoveTowards(transform.position, swingPoint.transform.position, step);
        //        Vector3 newPos = transform.position - swingPoint.transform.position;
        //        if (newPos.magnitude < 2)
        //        {
        //            shittyWayToDoThis = false;
        //        }


        //    }

        //}
    }


    //Alright, so there's potentially superflous stuff in here. Specifically, though I'm using a queue, 
    //I'm not sure if it's really necessary. Previously the bird had a isTethered bool, but all  objects within
    //the player's radious would be collected. Now I have it where the collectable has a isTethered bool and things
    //seem to be working great. Obviously this feature is going to be emphasized, and what I have right now is the first
    //go of it. 

    void FindStuff()
    {
        canCollect = playerRadious.GetComponent<RangeTrigger>().canCollect;

        if (canCollect)
        {
            isIdle = false;
            collectable = playerRadious.GetComponent<RangeTrigger>().thing;
            if(!collectQueue.Contains(collectable.transform.position))
            {
                collectQueue.Enqueue(collectable.transform.position);
            }
            Debug.Log("Queue length is" + collectQueue.Count);
            AIBird.SetDestination(collectable.transform.position);
            Vector3 distanceToCollectable = transform.position - collectable.transform.position;
            if(distanceToCollectable.magnitude < 2)
            {
                collectable.GetComponent<Collectable>().isTethered = true; 
                Debug.Log("Reached collectable perch");
                collectQueue.Dequeue();
                canCollect = false;
                playerRadious.GetComponent<RangeTrigger>().canCollect = false;
                isIdle = true;
                canPickTarget = false; 
            }
        }


    }


}


    
