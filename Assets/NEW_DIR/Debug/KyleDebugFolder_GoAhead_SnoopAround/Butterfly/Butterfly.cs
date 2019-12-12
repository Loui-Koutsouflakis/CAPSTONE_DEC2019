using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{

    public Transform[] followPoint;

    // Start is called before the first frame update
    float timer;
    Vector3 smoothing;
    Transform target;
    float sec;

    private void Start()
    {
        target = ResetTarget();
        sec = ResetSec();
    }

    // Update is called once per frame
    void Update()
    {
       // timer += Time.deltaTime;
        if (Vector3.Distance(transform.position,target.position) < .5f)
        {
            target = ResetTarget();
            //sec = ResetSec();
        }
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref smoothing, 3);
       // transform.rotation = transform.LookAt(target.p);
       // transform.position = Vector3.MoveTowards(transform.position,followPoint.position, 2 * Time.smoothDeltaTime);
    }
    Transform ResetTarget()
    {
        return followPoint[Random.Range(0,followPoint.Length)];
    }
    int ResetSec()
    {
        timer = 0;
        return Random.Range(4, 6);
    }

}
