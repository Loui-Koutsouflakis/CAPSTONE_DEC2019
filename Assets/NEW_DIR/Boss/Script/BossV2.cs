﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossV2 : MonoBehaviour
{
    public Transform[] body;
    public Animator[] bodyAnims;

    public GameObject[] pathOne;
    public GameObject[] pathTwo;

    public Transform leftHand;
    public Transform rightHand;
    public Animator leftHandAnim;
    public Animator rightHandAnim;

    public bool steering;

    float leftHandBlocking;
    float rightHandBlocking;
    float handLerpRate;

    Vector3 leftHandBlockingPoint;
    Vector3 rightHandBlockingPoint;

    public Camera playerCam;
    public Camera bossCam;

    private void Start()
    {
        if(bossCam.enabled)
        {
            bossCam.enabled = false;
        }
    }

    private void Update()
    {
        if(rightHandBlocking < 0f)
        {
            rightHand.position = Vector3.Lerp(rightHand.position, rightHandBlockingPoint, handLerpRate * Time.deltaTime);
        }

        if(leftHandBlocking < 0f)
        {
            leftHand.position = Vector3.Lerp(leftHand.position, leftHandBlockingPoint, handLerpRate * Time.deltaTime);
        }

        if(steering)
        {

        }
    }

    public void CueSteer()
    {
        bossCam.enabled = true;
        playerCam.enabled = false;

        steering = true;
    }
}
