//Created by Dylan LeClair May, 17/19

//Empty gameobject childed to the player must be created and tagged as "FollowPoint"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdIdleFollowPointTest : MonoBehaviour
{
    private Vector3 pointPosition;

    private void Start()
    {
        pointPosition = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(1.25f, 1.75f), Random.Range(-0.15f, -0.35f));
        transform.position = pointPosition;

        StartCoroutine(ChooseNewPosition());
    }

    private IEnumerator ChooseNewPosition()
    {
        yield return new WaitForSecondsRealtime(3f);

        pointPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1.5f, 2), Random.Range(-0.5f, -1));
        transform.position = pointPosition;

        StartCoroutine(ChooseNewPosition());
    }
}
