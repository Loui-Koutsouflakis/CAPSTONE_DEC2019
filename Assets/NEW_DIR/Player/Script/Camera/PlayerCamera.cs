//Written by: Kyle J. Ennis
//last edited 14/10/19
//last edited 14/11/19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera/Player Camera", 0)]
[RequireComponent(typeof(Camera))]

public class PlayerCamera : MonoBehaviour
{

    #region Player
    GameObject Player;
    GameObject shadow;
    Rigidbody p_RB;
    #endregion
    #region UI and Player Choices
    [Header("Turn on/off the mouse cursor")]
    public bool lockCursor = false;
    [Header("Invert Camera Y")]
    public bool invY;
    [Header("Camera Sensitivity and rotation smoothing")]
    [Tooltip("Adjusts camera movement sensitivity.")]
    [Range(1, 10)]
    public float sensitivity = 2.5f;
    [Tooltip("Adjusts how quickly the camera moves acording to user input.")]
    [Range(0, 1)]
    public float rotationsmoothTime = 0.202f;
    #endregion
    #region Camera Distance and Orbit Management
    [Header("Distance from Player, Suggested Distance 2-3")]
    [Range(1, 10)]
    float distFromPlayer = 3;
    float YAxis;
    float YDistSpeed = 1;
    float camDistMax = 5;
    float camDistMin = 2;
    Vector2 pitchMinMax;
    [Header("Debug current camera rotations and eulers")]
    [SerializeField]
    Vector3 currentRotation;
    Vector3 smoothingVelocity;
    float yaw;
    float pitch;
    #endregion
    #region CameraRayCasts
    [Header("CameraRayCast")]
    Camera main;
    GameObject aimer;
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

    Vector3 lineStart;
    Vector3 left;
    Vector3 right;
    Vector3 forward;
    Vector3 back;
    #endregion
    #region CameraReset
    bool camReset;
    float cameraResetTimer;
    float cameraResetTime = 2;
    float cameraResetSmoothing = 0.5f;

    bool camWall;
    float camWallTimer;
    float camWallTime = 1f;
    #endregion
    #region DragonCamera
    [Header("Dragon Camera Settings")]
    public float dragonOffsetX;
    public float dragonOffsetY;
    public float dragonCamDist;
    public float dragonPitch;
    public float dragonRotationSmoothTime;
    #endregion
    #region Field of View and Zoom Function Smoothing
    [Header("Camera Field of View")]
    [Tooltip("Initial camera field of view.")]
    float i_FOV = 70;
    [Tooltip("Current camera field of view.")]
    float c_FOV;
    [Tooltip("Field of view when the player enters a small area.")]
    float enclosedCam_FOV = 75;
    [Tooltip("How quickly the camera switches between enclosed areas and open areas.")]
    float smooth = 5;
    #endregion

    public enum CameraType { Orbit, SideScroll, Cinema, Shake, Dragon }
    [Header("CameraType")]
    [SerializeField]
    [Tooltip("Choose what type of camera the game is currently using.")]
    CameraType CamType;

    #region 2D sideScroll
    [Header("2D Platforming")]
    [Tooltip("The cameras follow speed when in 2D Mode.")]
    float c_Lerp = 0.5f;
    [Tooltip("How far away the camera is when in 2D Mode.")]
    public float DistIn2D = 5;
    public enum WallCamChoice { Back, Left, Front, Right }
    [SerializeField]
    [Tooltip("Choose what direction to set the 2D camera.")]
    WallCamChoice wallCamChoice;
    #endregion
    [Header("Cinematics")]
    [Tooltip("Cinematic camera One.")]
    public Transform cameraOnePos;
    [Tooltip("Cinematic Camera Two.")]
    public Transform cameraTwoPos;

    #region CameraShake
    [Header("Camera Shake Options")]
    [Tooltip("How far to shake the camera.")]
    public float s_Distance;
    [Tooltip("How hard to shake the camera.")]
    public float s_Magnitude;
    [Tooltip("How long to shake the camera for.")]
    public float s_Duration;

    public bool increasing = true;
    #endregion
    float time = 0;

    private void Awake()
    {
        if (!Player)
            Player = FindObjectOfType<PlayerController>().gameObject;
        //if (!shadow)
        //    shadow = GameObject.FindGameObjectWithTag("LumiShadow");
        if (!aimer)
            aimer = GameObject.FindGameObjectWithTag("CameraAimer");
        if (!main)
            main = Camera.main;
        if (!p_RB)
            p_RB = Player.GetComponent<Rigidbody>();

    }
    void Start()
    {
        pitchMinMax = new Vector2(-20, 75);
        p_LayerMask = ~p_LayerMask;
        CamType = CameraType.Orbit;
        wallCamChoice = WallCamChoice.Front;

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {

        c_FOV = main.fieldOfView;
        distFromPlayer = UpDownCam(invY);
        camDist = CameraRaycast(distFromPlayer);
        RaycastDirections();
        if (Input.GetKey(KeyCode.T))
        {
            CamType = CameraType.Shake;
            // CamType = CameraType.Cinema;
            time = 0;
        }

        //if (ShadowManager(Player))
        //    shadow.transform.position = new Vector3(s_Ground.point.x, s_Ground.point.y + 0.02f, s_Ground.point.z);

        if (CamType == CameraType.Orbit)
        {
            if (Input.GetButtonDown("LeftBumper"))
            {
                resetCamera(0.0f);
            }
        }
    }
    private void LateUpdate()
    {
        CameraTypes(CamType);
        EnclosedCameraFunctions(AreaChecks(forward), AreaChecks(back), AreaChecks(left), AreaChecks(right));

        if (Vector3.Distance(this.gameObject.transform.position, Player.gameObject.transform.position) <= 1)
        {
            camWall = true;
        }
        if (Vector3.Distance(this.gameObject.transform.position, Player.gameObject.transform.position) > 1)
        {
            camWall = false;
            camWallTimer = 0;
        }
    }

    #region CameraTypes 

    void CameraTypes(CameraType cam)
    {
        switch (cam)
        {
            case CameraType.Orbit:
                CamMovement3D();
                break;
            case CameraType.SideScroll:
                CamMovement2D(wallCamChoice);
                break;
            case CameraType.Cinema:
                //TravelToCinematic(cameraOnePos, 3);
                break;
            case CameraType.Shake:
                time += Time.deltaTime;
                if (time < s_Duration)
                    CameraShake(s_Distance, s_Magnitude);
                else
                    CamType = CameraType.Orbit;
                break;
            case CameraType.Dragon:
                DragonCam();
                break;
        }
    }

    //Basic Camera Movements, Orbit around player and joystick control, as well as player offset
    void CamMovement3D()
    {
        yaw += Input.GetAxis("HorizontalJoy") * sensitivity + Input.GetAxis("Horizontal") * sensitivity + Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        YAxis = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        pitch = (invY) ? pitch += YAxis : pitch -= YAxis;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX) + (transform.up * -cameraOffsetY)) * camDist;
    }

    //2D Platforming void, for those tough to reach areas
    void CamMovement2D(WallCamChoice x)
    {
        switch (x)
        {
            case WallCamChoice.Front:
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * DistIn2D, ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.Back:
                transform.eulerAngles = new Vector3(0, 180, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * DistIn2D, ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.Left:
                transform.eulerAngles = new Vector3(0, 90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * DistIn2D, ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.Right:
                transform.eulerAngles = new Vector3(0, -90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * DistIn2D, ref smoothingVelocity, c_Lerp);
                break;
        }
    }

    //baseline version untested on actual scene or asset****************
    void DragonCam()
    {
        yaw += Input.GetAxis("HorizontalJoy") * sensitivity + Input.GetAxis("Horizontal") * sensitivity;
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(dragonPitch, yaw), ref smoothingVelocity, dragonRotationSmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * dragonOffsetX) + (transform.up * -dragonOffsetY)) * dragonCamDist;
    }

    public void SwitchToCinema(CameraType which)
    {
        CamType = which;
    }

    #endregion

    #region CameraUtilities

    //changes the distance of the cam based on the angle of the camera
    float UpDownCam(bool x)
    {
        if (distFromPlayer < camDistMin)
            distFromPlayer = camDistMin;
        if (distFromPlayer > camDistMax)
            distFromPlayer = camDistMax;

        if (x)
        {
            if (distFromPlayer <= camDistMax && distFromPlayer >= camDistMin)
                distFromPlayer += YAxis * YDistSpeed * Time.deltaTime;
            return distFromPlayer;
        }
        else
            if (distFromPlayer <= camDistMax && distFromPlayer >= camDistMin)
            distFromPlayer -= YAxis * YDistSpeed * Time.deltaTime;
        return distFromPlayer;
    }

    //Checks cameras distance from the player and adjusts accordingly
    float CameraRaycast(float x)
    {
        if (Physics.Raycast(aimer.transform.position, main.transform.position - aimer.transform.position, out hit, x, p_LayerMask))
            return hit.distance - 0.2f;
        else
            return x;
    }

    //needs to be tied to menu system for adjustment by player
    public void switchInverseY()
    {
        invY = !invY;
    }

    //Zooms the camera out when in tight enclosures for a better view
    void EnclosedCameraFunctions(bool w, bool x, bool y, bool z)
    {
        //Debug.Log(w + " " + x + " " + y + " " + z);
        if (w && x || y && z)
            enclosed = true;
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

    void resetCamera(float x)
    {
        camReset = false;
        currentRotation = Vector3.SmoothDamp(currentRotation, Player.transform.eulerAngles, ref smoothingVelocity, x);
        transform.eulerAngles = currentRotation;
        pitch = currentRotation.x;
        yaw = currentRotation.y;
    }

    void CameraShake(float x, float mag)
    {
        float y = -x;
        if (currentRotation.z > x)
            increasing = false;
        else if (currentRotation.z < y)
            increasing = true;
        if (increasing)
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, currentRotation.z += mag * Time.deltaTime), ref smoothingVelocity, rotationsmoothTime);
        else
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, currentRotation.z -= mag * Time.deltaTime), ref smoothingVelocity, rotationsmoothTime);

        CamMovement3D();
    }

    #region  AreaChecks

    void RaycastDirections()
    {
        lineStart = Player.transform.position;
        left = new Vector3(lineStart.x + wallDist, lineStart.y, lineStart.z);
        right = new Vector3(lineStart.x - wallDist, lineStart.y, lineStart.z);
        forward = new Vector3(lineStart.x, lineStart.y, lineStart.z + wallDist);
        back = new Vector3(lineStart.x, lineStart.y - wallDist, lineStart.z);
    }

    public bool AreaChecks(Vector3 d)
    {
        Vector3 vectorToSearch = d;
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

    #endregion




}