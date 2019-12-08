using System.Collections;
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

    
    float leftHandBlocking;
    float rightHandBlocking;

    float handLerpRate;

    Vector3 leftHandBlockingPoint;
    Vector3 rightHandBlockingPoint;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
