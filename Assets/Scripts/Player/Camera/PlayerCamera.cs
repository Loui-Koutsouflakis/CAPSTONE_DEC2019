//Written by: Kyle J. Ennis
//last edited 14/10/19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera", 0)]

public class PlayerCamera : MonoBehaviour
{
    [Header("Turn on/off the mouse cursor")]
    public bool lockCursor = false;

    [Header("Invert Camera Y")]
    public bool invY;

    [Header("Player")]
    public GameObject Player;
    public GameObject shadow;
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
    Rigidbody p_RB;

    [Header("Camera Sensitivity and rotation smoothing")]
    [Tooltip("Adjusts camera movement sensitivity.")]
    [Range(1, 10)]
    public float sensitivity = 2.5f;
    [Tooltip("Adjusts how quickly the camera moves acording to user input.")]
    [Range(0, 1)]
    public float rotationsmoothTime = 0.202f;

    [Header("Distance from Player, Suggested Distance 2-3")]
    [Range(1, 10)]
    float distFromPlayer = 3;
    float YAxis;
    float YDistSpeed = 1;
    float camDistMax = 5;
    float camDistMin = 2;
    Vector2 pitchMinMax;

    #region CameraReset
    bool camReset;
    float cameraResetTimer;
    float cameraResetTime = 2;
    float cameraResetSmoothing = 0.5f;

    bool camWall;
    float camWallTimer;
    float camWallTime = 1f;
    #endregion

    [Header("Camera Field of View")]
    [Tooltip("Initial camera field of view.")]
    float i_FOV = 70;
    [Tooltip("Current camera field of view.")]
    float c_FOV;
    [Tooltip("Field of view when the player enters a small area.")]
    float enclosedCam_FOV = 75;
    [Tooltip("How quickly the camera switches between enclosed areas and open areas.")]
    float smooth = 5;

    public enum CameraType { Orbit, SideScroll, Cinema, Shake }
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

    [Header("Debug current camera rotations and eulers")]
    [SerializeField]
    Vector3 currentRotation;
    Vector3 smoothingVelocity;
    float yaw;
    float pitch;

    #region CameraShake
    public float magX;
    public float mag;
    public float dur;

    public bool increasing = true;
    #endregion

    void Start()
    {
        main = Camera.main;
        pitchMinMax = new Vector2(-20, 75);
        p_RB = Player.GetComponent<Rigidbody>();
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

        if (Input.GetKey(KeyCode.T))
        {
            //CamType = CameraType.Shake;
            CamType = CameraType.Cinema;
        }

        if (ShadowManager(Player))
            shadow.transform.position = new Vector3(s_Ground.point.x, s_Ground.point.y + 0.02f, s_Ground.point.z);

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
        EnclosedCameraFunctions(isInCloseF(), isInCloseB(), isInCloseL(), isInCloseR());        
        if(Vector3.Distance(this.gameObject.transform.position, Player.gameObject.transform.position) <= 1)
        {
            camWall = true;
        }
        if(Vector3.Distance(this.gameObject.transform.position, Player.gameObject.transform.position) > 1)
        {
            camWall = false;
            camWallTimer = 0;
        }
    }

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

    //Basic Camera Movements, Orbit around player and joystick control, as well as player offset
    void CamMovement3D()
    {
        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        YAxis = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        pitch = (invY) ? pitch += YAxis : pitch -= YAxis;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        if (Input.GetAxis("CamX") + Input.GetAxis("MouseX") == 0 && Input.GetAxis("CamY") + Input.GetAxis("MouseY") == 0 && p_RB.velocity.magnitude == 0)
            camReset = true;
        else
            cameraResetTimer = 0;
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
                transform.eulerAngles = new Vector3(0,-90, 0);
                transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position - transform.forward * DistIn2D, ref smoothingVelocity, c_Lerp);
                break;
        }
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
                TravelToCinematic(cameraOnePos, 3);
                break;
            case CameraType.Shake:
                float time = 0;
                if(time < dur)
                CameraShake(magX, mag);
                else
                    CamType = CameraType.Orbit;
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


        yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        YAxis = Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
        pitch = (invY) ? pitch += YAxis : pitch -= YAxis;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX) + (transform.up * -cameraOffsetY)) * camDist;
    }

    //will switch this to travel along a path using an array
    void TravelToCinematic(Transform camPos, float travelSpeed)
    {
        transform.position = Vector3.SmoothDamp(transform.position, camPos.position, ref smoothingVelocity, travelSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, camPos.rotation, travelSpeed * Time.deltaTime);
        //int i = 0;
        //foreach (var item in camPos)
        //{
        //    transform.position = Vector3.SmoothDamp(transform.position, camPos[i].position, ref smoothingVelocity, travelSpeed);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, camPos[i].rotation, travelSpeed * Time.deltaTime);
        //    if (transform.position == camPos[i].position)
        //        i++;
        //}
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
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y- wallDist, lineStart.z );
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

}