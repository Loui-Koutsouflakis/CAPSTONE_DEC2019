using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kickable_Object : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField, Range(1,10)]
    float forwardForce = 1.0f;
    [SerializeField, Range(1, 10)]
    float upwardForce = 1.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Player")
        {
            if(Vector3.Angle(c.transform.forward, transform.forward) >= 90)
            {
                rb.AddForce(-(c.transform.forward * forwardForce) + -(c.transform.up * upwardForce), ForceMode.Impulse);
            }
            else
            {
                rb.AddForce((c.transform.forward * forwardForce) + (c.transform.up * upwardForce), ForceMode.Impulse);
            }


            
        }


    }


}
