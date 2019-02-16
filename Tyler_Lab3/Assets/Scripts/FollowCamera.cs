using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowCamera : MonoBehaviour {
    public Transform Target;
    private Vector3 Zoom;

    [Range(0.01f,1.0f)]
    public float CamSmooth = 0.5f;

    public bool LookAtPlayer = false;
	// Use this for initialization
	void Start () {
        Zoom = transform.position - Target.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Target != null)
        {
            Vector3 newPos = Target.position + Zoom;


            transform.position = Vector3.Slerp(transform.position, newPos, CamSmooth);

            if (LookAtPlayer == true)
            {
                transform.LookAt(Target);
            }
        }
	}
}
