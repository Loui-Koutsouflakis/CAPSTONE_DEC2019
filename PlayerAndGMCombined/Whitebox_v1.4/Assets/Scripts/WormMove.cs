// Created by Fatin Majumder
// Edited March 20 by Loui Koutsouflakis - Added ability to change direction back and forth.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMove : MonoBehaviour
{

    public List<Transform> Bodyparts = new List<Transform>();

    public float mindistance = 1f;
    public float speed = 5;
    public float rotationspeed = 50;
    public float turnTime = 4f;

    private float dis;

    private Transform curBodyPart, prevbodypart;

    public float frequency = 3f;
    public float magnitude = 0.5f;

    Vector3 pos, localscale;

    public bool goingForward;

    // Start is called before the first frame update
    void Start()
    {
        goingForward = true;
        StartCoroutine(ForwardAndBack());
        pos = Bodyparts[0].transform.position;
        localscale = Bodyparts[0].transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    void Move()
    {
        float curspeed = speed;

        if (goingForward)
        {
            Bodyparts[0].Translate(Bodyparts[0].forward * curspeed * Time.smoothDeltaTime, Space.World);
        }
        else
        {
            Bodyparts[0].Translate(-Bodyparts[0].forward * curspeed * Time.smoothDeltaTime, Space.World);
        }
   
        Bodyparts[0].Translate(Bodyparts[0].up * Mathf.Sin(Time.time * frequency) * magnitude, Space.World);

        for (int i = 1; i < Bodyparts.Count; i++)
        {
            curBodyPart = Bodyparts[i];
            prevbodypart = Bodyparts[i - 1];

            dis = Vector3.Distance(prevbodypart.position, curBodyPart.position);

            Vector3 newpos = prevbodypart.position;

            float T = Time.deltaTime * dis / mindistance * curspeed;

            if (T > 0.3f)
                T = 0.3f;

            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
        
            curBodyPart.transform.LookAt(prevbodypart);

        }
    }

    public IEnumerator ForwardAndBack()
    {
        yield return new WaitForSecondsRealtime(turnTime);
        goingForward = !goingForward;
        StartCoroutine(ForwardAndBack());
    }
}