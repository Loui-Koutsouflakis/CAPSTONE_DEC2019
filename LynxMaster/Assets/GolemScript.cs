using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemScript : MonoBehaviour
{

    public Transform[] wayPoints;
    public Transform Player;
    public GameObject Projectile;
    public Transform ProjectileSpawn;
    GameObject bullet;

    public bool isFiring = false;
    public bool inMeleeRange = false;

    public int destPoint = 0;
    public float FireTimer = 1;
    public float throwRange;
    public float meleeRange;

    public float projectileRadiusLength = 1;

    int layerMask = 1 << 2;

    public float speed;
    public float slowSpeed;
    public float ProjectileSpeed;
    


    void Start()
    {
        transform.position = wayPoints[destPoint].transform.position;
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(gameObject.transform.position, Player.position) < throwRange) // if within rock throwing range. //  && FireTimer == 0 //
        {
            
            if(isFiring == false && inMeleeRange == false)
            {
                StartCoroutine(ThrowRock());
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Player.position.x, transform.position.y, Player.position.z), slowSpeed * Time.deltaTime);
            ProjectileSpawn.transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
            transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));

            if ((Vector3.Distance(gameObject.transform.position, Player.position) < meleeRange)) // if within melee range.
            {
                inMeleeRange = true;
                transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
                slowSpeed = 0;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(Player.position.x, transform.position.y, Player.position.z), 0 * Time.deltaTime);
                // do melee attack.
            }
            else
            {
                inMeleeRange = false;
            }
        }
        else
        {
            isFiring = false;
            slowSpeed = 0.5f;
            NextPoint();
        }



        

    }

    void Fire() // throws rock.
    {
        bullet = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * ProjectileSpeed;

    }

    IEnumerator ThrowRock()
    {
        
        isFiring = true;

        if(isFiring == true)
        {
            yield return new WaitForSeconds(FireTimer);

            if(Vector3.Distance(gameObject.transform.position, Player.position) < throwRange)
            {
                Fire();
                Debug.Log("AfterTimer");
            }
        }
        isFiring = false;
    }

    void NextPoint() // goes to next detination point.
    {
        transform.forward = wayPoints[destPoint].position - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[destPoint].transform.position, speed * Time.deltaTime);

        if (transform.position == wayPoints[destPoint].transform.position)
        {
            destPoint = (destPoint + 1) % wayPoints.Length;
        }

        if (destPoint == wayPoints.Length)
        {
            destPoint = 0;
        }
    }
}


