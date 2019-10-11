using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMother : MonoBehaviour
{
    public Enemy enemyScript;
    public bool kill;
    public float range;
    public float meleeAttackRange;

    public Vector3 locationSpawned;
    public float speed;
    private Vector3 destination;
    Vector2 tempV2;
    Vector2 tempLocation;
    Rigidbody rigidBody;
    float angleDifferenceForward = 0.0f;
    bool shootingTodds = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        enemyScript = GetComponent<Enemy>();

    }

    private void Kill()
    {
        StartCoroutine(enemyScript.DeathSequence());
        this.enabled = false;
        kill = false;
    }

    // Current choices "Wonder" and "ChasePlayer"
    public void SetDestination(string MovementPattern)
    {
        //if (!shootingTodds)
        //{
            if (MovementPattern == "Wonder")
            {
                destination.x = Random.Range(locationSpawned.x - 20, locationSpawned.x + 20);
                destination.y = transform.position.y;
                destination.z = Random.Range(locationSpawned.z - 20, locationSpawned.z + 20);
            }
            else if (MovementPattern == "ChasePlayer")
            {
                destination = enemyScript.playerTrigger.transform.position;
                destination.y = transform.position.y;
            }

            float tempMag = 0;

            switch (MovementPattern)
            {
                case "Wonder":
                    tempV2 = new Vector2(destination.x, destination.z);
                    tempLocation = new Vector2(locationSpawned.x, locationSpawned.z);
                    tempMag = (tempV2 - tempLocation).magnitude;
                    if (tempMag > range)
                    {
                        shootingTodds = false;
                        SetDestination(MovementPattern);
                    }
                    break;

                case "ChasePlayer":
                    tempV2 = new Vector2(destination.x, destination.z);
                    tempLocation = new Vector2(transform.position.x, transform.position.z);
                    tempMag = (tempV2 - tempLocation).magnitude;
                    if (tempMag > meleeAttackRange)
                    {
                        SetDestination("StandAndShoot");
                    }
                    break;
                case "StandAndShoot":
                    //destination = transform.position;
                    if (!shootingTodds)
                    {
                        shootingTodds = true;
                        ShootTheTods();
                    }
                    break;
            }
        //}
        tempV2 *= 0;
        tempLocation *= 0;

    }

    void ShootTheTods()
    {

        shootingTodds = false;
    }

    void Update()
    {

        if (kill) Kill();

        if ((transform.position - destination).magnitude < 5 && !enemyScript.seesPlayer)
        {
            SetDestination("Wonder");
        }

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
    }


    float timeCount = 0f;
    bool rotate = false;
    Vector3 movement;
    float angle;
    Vector3 newDirection;
    private void FixedUpdate()
    {
        if (!shootingTodds)
        {
            movement = transform.forward * (speed * Time.deltaTime);
            movement.y = 0;
            transform.position += (transform.forward * (speed * Time.deltaTime));
        }

        newDirection = destination - rigidBody.position;
        newDirection.y = transform.forward.y;
        angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

        if (angle > 5) transform.Rotate(transform.up, -1);
        if (angle < -5) transform.Rotate(transform.up, 1);
    }

}
