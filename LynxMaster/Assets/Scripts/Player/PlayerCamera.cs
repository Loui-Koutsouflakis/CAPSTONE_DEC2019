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
    public float sensitivity = 2.5f;
    [Range(0, 1)]
    public float rotationsmoothTime = 0.202f;

    [Header("Distance from Player, Suggested Distance 2-3")]
    [Range(1, 10)]
    float distFromPlayer = 3;
    float properDistance;
    Vector2 pitchMinMax;

    [Header("Debug current camera rotations and eulers")]
    [SerializeField]
    Vector3 currentRotation;
    Vector3 smoothingVelocity;
    float yaw;
    float pitch;
    // bool looking = false;

    [Header("Camera Field of View")]
    public float i_FOV = 60;
    public float c_FOV;
    public float enclosedCam_FOV = 80;
    public float smooth = 10;
    //public float jumpCam_FOV = 80;
    //bool jumping;

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

    [Header("Cinematics")]
    public int cameraChoice;
    public Vector3 cameraOnePos;
    public Vector3 cameraOneEuler;
    public Vector3 cameraTwoPos;
    public Vector3 cameraTwoEuler;

    void Start()
    {
        main = Camera.main;
        i_FOV = main.fieldOfView;
        properDistance = distFromPlayer;
        pitchMinMax = new Vector2(-5, 75);
        p_RB = Player.GetComponent<Rigidbody>();
        p_LayerMask = ~p_LayerMask;

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    }
    private void Update()
    {
        c_FOV = main.fieldOfView;
        camDist = CameraRaycast(distFromPlayer);
        if (ShadowManager(Player))
            shadow.transform.position = new Vector3(s_Ground.point.x, s_Ground.point.y + 0.02f, s_Ground.point.z);
    }

    private void LateUpdate()
    {       
        CameraPositions(cameraChoice);
        EnclosedCameraFunctions(isInCloseF(),isInCloseB(), isInCloseL(), isInCloseR());
       // JumpCamera();
    }

    //Basic Camera Movements, Orbit around player and joystick control, as well as player offset
    void CamMovement()
    {
        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        float f = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        pitch = (invY) ? pitch += f : pitch -= f;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX) + (transform.up * -cameraOffsetY)) * camDist;
    }

    //Checks cameras distance from the player and adjusts accordingly
    float CameraRaycast(float x)
    {
        if (Physics.Raycast(aimer.transform.position, main.transform.position - aimer.transform.position, out hit, properDistance, p_LayerMask))
        {
            return hit.distance;
        }
        else
        {
            return properDistance;
        }
    }

    //needs to be tied to menu system for adjustment by player
    public void switchInverseY()
    {
        invY = !invY;
    }

    //Zooms the camera out when in tight enclosures for a better view
    void EnclosedCameraFunctions(bool w,bool x, bool y,bool z)
    {
        if (w && x || y && z)
        {
            enclosed = true;
        }
        else if (!w && !x || !y && !z)
        {
            enclosed = false;
        }
        CameraZooms(enclosed, wallCheckSmoothing, enclosedCam_FOV);
    }

    //set this up for all zooms, cinematic and what not 
    void CameraZooms(bool x, float smoothing, float t_goal)
    {
        switch (x)
        {
            case true:
                if (c_FOV < t_goal)
                    main.fieldOfView += smoothing * Time.deltaTime;
                else
                    main.fieldOfView = t_goal;
                break;
            case false:
                if (c_FOV > i_FOV)
                    main.fieldOfView -= smoothing * Time.deltaTime;
                else
                    main.fieldOfView = i_FOV;
                break;
        }
    }

    //Used for positioning camera for cinematics and while the player is playing ** ADD MORE CASES FOR MORE CAMERA LOCATIONS
    void CameraPositions(int camera)
    {
        switch (camera)
        {
            case 0:
                CamMovement();
                break;
            case 1:
                transform.position = Vector3.SmoothDamp(transform.position, cameraOnePos, ref smoothingVelocity, smooth * Time.deltaTime);
                transform.localEulerAngles = cameraOneEuler;
                break;
            case 2:
                transform.position = cameraTwoPos;
                transform.localEulerAngles = cameraTwoEuler;
                break;
        }
    }


    #region  AreaChecks
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

    public bool isInCloseF()
    {
        Vector3 lineStart = Player.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y, lineStart.z + wallDist);
        Debug.DrawLine(lineStart, vectorToSearch, Color.black);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
    }

    public bool isInCloseB()
    {
        Vector3 lineStart = Player.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y, lineStart.z - wallDist);
        Debug.DrawLine(lineStart, vectorToSearch, Color.black);
        return Physics.Linecast(lineStart, vectorToSearch, out wall, p_LayerMask);
    }

    //places a shadow beneath the player
    public bool ShadowManager(GameObject x)
    {
        Vector3 lineStart = x.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - 100, lineStart.z);
        return Physics.Linecast(lineStart, vectorToSearch, out s_Ground, p_LayerMask);
    }
    #endregion

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

    //void JumpCamera()
    //{
    //    if (p_RB.velocity.y != 0)
    //        jumping = true;
    //    else
    //        jumping = false;
    //    if (!enclosed)
    //        CameraZooms(jumping, smooth, jumpCam_FOV);
    //}
}