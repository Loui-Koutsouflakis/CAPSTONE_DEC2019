using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWayPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("next waypoint");
        if (collider.gameObject.layer == 11 && collider.gameObject.GetComponent<EnemyMovementControlles>() != null)
        {

            collider.gameObject.GetComponent<EnemyMovementControlles>().NextWayPoint 
            /*------->*/(WayPointNext(collider.gameObject.GetComponent<EnemyMovementControlles>().e_WayPoints));
        }
    }

    //used in OnTriggerEnter
    int WayPointNext(List<GameObject> wayPoints)
    {
        int randomWayPointChoice = Mathf.RoundToInt(Random.Range(0, wayPoints.Count));

        while (wayPoints[randomWayPointChoice] == gameObject)
        {
            randomWayPointChoice = Mathf.RoundToInt(Random.Range(0, wayPoints.Count));
        }

        return randomWayPointChoice; 
    }
}
