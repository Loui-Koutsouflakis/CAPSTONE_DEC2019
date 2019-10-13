//Written by: Kyle J. Ennis

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Turn on/off the mouse cursor")]
    public bool lockCursor = false;

    [Header("Invert Camera Y")]
    public bool invY;

    [Header("Player")]
    public GameObject Player;
    public GameObject shadow;
    Rigidbody p_RB;

    [Header("Camera Sensitivity and rotation smoothing")]
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

    #region CameraReset
    bool camReset;
    float cameraResetTimer;
    float cameraResetTime = 1;
    float cameraResetSmoothing = 0.5f;
    #endregion

    [Header("Camera Field of View")]
    public float i_FOV = 70;
    public float c_FOV;
    public float enclosedCam_FOV = 75;
    public float smooth = 15;


    #region CameraRayCasts
    [Header("CameraRayCast")]
    public Camera main;
    public GameObject aimer;
    int p_LayerMask = 1 << 9;
    RaycastHit hit;
    RaycastHit wall;
    RaycastHit s_Ground;
    float wallDist = 5;
    float wallCheckSmoothing = 20;
    bool enclosed = false;
    float cameraOffsetX = .2f;
    float cameraOffsetY = 0.2f;
    float camDist;
    #endregion

    [Header("2D Platforming")]
    public float c_Lerp;
    public float FlatDistance;
    public int wallCamChoice;//0 = forward, 1 = back, 2 = left, 3 = right

    [Header("Cinematics")]
    public int cameraChoice;//0 = 3D Platforming, 1 = 2D Platforming, 2+ = Cinematic Camera types or underdetermined functionality (to be written)
    public Vector3 cameraOnePos;
    public Vector3 cameraOneEuler;
    public Vector3 cameraTwoPos;
    public Vector3 cameraTwoEuler;

    void Start()
    {
        main = Camera.main;
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
        if (cameraChoice == 0)
        {
            if (camReset && cameraResetTimer < cameraResetTime)
                cameraResetTimer += 1 * Time.deltaTime;
            if (cameraResetTimer >= cameraResetTime)
                resetCamera();
        }
    }

    private void LateUpdate()
    {       
        CameraTypes(cameraChoice);
        EnclosedCameraFunctions(isInCloseF(),isInCloseB(), isInCloseL(), isInCloseR());
    }

    //Basic Camera Movements, Orbit around player and joystick control, as well as player offset
    void CamMovement3D()
    {
        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        float f = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        pitch = (invY) ? pitch += f : pitch -= f;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        if (Input.GetAxis("CamX") == 0 && Input.GetAxis("CamY") == 0 && p_RB.velocity.magnitude == 0)
            camReset = true;
        else
            cameraResetTimer = 0;
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX) + (transform.up * -cameraOffsetY)) * camDist;
    }

    //2D Platforming void, for those tough to reach areas
    void CamMovement2D(int x)
    {
        switch (x)
        {
            case 0:
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * FlatDistance, ref smoothingVelocity, c_Lerp);
                break;
            case 1:
                transform.eulerAngles = new Vector3(0, 180, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * FlatDistance, ref smoothingVelocity, c_Lerp);
                break;
            case 2:
                transform.eulerAngles = new Vector3(0, 90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * FlatDistance, ref smoothingVelocity, c_Lerp);
                break;
            case 3:
                transform.eulerAngles = new Vector3(0,-90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * FlatDistance, ref smoothingVelocity, c_Lerp);
                break;
        }
    }

    //Checks cameras distance from the player and adjusts accordingly
    float CameraRaycast(float x)
    {
        if (Physics.Raycast(aimer.transform.position, main.transform.position - aimer.transform.position, out hit, properDistance, p_LayerMask))
            return hit.distance;
        else
            return properDistance;
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
            enclosed = true;
        else if (!w && !x || !y && !z)
        {
            enclosed = false;
        }
        CameraZooms(enclosed, wallCheckSmoothing, enclosedCam_FOV);
    }

    //Used for positioning camera for cinematics and while the player is playing ** ADD MORE CASES FOR MORE CAMERA LOCATIONS
    void CameraTypes(int camera)
    {
        switch (camera)
        {
            case 0:
                CamMovement3D();
                break;
            case 1:
                CamMovement2D(wallCamChoice);
                break;
            case 2:
                transform.position = Vector3.SmoothDamp(transform.position, cameraOnePos, ref smoothingVelocity, smooth * Time.deltaTime);
                transform.localEulerAngles = cameraOneEuler;
                break;
            case 3:
                transform.position = cameraTwoPos;
                transform.localEulerAngles = cameraTwoEuler;
                break;
        }
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

    void resetCamera()
    {
        camReset = false;
        currentRotation = Vector3.SmoothDamp(currentRotation, Player.transform.localEulerAngles, ref smoothingVelocity, cameraResetSmoothing);
        transform.eulerAngles = currentRotation;
        pitch = currentRotation.x;
        yaw = currentRotation.y;
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