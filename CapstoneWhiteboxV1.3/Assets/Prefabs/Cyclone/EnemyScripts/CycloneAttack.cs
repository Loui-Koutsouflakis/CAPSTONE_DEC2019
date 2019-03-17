using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CycloneAttack : MonoBehaviour
{
    public GameObject c_AttackMode; //For this case use CycloneEnemyAttackMode.
    public GameObject c_WayPoints; //The "Emply gameobject" that holds all the waypoints for this enemy.
    public List<GameObject> c_Rings;
    public float c_SpeedUpTimer;
    public float c_SpeedIncrease;
    public int c_AttackDamage;


    private NavMeshAgent c_NavMesh;
    private float c_TimerStartingValue;
    private float c_NavMeshStartSpeed;
    private float c_NaveMeshStartAcc;
    private bool c_Timer;
    private bool c_AllowedAttack;
    

    void Start()
    {
        c_AllowedAttack = true;

        if (gameObject.GetComponent<NavMeshAgent>() != null)
        {
            c_NavMesh = gameObject.GetComponent<NavMeshAgent>();
            c_NavMeshStartSpeed = c_NavMesh.speed;
            c_NaveMeshStartAcc = c_NavMesh.acceleration;
        }
        c_Timer = false;
        c_TimerStartingValue = c_SpeedUpTimer;
            
    }

    void Update()
    {
        SpeedUpTimer();
        SlowBackDown();
        AllowAttack();
    }

    void AllowAttack()
    {
        if (c_Rings[0].activeSelf == false 
            && c_Rings[1].activeSelf == false 
            && c_Rings[2].activeSelf == false 
            && c_Rings[3].activeSelf == false)
        {
            c_AllowedAttack = false;
            c_AttackMode.SetActive(false);
            c_WayPoints.SetActive(true);
            c_Timer = false;
            c_SpeedUpTimer = c_TimerStartingValue;
        }
    }    



    void SpeedUpTimer()
    {
        if(c_Timer == true && c_SpeedUpTimer > 0)
        {
            c_SpeedUpTimer -= Time.deltaTime;
        }
        else if (c_Timer == true && c_SpeedUpTimer < 0 && c_NavMesh.speed < 800)
        {
            c_NavMesh.acceleration += c_SpeedIncrease;
            c_NavMesh.speed += c_SpeedIncrease;
            c_SpeedUpTimer = c_TimerStartingValue;
        }
    }

    void SlowBackDown()
    {
        if(c_NavMesh.speed > c_NavMeshStartSpeed && c_Timer == false)
        {
            c_NavMesh.speed -= Time.deltaTime * c_SpeedIncrease;
        }
        
        if(c_NavMesh.acceleration > c_NaveMeshStartAcc && c_Timer == false)
        {
            c_NavMesh.acceleration -= Time.deltaTime * c_SpeedIncrease;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && c_AllowedAttack == true)
        {
            collision.gameObject.GetComponent<HitBox>().TakeDamage(c_AttackDamage);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && c_AllowedAttack == true) 
        {
            c_AttackMode.SetActive(true);
            c_WayPoints.SetActive(false);
            c_NavMesh.SetDestination(collider.transform.position);
            c_Timer = true;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && c_AllowedAttack == true)
        {
            c_NavMesh.SetDestination(collider.transform.position);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            c_AttackMode.SetActive(false);
            c_WayPoints.SetActive(true);
            c_Timer = false;
            c_SpeedUpTimer = c_TimerStartingValue;
            c_NavMesh.SetDestination(gameObject.GetComponent<EnemyMovementControlles>().e_WayPoints[0].transform.position);
        }
    }

}
