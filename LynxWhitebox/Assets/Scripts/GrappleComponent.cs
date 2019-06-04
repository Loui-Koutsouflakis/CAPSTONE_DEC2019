using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleComponent : MonoBehaviour
{

    private Rigidbody body;
    public Transform tetherPoint;
    bool tether;
    public float maxSwingSpeed; 
    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;
    public float launchSpeed;


    private GrappleComponent grapple;
    private PlayerMovementv2 normalMove;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        tether = false;

        normalMove = GetComponent<PlayerMovementv2>();
        

        grapple = GetComponent<GrappleComponent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        jump();
        moving();



        if(Input.GetKeyDown(KeyCode.F))
        {
            //Vector3 radial = (tetherPoint.position - transform.position).normalized;
            //Vector3 radialVelocityComponent = Vector3.Dot(body.velocity, radial) * radial;
            //body.velocity -= radialVelocityComponent;
            //body.velocity = body.velocity.normalized * Mathf.Min(body.velocity.magnitude, maxSwingSpeed);
            tether = true;
            body.mass = 5; 
            

        }


        if(tether)
        {
            Vector3 radial = (tetherPoint.position - transform.position).normalized;
            Vector3 radialVelocityComponent = Vector3.Dot(body.velocity, radial) * radial;
            body.velocity -= radialVelocityComponent;
            body.velocity = body.velocity.normalized * Mathf.Min(body.velocity.magnitude, maxSwingSpeed);

            Vector3 tetherDirection = tetherPoint.position - transform.position;
            float tetherLength  = tetherDirection.magnitude;
            float x = Vector3.Dot(tetherDirection.normalized, Vector3.up);
            float y = (tetherDirection.normalized - x * Vector3.up).magnitude;
            float theta = Mathf.Atan2(y, x);
            float tension = body.mass * 9.8f * Mathf.Cos(theta) + body.mass * Mathf.Pow(body.velocity.magnitude, 2)/tetherLength;
            body.AddForce(tension * tetherDirection.normalized);
            Debug.DrawLine(transform.position, tetherPoint.position);


        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            body.mass = 1;
            body.AddForce(body.velocity.normalized * launchSpeed, ForceMode.Impulse);
            tether = false;
        }

        if (Input.GetKey(KeyCode.Y))
        {
            normalMove.enabled = true;
            grapple.enabled = false;
        }
    }


    void moving()
    {
        Vector3 horizontal = Input.GetAxis("Horizontal") * sidewaysSpeed * transform.right;
        Vector3 forward = Input.GetAxis("Vertical") * forwardSpeed * transform.forward;

        body.AddForce(horizontal + forward);

    }

    void jump()
    {

        if (Input.GetKey(KeyCode.Space))
        {

            body.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
        }

    }

    
}
