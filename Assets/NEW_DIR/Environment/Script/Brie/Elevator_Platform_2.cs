using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator_Platform_2 : MonoBehaviour
{
    public GameObject platform;
    public GameObject bridge;

    Animator anim;
    Animator anim2;
    void Start()
    {
        anim = platform.GetComponent<Animator>();
        anim2 = bridge.GetComponent<Animator>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.layer == 14)
        {
            anim.SetTrigger("Elevate2");
            anim2.SetTrigger("BridgeFall");
        }
    }


}
