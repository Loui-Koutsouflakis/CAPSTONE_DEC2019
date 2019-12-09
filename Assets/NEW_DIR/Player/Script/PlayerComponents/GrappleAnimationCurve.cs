using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAnimationCurve : MonoBehaviour
{

    public Transform throwPosition;

    public GameObject hook;

    //public Transform startPosition;
    public Transform endPosition;

    public List<Vector3> pointsInbetween = new List<Vector3>();

    public Transform parent;

    public float moveSpeed = 50;

    [Range(-1, 1)]
    public float upperControlPointOffset = 0.1f;

    [Range(-1, 1)]
    public float lowerControlPointOffset = -0.5f;

    Coroutine MoveGrappleHook;

    RopeController rC;


    public GameObject debugBalls;

    // Start is called before the first frame update
    void Start()
    {
        rC = FindObjectOfType<RopeController>();


        if (parent != null)
            throwPosition.position = parent.transform.position;

    }

    public void SetHookPoint(Transform hP)
    {

        endPosition = hP;

    }

    public void DetatchHook(bool value)
    {
        if (value)
            transform.parent = null;

        else
        {
            transform.parent = parent;
            transform.position = throwPosition.position;
        }
    }

    public void AnimateHook()
    {
        //transform.position = throwPosition.position;

        Vector3 A = throwPosition.position;
        Vector3 D = endPosition.position;

        Vector3 B = A + throwPosition.up * (-(A - D).magnitude * upperControlPointOffset);

        Vector3 C = D + endPosition.up * ((A - D).magnitude * lowerControlPointOffset);


        BezierCurve.GetBezierCurve(A, B, C, D, pointsInbetween);

        Vector3 finalVector = pointsInbetween[pointsInbetween.Count - 1] + new Vector3(0, -0.6f, 0);

        pointsInbetween.Add(finalVector);

        //DebugCurve();

        StartCoroutine(MoveAlongPath());
    }


    void DebugCurve()
    {

        foreach (Vector3 item in pointsInbetween)
        {
            GameObject newThing = Instantiate(debugBalls, item, Quaternion.identity);
        }
    }

    IEnumerator Moving(int currentPosition)
    {

        while (transform.position != pointsInbetween[currentPosition])
        {
            transform.position = Vector3.MoveTowards(transform.position, pointsInbetween[currentPosition], moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator MoveAlongPath()
    {
        transform.position = pointsInbetween[0];

        for (int i = 0; i < pointsInbetween.Count; i++)
        {
            MoveGrappleHook = StartCoroutine(Moving(i));
            yield return MoveGrappleHook;

        }
        rC.beTwisty = false;
    }



}
