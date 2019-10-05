using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Turn on/off the mouse cursor")]
    public bool lockCursor = false;

    [Header("Invert Camera Y (set to off for mouse control)")]
    public bool invY;

    [Header("Player")]
    public GameObject Player;
    public GameObject shadow;
    Rigidbody p_RB;
    Vector3 playerFront;

    [Header("Camera Sensitivity and rotation smoothing(Suggested sensitivity = 4)")]
    [Range(1, 10)]
    public float sensitivity = 5;
    [Range(0,1)]
    public float rotationsmoothTime = 0.667f;
    float yAngle = 75;

    [Header("Distance from Player, Suggested Distance 2-3")]
    [Range(1, 10)]
    public float distFromPlayer = 3;
    float properDistance = 3;
    Vector2 pitchMinMax; //set to -15 and 90 respectively in editor

    [Header("Debug current camera rotations and eulers")]
    [SerializeField]
    Vector3 currentRotation;
    Vector3 eul;
    Vector3 smoothingVelocity;
    Vector3 temp;
    float yaw;
    float pitch;
   // bool looking = false;

    [Header("Camera Field of View")]
    public float i_FOV = 60;
    public float z_FOV = 30;
    public float b_FOV = 80;
    public float c_FOV;
    public float smooth = 90;

    [Header("CameraRayCast")]
    public Camera main;
    public GameObject aimer;
    RaycastHit s_Ground;
    LayerMask layerMask;
    float cameraOffsetX = .2f;
    float cameraOffsetY = 0.5f;

    void Start()
    {
        main = Camera.main;
        i_FOV = main.fieldOfView;
        properDistance = distFromPlayer;
        pitchMinMax = new Vector2(-5, 75);
        properDistance = distFromPlayer;
        p_RB = Player.GetComponent<Rigidbody>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        c_FOV = main.fieldOfView;
        if (ShadowManager(Player))
        {
            shadow.transform.position = new Vector3(s_Ground.point.x, s_Ground.point.y + 0.02f, s_Ground.point.z);
        }
    }
    private void LateUpdate()
    {
        CamMovement();
        JumpCamera();
    }

    void CamMovement()
    {
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        if (invY)
        {
            invertY();
        }
        if (!invY)
        {
            nonInvertY();
        }
        pitchMinMax = new Vector2(-12, yAngle);
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        eul = transform.eulerAngles;
        eul.x = 0;
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX)) * CameraRaycast(distFromPlayer) + (transform.up * cameraOffsetY);

    }

    void invertY()
    {
        pitch += Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);

    }
    void nonInvertY()
    {
        pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
    }

    public void switchInverseY()
    {
        invY = !invY;
    }

    void JumpCamera()
    {
        
        if (p_RB.velocity.y > 0 && c_FOV <= b_FOV)
        {
            main.fieldOfView -= (-smooth * Time.deltaTime);
        }
        if (p_RB.velocity.y <= 0 && c_FOV >= i_FOV)
        {
            main.fieldOfView += (-smooth * Time.deltaTime);
        }
    }

    public bool ShadowManager(GameObject x)
    {
        int p_LayerMask = 1 << 9;
        p_LayerMask = ~p_LayerMask;
        Vector3 lineStart = x.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - 10, lineStart.z);
        return Physics.Linecast(lineStart, vectorToSearch, out s_Ground, p_LayerMask);
    }

    float CameraRaycast(float x)
    {
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit hit;
        aimer.transform.LookAt(main.transform.position);
        if (Physics.Raycast(aimer.transform.position, aimer.transform.TransformDirection(Vector3.forward), out hit,10, layerMask))
        {
            float dist = hit.distance;
            if (dist < distFromPlayer)
            {
                x = dist;
            }
            else 
            {
                x = properDistance;
            }
        }
        return x;
    }
    //*********************************************************************NOT IN USE BUT MAYBE LATER*************************************************************************
    //void ZoomFunction()
    //{
    //    looking = true;
    //    if (c_FOV != z_FOV)
    //    {
    //        if (c_FOV > z_FOV)
    //        {
    //            main.fieldOfView += (-smooth * Time.deltaTime);
    //        }
    //    }
    //    else if (c_FOV <= z_FOV)
    //    {
    //        main.fieldOfView = z_FOV;
    //    }
    //}

}