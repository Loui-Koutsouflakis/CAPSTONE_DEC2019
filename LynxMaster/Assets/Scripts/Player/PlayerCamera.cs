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


    [Header("Camera Sensitivity and rotation smoothing(Suggested sensitivity = 4)")]
    [Range(1, 10)]
    public float sensitivity = 5;
    [Range(0, 1)]
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
    Vector3 smoothingVelocity;
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
    int p_LayerMask = 1 << 9;
    RaycastHit hit;
    RaycastHit wall;
    RaycastHit s_Ground;
    public float wallDist = 5;
    public float wallCheckSmoothing = 60;
    public bool enclosed = false;
    float cameraOffsetX = .2f;
    float cameraOffsetY = 0.2f;
    float camDist;
    void Start()
    {
        main = Camera.main;
        i_FOV = main.fieldOfView;
        properDistance = distFromPlayer;
        pitchMinMax = new Vector2(-5, 75);
        p_RB = Player.GetComponent<Rigidbody>();
        p_LayerMask = ~p_LayerMask;

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
        camDist = CameraRaycast(distFromPlayer);
    }

    float CameraRaycast(float x)
    {
        if (Physics.Raycast(aimer.transform.position, main.transform.position - aimer.transform.position, out hit, properDistance, p_LayerMask)) //aimer.transform.TransformDirection(Vector3.forward), out hit, 10, p_LayerMask))
        {
            return hit.distance;
        }
        else
        {
            return properDistance;
        }
    }
    private void LateUpdate()
    {
        CamMovement();
        //CameraFieldOfView();
        // JumpCamera();
    }

    void CamMovement()
    {
        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        float f = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        
        if (invY) pitch += f; 
        else pitch -= f;

        pitch = Mathf.Clamp(pitch, -12, yAngle);
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX) + (transform.up * -cameraOffsetY)) * camDist;
    }

    public void switchInverseY()
    {
        invY = !invY;
    }

    void JumpCamera()
    {
        if (p_RB.velocity.y > 0 && c_FOV <= b_FOV/* && !enclosed*/)
        {
            main.fieldOfView += (smooth * Time.deltaTime);
        }
        if (p_RB.velocity.y <= 0 && c_FOV >= i_FOV /*&& !enclosed*/)
        {
            main.fieldOfView -= (smooth * Time.deltaTime);
        }
    }

    void CameraFieldOfView()
    {
        if (isInCloseR() && isInCloseL() && c_FOV <= b_FOV)
        {
            main.fieldOfView += wallCheckSmoothing * Time.deltaTime;
            enclosed = true;
        }
        else if (!isInCloseL() && !isInCloseR() && c_FOV >= i_FOV)
        {
            main.fieldOfView -= wallCheckSmoothing * Time.deltaTime;
            enclosed = false;
        }
    }
    public bool ShadowManager(GameObject x)
    {
        Vector3 lineStart = x.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - 100, lineStart.z);
        return Physics.Linecast(lineStart, vectorToSearch, out s_Ground, p_LayerMask);
    }

    public bool isInCloseL()
    {
        Vector3 lineStart = Player.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x + wallDist, lineStart.y, lineStart.z);
        Debug.DrawLine(lineStart, vectorToSearch, Color.black);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
    }

    public bool isInCloseR()
    {
        Vector3 lineStart = Player.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x - wallDist, lineStart.y, lineStart.z);
        Debug.DrawLine(lineStart, vectorToSearch, Color.black);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
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