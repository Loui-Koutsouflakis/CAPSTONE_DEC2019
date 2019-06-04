using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce_Platform : MonoBehaviour
{
    [SerializeField, Range(1, 30)]
    float jumpBoost = 1.0f;
    [SerializeField]
    GameObject playerRef;


    private void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
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

    }

    private void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Player")
        {
            playerRef.GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerRef.GetComponent<Rigidbody>().AddForce((playerRef.transform.position - transform.position).normalized * jumpBoost, ForceMode.Impulse);
        }
    }




}

