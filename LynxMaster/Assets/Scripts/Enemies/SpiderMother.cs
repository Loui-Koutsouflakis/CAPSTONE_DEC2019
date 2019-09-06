using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMother : MonoBehaviour
{
    public Enemy enemyScript;
    public bool kill;
    public float range;
    public GameObject toddsSpawner;

    public Vector3 locationSpawned;
    public float speed;
    private Vector3 destination;
    private WaitForSecondsRealtime frameRate = new WaitForSecondsRealtime(1/60);
    Vector2 tempV2;
    Vector2 tempLocation;
    Rigidbody rigidBody;
    float angleDifferenceForward = 0.0f;
    bool shootingTodds = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        locationSpawned = transform.position;
        SetDestination("Wonder");
    }

    private void Start()
    {
        
    }
    private void Kill()
    {
        StartCoroutine(enemyScript.DeathSequence());
        this.enabled = false;
        kill = false;
    }


    void Update()
    {
        if (kill) Kill();
        //Debug.Log(GetComponent<Rigidbody>().velocity);

        if((transform.position - destination).magnitude < 1 && !enemyScript.seesPlayer)
        {
            SetDestination("Wonder");
        }
        //Debug.Log(enemyScript.playerTrigger.transform.position);
        if (enemyScript.playerTrigger != null)
        {
            if (enemyScript.seesPlayer && (locationSpawned - enemyScript.playerTrigger.transform.position).magnitude < range)
            {
                SetDestination("ChasePlayer");
            }
            else if (enemyScript.seesPlayer && (locationSpawned - enemyScript.playerTrigger.transform.position).magnitude > range)
            {
                SetDestination("StandAndShoot");
            }
        }
        Debug.Log((transform.position - destination).magnitude);   
    }



    // Current choices "Wonder" and "ChasePlayer"
    public void SetDestination(string IDontKnow)
    {
        if (!shootingTodds)
        {
            if (IDontKnow == "Wonder")
            {
                destination = new Vector3(
                    Random.Range(locationSpawned.x - 20, locationSpawned.x + 20),
                    transform.position.y,
                    Random.Range(locationSpawned.z - 20, locationSpawned.z + 20));
            }
            else if (IDontKnow == "ChasePlayer")
            {
                destination = enemyScript.playerTrigger.transform.position;
            }

            float tempMag = 0;

            if (IDontKnow != "StandAndShoot")
            {
                tempV2 = new Vector2(destination.x, destination.z);
                tempLocation = new Vector2(locationSpawned.x, locationSpawned.z);
                tempMag = (tempV2 - tempLocation).magnitude;
            }
            Debug.Log(destination);
            switch (IDontKnow)
            {
                case "Wonder":

                    if (tempMag > range)
                    {
                        SetDestination(IDontKnow);
                    }
                    break;

                case "ChasePlayer":
                    if (tempMag > range)
                    {
                        SetDestination("StandAndShoot");
                    }
                    break;
                case "StandAndShoot":
                    destination = transform.position;
                    if (!shootingTodds)
                    {
                        ShootTheTods();
                        shootingTodds = true;
                    }
                    break;
            }
        }
        tempV2 *= 0;
        tempLocation *= 0;

    }

    void ShootTheTods()
    {

        shootingTodds = false;
    }

    private void FixedUpdate()
    {
        Vector3 movement = (destination - transform.position).normalized * (speed * Time.deltaTime);

        movement.y = 0.0f;

        Vector3 targetPosition = rigidBody.position + movement;

        // Calculate new desired rotation
        Vector3 movementDirection = destination - rigidBody.position;
        movementDirection.y = 0.0f;
        movementDirection.Normalize();


        Vector3 currentFacingXZ = transform.forward;
        currentFacingXZ.y = 0.0f;

        angleDifferenceForward = Vector3.SignedAngle(movementDirection, currentFacingXZ, Vector3.up);
        Vector3 targetAngularVelocity = Vector3.zero;
        targetAngularVelocity.y = angleDifferenceForward * Mathf.Deg2Rad;

        Quaternion syncRotation = Quaternion.identity;
        syncRotation = Quaternion.LookRotation(movementDirection);

        Vector3 temp = targetPosition;
        temp.z += 0.1f;

        rigidBody.MovePosition(targetPosition);
        //}

        if (movement.sqrMagnitude > Mathf.Epsilon)
        {
            Debug.Log("Meep");
            // Currently we only update the facing of the character if there's been any movement
            rigidBody.MoveRotation(syncRotation);
        }

    }

}
