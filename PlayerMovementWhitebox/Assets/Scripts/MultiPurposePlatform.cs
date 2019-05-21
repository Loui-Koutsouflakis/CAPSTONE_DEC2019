using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPurposePlatform : MonoBehaviour
{
    //Platform Types
    enum PlatformType { Linear = 1, Radial, Spinning }
    [SerializeField, Header("Movement Type")]
    PlatformType type;

    [SerializeField, Header("Enable Live Editing")]
    bool liveEdit = false;
    
    // Player Reference
    GameObject Player;// Used For Attaching To Platform
    
    // Target Position Variables
    [Range(-10, 10), SerializeField, Header("End Position / Rotation Point")]
    float xDistance = 0.0f;// Desired Position X Mod Variable

    [Range(-10, 10), SerializeField]
    float yDistance = 0.0f;// Desired Position Y Mod Variable

    [Range(-10, 10), SerializeField]
    float zDistance = 0.0f;// Desired Position Z Mod Variable

    [SerializeField, Header("Linear or Radial Movement Repeatable") ]
    bool repeatable = false;
   
    // Movement variables   
    Vector3 startPosition;//Return Point for linear movement
    Vector3 desiredPosition;//Final Destination for linear movement
    Vector3 target;//Target Destination for linear movement    
    float startTime = 0.0f;// used for movement timing
    
    //Radial Movement Variables
    Vector3 centerPoint;// Point Between Start Position and Desrired Position
    Vector3 startRelativeToCenter;// Start Position Relative To Center
    Vector3 desiredRelativeToCenter;// Desired Position Relative To Center
    
    // Spinning Platfrom exclusive Variables
    [SerializeField, Range(1, 100), Header("Spinning Type Speed Variables")]
    float speedAroundDesiredPosition = 1.0f;// Rotation Speed Around Desired Position
    [SerializeField, Range(1, 100)]
    float speedAroundOwnAxis = 1.0f;// Rotation Around Own Y Axis
    // Use this for initialization
    void Start()
    {
        Startup();
    }
    // Update is called once per frame
    void Update()
    {
        if (liveEdit)
        {
            ResetPosition();            
        }
        else
        {
            PlatformRun();
        }
    }
    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject == Player)
        {
            Player.transform.parent = transform;
        }

    }

    private void OnTriggerExit(Collider o)
    {
        if (o.gameObject == Player)
        {
            Player.transform.parent = null;
        }
    }

    void LinearMovement()
    {
        if (repeatable)
        {            
            RepeatableLinearMovement();
        }
        else
        {
            SingleLinearMovement();
        }
    }
    void MoveWithRadius()
    {
        if (repeatable)
        {
            RepeatableRadialMovement();
        }
        else
        {
            SingleRadialMovement();
        }
    }
    void GetCenter(Vector3 direction)
    {
        centerPoint = (startPosition + desiredPosition) * .5f;
        centerPoint -= direction;
        startRelativeToCenter = startPosition - centerPoint;
        desiredRelativeToCenter = desiredPosition - centerPoint;
    }
    void SingleRadialMovement()
    {
        float fractionOfTravelComplete = (Time.time - startTime);
        transform.position = Vector3.Slerp(startRelativeToCenter, desiredRelativeToCenter, fractionOfTravelComplete);
        transform.position += centerPoint;
    }
    void RepeatableRadialMovement()
    {
        float fractionOfTravelComplete = Mathf.PingPong(Time.time - startTime, 1);
        transform.position = Vector3.Slerp(startRelativeToCenter, desiredRelativeToCenter, fractionOfTravelComplete);
        transform.position += centerPoint;
    }
    void SingleLinearMovement()
    {
        float fractionOfTravelComplete = (Time.time - startTime);
        transform.position = Vector3.Lerp(startPosition, desiredPosition, fractionOfTravelComplete);
    }
    void RepeatableLinearMovement()
    {
        float fractionOfTravelComplete = Mathf.PingPong((Time.time - startTime), 1);
        transform.position = Vector3.Lerp(startPosition, desiredPosition, fractionOfTravelComplete);
    }
    void SpinPlatform()
    {
        transform.Rotate(Vector3.up, 5.0f * Time.deltaTime * speedAroundOwnAxis);
        transform.RotateAround(desiredPosition, Vector3.up, 5.0f * Time.deltaTime * speedAroundDesiredPosition);
    }
    void ResetPosition()
    {
        switch (type)
        {
            case PlatformType.Linear:                               
                desiredPosition.Set(transform.position.x + xDistance, transform.position.y + yDistance, transform.position.z + zDistance);
                transform.position = startPosition;              
                break;
            case PlatformType.Radial:                              
                desiredPosition.Set(transform.position.x + xDistance, transform.position.y + yDistance, transform.position.z + zDistance);
                transform.position = startPosition;              
                break;
            case PlatformType.Spinning:
                desiredPosition.Set(startPosition.x + xDistance, startPosition.y + yDistance, startPosition.z + zDistance);
                transform.position = startPosition;
                break;
        }
    }
    void PlatformRun()
    {
        switch (type)
        {
            case PlatformType.Linear:               
                LinearMovement();
                break;
            case PlatformType.Radial:                
                GetCenter(Vector3.up);
                MoveWithRadius();
                break;
            case PlatformType.Spinning:
                SpinPlatform();
                break;
        }
    }
    void Startup()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        desiredPosition.x = xDistance + transform.position.x;
        desiredPosition.y = yDistance + transform.position.y;
        desiredPosition.z = zDistance + transform.position.z;
        startPosition = transform.position;
        startTime = Time.time;
    }
    public void SetObjectMesh()
    {
        // To Be Written
    }

    void OnDrawGizmosSelected()
    {
        switch(type)
        {
            case PlatformType.Linear:
                Gizmos.color = Color.green;
                Gizmos.DrawCube(desiredPosition, transform.lossyScale);
                break;
            case PlatformType.Radial:
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(desiredPosition, transform.lossyScale);
                break;
            case PlatformType.Spinning:
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(desiredPosition, .3f);
                break;
        }        
    }
}
