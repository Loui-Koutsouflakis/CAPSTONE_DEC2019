using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    public Transform[] Waypoints;
    public int currentLocation = 0;
    public float p_Speed;
    public float r_Speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        if (Vector3.Distance(transform.position, Waypoints[currentLocation].transform.position) >= 1)
        {
            //float r_Step = r_Speed * Time.deltaTime;
            //Vector3 target = Waypoints[currentLocation].transform.position - transform.position;
            //Vector3 dir = Vector3.RotateTowards(transform.position, target, r_Step, 0.0f);
            //transform.rotation = Quaternion.LookRotation(dir);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(Waypoints[currentLocation].transform.position.x, transform.position.y, Waypoints[currentLocation].transform.position.z), p_Speed * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, Waypoints[currentLocation].transform.position) <= 3)
        {
            currentLocation += 1;
        }
        if (currentLocation >= Waypoints.Length)
        {
            currentLocation = 0;
        }
    }
    }
