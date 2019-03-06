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
    private float birdOffsetHeight;
    NavMeshAgent AIBird;


  

  






    // Start is called before the first frame update
    void Start()
    {

        isIdle = true; 
        targetTimer = 0;
        canPickTarget = true;

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


        //this is in update because I'm being stupid and can't the bird to update it's checkpoint position (see above). 
        if(!isIdle)
        {
        if (CheckPointList.Count > 0)
        {
                playerCheckCount = player.GetComponent<playerCheckPoint>().checkPointCount;

                //why? I don't know. I hate lists. 
                GameObject[] checkPointArray = CheckPointList.ToArray();

                
                
                //this is super important. It relates to how many times the player has hit the checkpoint
                if(playerCheckCount < checkPointArray.Length)
                {
                  currentCheckPoint = checkPointArray[playerCheckCount];

                    Vector3 checkPointPos = currentCheckPoint.GetComponent<CheckPoint>().cBirdTarget.position;


                    AIBird.SetDestination(checkPointPos);

                    Vector3 distanceToCheckPoint = checkPointPos - transform.position;


                    if (distanceToCheckPoint.magnitude < 2)
                    {
                        Debug.Log("Reached checkpoint");
                        isIdle = true;
                        PickBirdTarget();
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

        else
            Debug.LogError("No checkpoints in list");
        }
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
        AIBird.baseOffset = birdOffsetHeight + 5;
        AIBird.SetDestination(birdTarget.transform.position);
        }



    }


    void MoveToNextCheckPoint()
    {
        if (Input.GetButtonDown("LeftBumper"))
        {
            isIdle = false;
        }

           
     }

    }

    
