using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public float springRadious; 
    public float springForce;
    public float springConstant;
    public float damperForce;
    public float damperConstant;
    public float restLength;
    public LayerMask springLM;
    public GameObject springEnd; 


    private float previousLength;
    private float currentLength;
    private float springVelocity;

    public Rigidbody scarebody;

    // Start is called before the first frame update
    void Start()
    {
        scarebody = GetComponentInParent<Rigidbody>();
        springRadious = (springEnd.transform.position.y - transform.position.y) * -1; 
        
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        springConstant = scarebody.mass * 15;
        damperConstant = springForce / 10;

        int layermask = 1 << 9;
        springLM = ~layermask; 
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, restLength + springRadious, springLM))
        {
            Debug.Log(hit);
            previousLength = currentLength;
            currentLength = restLength - (hit.distance - restLength);
            springVelocity = (currentLength - previousLength) / Time.fixedDeltaTime;
            springForce = springConstant * currentLength;
            damperForce = damperConstant * springVelocity;

            scarebody.AddForceAtPosition(transform.up * (springForce + damperForce), transform.position);

        }

    }


}
