using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemArm : MonoBehaviour
{

    [Range(0, 6)]
    public float maxHeight;
    [Range(0, 6)]
    public float minHeight;
    [Range(-5, 5)]
    public float range;

    public float center;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, center + Mathf.PingPong(Time.time * range, maxHeight) - maxHeight / range, transform.position.z);
    }
}
