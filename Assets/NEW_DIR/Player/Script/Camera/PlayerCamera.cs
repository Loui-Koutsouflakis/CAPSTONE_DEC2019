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
    PlayerClass pl;
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
    float r_Sensitivity = 2f;
    float sr_Sensitivity;
    float r_RotationSmoothTime = .202f;
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
    // int p_LayerMask = 1 << 9;
    public LayerMask IgnoreMask;
    RaycastHit hit;
    RaycastHit wall;
    RaycastHit s_Ground;
    float wallDist = 3;
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
    public Boss dragon;
    [Header("Dragon Camera Settings")]
    public float dragonOffsetX;
    public float dragonOffsetY;
    public float dragonCamDist;
    public float dragonPitch;
    public float dragonRotationSmoothTime;
    public float dragonSensitivity;
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
    public float c_Lerp;
    [Tooltip("How far away the camera is when in 2D Mode.")]
    public float DistIn2D;
    public float Height2D;
    public Vector3 passThrough2D;
    public enum WallCamChoice { Back, Left, Front, Right, BackLeft, FrontLeft, BackRight, FrontRight }
    [SerializeField]
    [Tooltip("Choose what direction to set the 2D camera.")]
    public WallCamChoice wallCamChoice;
    public bool cinema_Playing = false;
    #endregion
    [Header("Cinematics")]
    [Tooltip("Cinematic camera One.")]
    public Transform cameraOnePos;
    [Tooltip("Cinematic Camera Two.")]
    public Transform cameraTwoPos;
    public bool isCinema;
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
        if (!pl) pl = FindObjectOfType<PlayerClass>();
        if (!aimer)
            aimer = GameObject.FindGameObjectWithTag("CameraAimer");
        if (!main)
            main = Camera.main;
        if (!p_RB)
            p_RB = Player.GetComponent<Rigidbody>();
        if (!dragon)
            dragon = FindObjectOfType<Boss>();
        //else
            //Debug.Log("dont worry about it");
    }
    void Start()
    {
        pitchMinMax = new Vector2(-20, 75);
        CamType = CameraType.Orbit;
        sr_Sensitivity = r_Sensitivity;
        wallCamChoice = WallCamChoice.Front;

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (main != null)
        {
            c_FOV = main.fieldOfView;
        }

        distFromPlayer = UpDownCam(invY);
        camDist = CameraRaycast(distFromPlayer);
        RaycastDirections();


        if (Input.GetKey(KeyCode.T))
        {
            CamType = CameraType.Shake;
            // CamType = CameraType.Cinema;
            time = 0;
        }
        if (enclosed)
            r_Sensitivity = 0.5f;
        else
            r_Sensitivity = sr_Sensitivity;
        //if (ShadowManager(Player))
        //    shadow.transform.position = new Vector3(s_Ground.point.x, s_Ground.point.y + 0.02f, s_Ground.point.z);

        if (CamType == CameraType.Orbit)
        {
            if (Input.GetButtonDown("LeftBumper"))
            {
                resetCamera(0.0f);
            }
        }
        if (dragon != null)//most of this will be moved to another script, (Player functionality to the grapple trigger)
        {
            SteeringTheDragon();
        }
    }
    private void LateUpdate()
    {
        CameraTypes(CamType);
        EnclosedCameraFunctions(AreaChecks(forward), AreaChecks(back), AreaChecks(left), AreaChecks(right));
    }

    #region CameraTypes 

    void CameraTypes(CameraType cam)
    {
        switch (cam)
        {
            case CameraType.Orbit:
                CamMovement3D();
                isCinema = false;
                break;
            case CameraType.SideScroll:
                CamMovement2D(wallCamChoice);
                isCinema = false;
                break;
            case CameraType.Cinema:
                isCinema = true;
                if (Input.GetButtonDown("AButton"))
                {
                    CamType = CameraType.Orbit;
                    pl.EnableControls();
                    pl.GetComponent<Rigidbody>().isKinematic = false;
                }
                break;
            case CameraType.Shake:
                time += Time.deltaTime;
                if (time < s_Duration)
                    CameraShake(s_Distance, s_Magnitude);
                else
                    CamType = CameraType.Orbit;
                break;
            case CameraType.Dragon:
                isCinema = false;
                DragonCam();
                break;
        }
    }

    //Basic Camera Movements, Orbit around player and joystick control, as well as player offset
    void CamMovement3D()
    {
        YAxis = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        pitch = (invY) ? pitch += YAxis : pitch -= YAxis;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        //if (p_RB.velocity.magnitude > 0.5f)
        //{
        //    yaw += Input.GetAxis("HorizontalJoy") * r_Sensitivity + Input.GetAxis("Horizontal") * r_Sensitivity + Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        //    currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, r_RotationSmoothTime);
        //}
        //else
        //{
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        //}

        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX) + (transform.up * -cameraOffsetY)) * camDist;
    }

    //2D Platforming void, for those tough to reach areas
    void CamMovement2D(WallCamChoice x)
    {
        
        switch (x)
        {
            case WallCamChoice.Front:
                passThrough2D = new Vector3(15, 0, 0);
                 transform.eulerAngles = new Vector3(15, 0, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.Back:
                passThrough2D = new Vector3(15, 180, 0);
                transform.eulerAngles = new Vector3(15, 180, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.Left:
                passThrough2D = new Vector3(15, 90, 0);
                transform.eulerAngles = new Vector3(15, 90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.Right:
                passThrough2D = new Vector3(15, -90, 0);
                transform.eulerAngles = new Vector3(15, -90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.BackLeft:
                passThrough2D = new Vector3(15, 135, 0);
                transform.eulerAngles = new Vector3(15, 135, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.BackRight:
                passThrough2D = new Vector3(15, -135, 0);
                transform.eulerAngles = new Vector3(15, -135, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.FrontLeft:
                passThrough2D = new Vector3(15, 45, 0);
                transform.eulerAngles = new Vector3(15, 45, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
            case WallCamChoice.FrontRight:
                passThrough2D = new Vector3(15, -45, 0);
                transform.eulerAngles = new Vector3(15, -45, 0);
                transform.position = Vector3.SmoothDamp(transform.position, (Player.transform.position - transform.forward * DistIn2D) + (transform.up * Height2D), ref smoothingVelocity, c_Lerp);
                break;
        }
    }

    //baseline version untested on actual scene or asset****************
    void DragonCam()
    {
        yaw += Input.GetAxis("HorizontalJoy") * dragonSensitivity + Input.GetAxis("Horizontal") * dragonSensitivity;
        //Add clamp for dragon rotation, must not exceed 45 degrees in either direction of the dragons nose
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(dragonPitch, yaw), ref smoothingVelocity, dragonRotationSmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = dragon.gameObject.transform.position - (transform.forward - (transform.right * dragonOffsetX) + (transform.up * -dragonOffsetY)) * dragonCamDist;
    }

    public void SwitchToCinema(CameraType which)
    {
        CamType = which;
    }
    public void SwitchFromWallJump(CameraType which, Vector3 wall)
    {
        CamType = which;
        pitch = wall.x;
        yaw = wall.y;
        currentRotation = wall;
        
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
        if (Physics.Raycast(aimer.transform.position, main.transform.position - aimer.transform.position, out hit, x, IgnoreMask))
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

    public void SteeringTheDragon()
    {

        if (dragon.steering == true)
        {
            CamType = CameraType.Dragon;
            Player.GetComponent<PlayerClass>().DisableControls();
            p_RB.isKinematic = true;
        }
        else
        {
            CamType = CameraType.Orbit;
            Player.GetComponent<PlayerClass>().EnableControls();
            p_RB.isKinematic = false;
        }
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
        return Physics.Linecast(lineStart, vectorToSearch, out wall, IgnoreMask);
    }

    //places a shadow beneath the player
    public bool ShadowManager(GameObject x)
    {
        Vector3 lineStart = x.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - 100, lineStart.z);
        return Physics.Linecast(lineStart, vectorToSearch, out s_Ground, IgnoreMask);
    }
    #endregion

    #endregion




}