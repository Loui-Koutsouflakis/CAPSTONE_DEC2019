using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScarecrowController : MonoBehaviour
{

    public GameObject spring;

    public float sidewaysSpeed;
    public float forwardSpeed;
    public float maxSpeed; 
    public float jumpSpeed;
    public float rotateSpeed;

    private float currentSpeed;

    public float previousY;
    public float currentY;

    private int jumpTime; 
    //public PhysicMaterial pogo; 

    private Vector3 forward;
    private Vector3 horizontal; 

    private Vector3 moveDirection;

    //UI
    public Text odometer;
    public Text height;
    public Image jumpBar;
    public float jumpSize;
    public Text maxBoost;
    public Text minBoost;
    public Text noBoost;
    public Text failBoost;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        jumpTime = 0;
        maxBoost.enabled = false;
        minBoost.enabled = false;
        noBoost.enabled = false;
        failBoost.enabled = false;




        jumpSize = 10;
        if (!GetComponent<Rigidbody>())
        {
            Debug.LogError("No rigid body on scarecrow");
        }
        else
        {
            body = GetComponent<Rigidbody>();
        }
        //pogo.bounciness = 0.7f; 
    }

    // Update is called once per frame
    void Update()
    {
        //currentY = transform.position.y;


        Debug.Log("Jump Time " + jumpTime);
      

        Vector3 vel = body.velocity;
        currentSpeed = vel.magnitude;
        //float mPH = Mathf.Round(body.velocity.magnitude * 2.237f);
        float mPH = body.velocity.magnitude * 2.237f;
        odometer.text = "Current Speed is " + forwardSpeed;
        float pHeight = transform.position.y;
        height.text = "Current Height is " + pHeight;




        //previousY = transform.position.y;
    }

    private void FixedUpdate()
    {
        //odometer.text = "Current Speed is " + currentSpeed.ToString();
        height.text = "Current Height is " + transform.position.y;
        moves();
        Rotate();
        CalculateJumpLine();
       ;
    }

    void moves ()
    {

        horizontal = Input.GetAxis("Horizontal") * sidewaysSpeed * transform.right;
        forward = Input.GetAxis("Vertical") * forwardSpeed * transform.forward;
        body.AddForce(horizontal + forward);

        if(forwardSpeed < 2)
        {
            forwardSpeed = 0; 
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpTime == 0)
            {
                forwardSpeed = forwardSpeed * 0.5f;
                jumpSpeed = jumpSpeed * 0.5f;
                Debug.Log("Boost Fail");
                failBoost.enabled = true;
                maxBoost.enabled = false;
                minBoost.enabled = false;
                noBoost.enabled = false;
            }
            else if (jumpTime == 1)
            {
                forwardSpeed = forwardSpeed * 1f;
                jumpSpeed = jumpSpeed * 1f;
                Debug.Log("Boost neutral");
                failBoost.enabled = false;
                maxBoost.enabled = false;
                minBoost.enabled = false;
                noBoost.enabled = true;
            }
            else if (jumpTime == 2)
            {
                forwardSpeed = forwardSpeed * 1.1f;
                jumpSpeed = jumpSpeed * 1.1f;
                Debug.Log("Boost Minor");
                failBoost.enabled = false;
                maxBoost.enabled = false;
                minBoost.enabled = true;
                noBoost.enabled = false;
            }
            else if (jumpTime == 3)
            {
                forwardSpeed = forwardSpeed * 1.2f;
                jumpSpeed = jumpSpeed * 1.2f;
                Debug.Log("Boost Major");
                failBoost.enabled = false;
                maxBoost.enabled = true;
                minBoost.enabled = false;
                noBoost.enabled = false;
            }
        }

        }

    void Rotate()
    {


        float deltaRotation = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

        Vector3 newLookAtDirection = transform.forward + deltaRotation * transform.right;
        Vector3 newLookAtPoint = transform.position + newLookAtDirection;
        transform.LookAt(newLookAtPoint);
    }


    private void OnCollisionEnter(Collision collision)
    {
      

        if (collision.gameObject.tag == "Ground")
        {
            body.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            maxBoost.enabled = false;
            minBoost.enabled = false;
            noBoost.enabled = false;
            failBoost.enabled = false;

        }
    }

    public void CalculateJumpLine()
    {
        currentY = transform.position.y;
        float totalLength = jumpBar.transform.localScale.x;
        if (currentY < previousY)
        {
            RaycastHit hit;
            if (Physics.Raycast(spring.transform.position, spring.transform.up * -1, out hit, Mathf.Infinity))
            //Debug.DrawRay(transform.position.y, )
            {

                if (hit.collider.tag == "Ground")
                {
                    jumpBar.enabled = true;

                    float twoThirdLength = 0.2f * 0.66f;
                    float oneThirdLength = 0.2f * 0.33f;

                    //Debug.Log("Hit Ground");
                    float lengthToGround = hit.distance - hit.distance;
                    float hitLength = hit.distance;


                    jumpBar.transform.localScale = new Vector3((hit.distance / jumpSize) * 0.5f, 0.1f, 0.1f);

                    if (jumpBar.transform.localScale.x < twoThirdLength && jumpBar.transform.localScale.x > oneThirdLength)
                    {
                        jumpBar.color = Color.yellow;
                        jumpTime = 2; 
                    }
                    else if (jumpBar.transform.localScale.x < oneThirdLength)
                    {
                        jumpBar.color = Color.green;
                        jumpTime = 3; 
                    }
                    else if (jumpBar.transform.localScale.x > twoThirdLength)
                    {
                        jumpBar.color = Color.red;
                        jumpTime = 1; 
                    }

                    if(jumpBar.transform.localScale.x > 0.2f)
                    {

                        jumpBar.transform.localScale = new Vector3(0.2f, 0.1f, 0.1f);
                    }

                  
                }
            }
        }
        else
        {
            jumpTime = 0; 
            //jumpBar.SetActive(false);
            jumpBar.enabled = false;
        }
        previousY = transform.position.y;
    }

}
