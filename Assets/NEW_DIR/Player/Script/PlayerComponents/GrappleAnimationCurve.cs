using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAnimationCurve : MonoBehaviour
{

   public Transform throwPosition;


    Vector3 startPosition; 

    public Transform parent; 

    public GameObject hook;

    public AnimationCurve animCurve; 
    public AnimationClip animClip;
    float startTime = 0;

    float startValueY;
    float startValueZ;
    float startValueX;


    float endTime = 0.1f;

    public Vector3 endValueVector; 
    float endValueY; 
    float endValueZ;
    float endValueX;



    string objectName = ""; // Leave empty if the target object is the one with the animator component


    // Start is called before the first frame update
    void Start()
    {
        hook = this.gameObject;

        if(parent != null)
        startPosition = parent.transform.position; 

    }

    public void setHookPoint(Transform hP)
    {

        endValueVector = hP.position;

    }

    public void DetatchHook(bool value)
    {
        if(value)
        hook.transform.parent = null;

        else
        {
            hook.transform.parent = parent;
            transform.position = startPosition; 
        }
    }

    public void AnimateHook()
    {
        transform.position = throwPosition.position;

        startValueY = throwPosition.transform.position.y;
        endValueY = endValueVector.y;

        AnimationCurve curveY = AnimationCurve.EaseInOut(startTime, startValueY, endTime, endValueY);
        string relativeObjectName = string.Empty; // Means the object holding the animator component
        animClip.SetCurve(relativeObjectName, typeof(Transform), "localPosition.y", curveY);


        startValueZ = throwPosition.transform.position.z;
        endValueZ = endValueVector.z;

        AnimationCurve curveZ = AnimationCurve.EaseInOut(startTime, startValueZ, endTime, endValueZ);
        animClip.SetCurve(relativeObjectName, typeof(Transform), "localPosition.z", curveZ);

        startValueX = throwPosition.transform.position.x;
        endValueX = endValueVector.x;

        AnimationCurve curveX = AnimationCurve.EaseInOut(startTime, startValueX, endTime, endValueX);
        animClip.SetCurve(relativeObjectName, typeof(Transform), "localPosition.x", curveX);


        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.a))
            //animClip.
    }
}
