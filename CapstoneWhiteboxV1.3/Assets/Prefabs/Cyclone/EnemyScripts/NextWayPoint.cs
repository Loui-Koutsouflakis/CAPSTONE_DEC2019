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
            Debug.Log("Graa");
            EnemyMovementControlles enemyMoveScript = collider.gameObject.GetComponent<EnemyMovementControlles>();
            enemyMoveScript.NextWayPoint(WayPointNext(enemyMoveScript.e_WayPoints));
        }
    }

    //used in OnTriggerEnter
    int WayPointNext(List<GameObject> wayPoints)
    {
        int randomWayPointChoice = Mathf.RoundToInt(Random.Range(0, wayPoints.Count));
        Debug.Log("teep" + randomWayPointChoice);
        while (wayPoints[randomWayPointChoice] == gameObject)
        {
            Debug.Log("meep");
            randomWayPointChoice = Mathf.RoundToInt(Random.Range(0, wayPoints.Count));
        }

        return randomWayPointChoice; 
    }
}
