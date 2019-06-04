using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Obstacle : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField, Range(1, 10)]
    float attackForce = 1.0f;
    [SerializeField, Range(1, 10)]
    float rotSpeed = 1.0f;
    [SerializeField, Range(1, 50)]
    float approachRange = 1.0f;
    [SerializeField, Range(1, 10)]
    float fallDelay = 1.0f;
    [SerializeField, Range(0, 10)]
    int smashRadius = 1;
    [SerializeField]
    float playerDistance;
    [SerializeField]
    bool functionStart;
    [SerializeField]
    float timeStart;
    [SerializeField]
    GameObject playerRef;

    Collider[] targetColliders;
    


    bool mySwitch;
    int moveCount = 0;
    

    private void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerRef == null)
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }
        FindPlayerDistance();
        CheckForPlayer();
    }

    private void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Surface")
        {
            // beakapart animation here
            Debug.Log("Breaks apart");
            targetColliders = Physics.OverlapSphere(transform.position, smashRadius);
            for (int i = 0; i < targetColliders.Length; i++)
            {
                if(targetColliders[i].gameObject.tag == "Player")
                {

                }
            }

            gameObject.SetActive(false);
        }
        if (c.gameObject.tag == "Player")
        {
            c.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            c.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - c.transform.position).normalized * attackForce, ForceMode.Impulse);
        }
    }

    void FindPlayerDistance()
    {
        playerDistance = (transform.position - playerRef.transform.position).magnitude;
    }


    void CheckForPlayer()
    {
        if(playerDistance <= approachRange)
        {
            if(functionStart == false)
            {
                timeStart = Time.time;
                functionStart = true;
            }
            //fill in for animation
            if (mySwitch)
            {
                transform.Rotate(0, 0, 45 * Time.deltaTime);
                moveCount++;
                if (moveCount > 5)
                {
                    mySwitch = !mySwitch;
                    moveCount = 0;
                }
            }
            else
            {
                transform.Rotate(0,0,-45 * Time.deltaTime);
                moveCount++;
                if (moveCount > 5)
                {
                    mySwitch = !mySwitch;
                    moveCount = 0;
                }
            }





            if (Time.time - timeStart >= fallDelay)// to be but at the end of an animation of a wiggle
            {
                rb.useGravity = true;
                rb.AddForce(Vector3.down * attackForce, ForceMode.Impulse);
            }
        }
    }


}
