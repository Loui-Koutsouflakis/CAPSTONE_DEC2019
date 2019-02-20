using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIALCAMERASCRIPT : MonoBehaviour {
    //public bool ZTarget;
    public bool lockCursor = true;

    public Transform Player;
    public Transform map;
    public float sensitivity = 5;
    private float distFromPlayer = 8;
    public Vector2 pitchMinMax ;

    public float rotationsmoothTime = 0.546f;

    private Vector3 tmpSmooth;
    Vector3 smoothingVelocity;
    public Transform cameraIdlePosition;
    Vector3 temp;
    public Vector3 currentRotation;
    public Vector3 eul;

    float yaw;
    float pitch;
    public bool cam = true;
    void Start()
    {
        //temp = Player.position - transform.forward * distFromPlayer;
        //tmpSmooth = smoothingVelocity;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
            pitchMinMax = new Vector2(map.position.x, 180);
            pitchMinMax = new Vector2(0, 180);
            CamMovement();
        //if (Input.GetButtonDown("RightJoyClick"))
        //{
        //    cam = false;
        //    camCentre();
        //}
        
    }
    private void LateUpdate()
    {
        
    }
    void CamMovement()
    {
       
        yaw -= Input.GetAxis("CamX")+ Input.GetAxis("MouseX") * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        if (currentRotation.x < 90)
        {
            pitch += Input.GetAxis("CamY")+ Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
            transform.eulerAngles = currentRotation;
            eul = transform.eulerAngles;
            eul.x = 0;
        }
        if(currentRotation.x >= 88 && currentRotation.x <= 92)
        {
            smoothingVelocity = smoothingVelocity * 2;
        }
        if (currentRotation.x > 90)
        {
            pitch -= Input.GetAxis("CamY")+ Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 180), ref smoothingVelocity, rotationsmoothTime);
            transform.eulerAngles = currentRotation;
            eul = transform.eulerAngles;
            eul.x = 0;
        }
        transform.eulerAngles = currentRotation;
        transform.position = Player.position - transform.forward * distFromPlayer;
    }
    //void camCentre()
    //{
    //    transform.position = temp;
    //    if (transform.position == temp)
    //    {
    //        cam = true;
    //    }
    //}
}