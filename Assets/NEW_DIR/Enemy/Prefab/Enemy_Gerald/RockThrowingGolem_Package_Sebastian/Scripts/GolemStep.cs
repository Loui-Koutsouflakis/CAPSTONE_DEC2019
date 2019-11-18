using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemStep : MonoBehaviour
{
    public Transform root;

    bool isStepping;
    float currentStepLerp = 6f;
    Vector3 currentStepDestination;

    void Update()
    {
        if (isStepping)
        {
            root.position = Vector3.Lerp(root.position, currentStepDestination, currentStepLerp * Time.deltaTime);
        }
    }

    public void Step(float stepAmount)
    {
        currentStepDestination = root.position + (root.forward * stepAmount);
        isStepping = true;
        //yield return new WaitForSeconds(duration);
        //isStepping = false;
    }

    public void StopStepping()
    {
        isStepping = false;
    }
}
