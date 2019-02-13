using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InVisual : MonoBehaviour
{
    public GameObject enemy;
    public float fieldOfViewAngle = 90.0f;
 
    private SphereCollider col;
    private bool ghostInSight;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<SphereCollider>();
        ghostInSight = false;
    }

    void OnTriggerStay(Collider other)
    {   
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            ghostInSight = false;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            //this is what finds the angle to see if within cone.
            if (angle < fieldOfViewAngle * 0.5f)
            {
                ghostInSight = true;
            }
            else
            {
                ghostInSight = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameObject.FindGameObjectWithTag("Ghost"))
        {
            ghostInSight = false;
        }
    }
    public bool GetGhostInSight()
    {
        return ghostInSight;
    }
}
