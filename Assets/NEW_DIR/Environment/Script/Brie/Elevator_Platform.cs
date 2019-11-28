// Section 1A Elevator Platform
// Created by Brie Stone on 10/24/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator_Platform : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.layer == 14)
        {
            anim.SetTrigger("Elevate");
        }
    }

}
