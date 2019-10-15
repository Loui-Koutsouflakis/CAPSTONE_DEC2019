using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMother : MonoBehaviour, IKillable
{
    [Range(0, 5)]
    public int rotationSpeed = 3;

    public bool kill;
    public float range;
    public float meleeAttackRange;
    public float shootRange;
    public float speed;
    private WaitForSecondsRealtime vulnerableTime = new WaitForSecondsRealtime(5);
    private WaitForSecondsRealtime timeBetweenSpiderlings = new WaitForSecondsRealtime(0.5f);
    private WaitForSecondsRealtime shootCoolDownTimer = new WaitForSecondsRealtime(10);


    private Vector3 destination;
    Vector2 destinationVector2;
    Vector2 tempLocationVector2;
    Rigidbody rigidBody;
    bool shootingTodds = false;

    float timeCount = 0f;
    bool rotate = false;
    Vector3 movement;
    float angle;
    Vector3 newDirection;
    private Enemy enemyScript;
    private Vector3 locationSpawned;
    public bool vulnerable = false;
    bool attacking = false;
    bool shootCoolDown = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        enemyScript = GetComponent<Enemy>();
    }

    public IEnumerator CheckHit()
    {
        yield return 0;
    }

    public IEnumerator TakeDamage(int dmg)
    {
        enemyScript.TakeDamage(dmg);
        yield return 0;
    }

    public IEnumerator Die()
    {
        enemyScript.DeathSequence();
        yield return 0;
    }

    public void SetSpawnLocation(Vector3 pos)
    {
        locationSpawned = pos;
    }

    private void Kill()
    {
        StartCoroutine(enemyScript.DeathSequence());
        this.enabled = false;
        kill = false;
    }

    IEnumerator ShootTheTods()
    {
        int numberOfSpiderlings = Random.Range(3, 6);
        for (int i = 0; i < numberOfSpiderlings; i++)
        {
            Debug.Log("Shoot spiderling " + i);
            yield return timeBetweenSpiderlings;
        }

        shootingTodds = false;
        StartCoroutine(ShootCoolDown());
    }

    void AttackPlayer()
    {
        Debug.Log("Hit player");
        Debug.Log("Play attack animation");
        attacking = false;
        vulnerable = true;
        StartCoroutine(Valnerable());
    }
    private IEnumerator Valnerable()
    {
        for (int i = 0; i < 30; i++)
        {
            //movement = transform.forward * (-speed * Time.deltaTime);
            //movement.y = 0;
            transform.position += (transform.forward * (-speed * Time.deltaTime));
            yield return new WaitForSecondsRealtime(1/60);
        }
        Debug.Log("Play valnerable animation");


        yield return new WaitForSecondsRealtime(10);

        vulnerable = false;
    }

    private IEnumerator ShootCoolDown()
    {


        yield return new WaitForSecondsRealtime(10);

        shootCoolDown = false;
    }


    public void SetDestination(string MovementPattern)
    {
        float tempMag = 0;

        if (!vulnerable)
        {
            switch (MovementPattern)
            {
                case "Wonder":
                    destination.x = Random.Range(locationSpawned.x - 20, locationSpawned.x + 20);
                    destination.y = transform.position.y;
                    destination.z = Random.Range(locationSpawned.z - 20, locationSpawned.z + 20);

                    destinationVector2 = new Vector2(destination.x, destination.z);
                    tempLocationVector2 = new Vector2(locationSpawned.x, locationSpawned.z);
                    tempMag = (destinationVector2 - tempLocationVector2).magnitude;
                    if (tempMag > range)
                    {
                        shootingTodds = false;
                        SetDestination(MovementPattern);
                    }
                    break;

                case "ChasePlayer":
                    destination = enemyScript.playerTrigger.transform.position;
                    destination.y = transform.position.y;

                    destinationVector2 = new Vector2(destination.x, destination.z);
                    tempLocationVector2 = new Vector2(transform.position.x, transform.position.z);
                    tempMag = (destinationVector2 - tempLocationVector2).magnitude;

                    if (tempMag <= meleeAttackRange)
                    {
                        attacking = true;
                        AttackPlayer();
                    }
                    else if (tempMag > meleeAttackRange && tempMag > shootRange && !shootCoolDown)
                    {
                        shootCoolDown = true;
                        SetDestination("StandAndShoot");
                    }

                    break;

                case "StandAndShoot":
                    destination = enemyScript.playerTrigger.transform.position;
                    if (!shootingTodds)
                    {
                        shootingTodds = true;
                        StartCoroutine(ShootTheTods());
                    }
                    break;
            }
        }
        else
        {
            destination = transform.position;
        }
        destinationVector2 *= 0;
        tempLocationVector2 *= 0;
    }

    void Update()
    {
        if (rotationSpeed > 5)
        {
            rotationSpeed = 5;
            Debug.Log("Rotation speed to high. Rotation speed was set to 5.");
        }

        if (kill) Kill();

        if (!enemyScript.seesPlayer && (transform.position - destination).magnitude < 5)
        {
            SetDestination("Wonder");
        }

        if (enemyScript.seesPlayer)
        {
            SetDestination("ChasePlayer");
        }
    }

    private void FixedUpdate()
    {
        if (!vulnerable && !attacking)
        {
            if (!shootingTodds)
            {
                //movement = transform.forward * (speed * Time.deltaTime);
                //movement.y = 0;
                transform.position += (transform.forward * (speed * Time.deltaTime));
            }

            newDirection = destination - rigidBody.position;
            newDirection.y = transform.forward.y;
            angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

            if (angle > 5) transform.Rotate(transform.up, -rotationSpeed);
            if (angle < -5) transform.Rotate(transform.up, rotationSpeed);
        }
    }
}
