using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementControlles : MonoBehaviour
{
    public List<GameObject> e_WayPoints; //Choose how many waypoints you want and enter them here.

    private NavMeshAgent e_NavMesh; 

    void Start()
    {
        e_NavMesh = GetComponent<NavMeshAgent>();
        e_NavMesh.SetDestination(e_WayPoints[0].transform.position);
    }

    //Called from NextWayPoint to set the next waypoint enemy is to go to.
    public void NextWayPoint(int listPosition)
    {
        e_NavMesh.SetDestination(e_WayPoints[listPosition].transform.position);
    }
}
