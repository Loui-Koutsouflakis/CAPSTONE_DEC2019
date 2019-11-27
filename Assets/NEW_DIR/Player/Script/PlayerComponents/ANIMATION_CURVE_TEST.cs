using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIMATION_CURVE_TEST : MonoBehaviour
{

    public Transform posOne;
    public Transform posTwo;

    public GameObject hook;

    public Animator anim; 
    public AnimationCurve animCurve; 
    public AnimationClip animClip;
    float startTime = 0;
    float startValueY;
    float startValueZ;
    float startValueX;


    float endTime = 1f;
    float endValueY; 
    float endValueZ;
    float endValueX;



    string objectName = ""; // Leave empty if the target object is the one with the animator component


    // Start is called before the first frame update
    void Start()
    {
        hook = this.gameObject; 
       


    }

    public void AnimateHook()
    {
        transform.position = posOne.position;

        startValueY = posOne.transform.position.y;
        endValueY = posTwo.transform.position.y;

        AnimationCurve curveY = AnimationCurve.EaseInOut(startTime, startValueY, endTime, endValueY);
        string relativeObjectName = string.Empty; // Means the object holding the animator component
        animClip.SetCurve(relativeObjectName, typeof(Transform), "localPosition.y", curveY);


        startValueZ = posOne.transform.position.z;
        endValueZ = posTwo.transform.position.z;

        AnimationCurve curveZ = AnimationCurve.EaseInOut(startTime, startValueZ, endTime, endValueZ);
        animClip.SetCurve(relativeObjectName, typeof(Transform), "localPosition.z", curveZ);

        startValueX = posOne.transform.position.x;
        endValueX = posTwo.transform.position.x;

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
