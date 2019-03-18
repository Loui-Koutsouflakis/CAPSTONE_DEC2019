using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swing : MonoBehaviour
{
    public bool amITethered;
    Rigidbody body;
    public Vector3 hitPoint;
    public float tetherLength;
    Vector3 testPosition;
    public int tethering;


    public GameObject swingPoint; 


    public float forwardSpeed = 100;
    public float sidewaysSpeed = 8;
    public float jumpSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        amITethered = false;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

   

        moving();
        jump();

       
        testPosition = body.position + body.velocity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F))
        {
            hitPoint = swingPoint.transform.position;
                amITethered = true;
                Vector3 tLength = hitPoint - body.position;
                tetherLength = tLength.magnitude;
           
        }
       


        if (amITethered)
            {
            tethered();
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

            body.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

    }

    void tethered()
    {
        Vector3 newPos = testPosition - hitPoint;
        Debug.DrawLine(transform.position, hitPoint);
      

        if (newPos.magnitude > tetherLength)
        {
            // we're past the end of our rope
            // pull the avatar back in.
            testPosition = hitPoint + newPos.normalized * (tetherLength);


        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            amITethered = false;
        }
        //Debug.Log(body.velocity);
        body.position = testPosition;

    }
}
