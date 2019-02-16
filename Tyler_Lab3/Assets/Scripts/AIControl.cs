using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{

    //public Animator AIController;
    public GameObject alertObject;
    public float MoveSpeed;
    public float detectionRange = 10;
    public Transform Player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TowardsPlayer();
        //Detected();
    }
    void TowardsPlayer()
    {

        transform.LookAt(Player);
        if (Player != null)
        {
            if (Vector3.Distance(transform.position, Player.position) <= detectionRange)
            {

                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(collision.gameObject,.5f);
        }
    }
}
