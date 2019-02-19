using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIALCAMERASCRIPT : MonoBehaviour {
    //public bool ZTarget;
    //public Transform cameraIdlePosition;
    //private bool cam = true;
    [Header("Turn on/off the mouse cursor")]
    public bool lockCursor = true;

    [Header("Invert Camera Y")]
    public bool invY;

    [Header("Invert Camera X")]
    public bool invX;

    [Header("Player and world position")]
    public Transform Player;
    public Transform map;

    [Header("Camera Sensitivity and rotation smoothing")]
    public float sensitivity = 15;
    public float rotationsmoothTime = 0.546f;

    [Header("Distance from Player")]
    public float distFromPlayer = 10;
    public Vector2 pitchMinMax ;


    [Header("Debug current camera rotations and eulers")]
    public Vector3 currentRotation;
    public Vector3 eul;
    Vector3 smoothingVelocity;
    Vector3 temp;
    float yaw;
    float pitch;

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
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        if (invX)
        {
            yaw -= Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        }
        if (!invX)
        {
            yaw += Input.GetAxis("CamX") * sensitivity + Input.GetAxis("MouseX") * sensitivity;
        }

        if (invY)
        {
            invertY();
        }
        if (!invY)
        {
            nonInvertY();
        }
        transform.eulerAngles = currentRotation;
        eul = transform.eulerAngles;
        eul.x = 0;
        transform.eulerAngles = currentRotation;
        transform.position = Player.position - transform.forward * distFromPlayer;
    }
    public void invertY()
    {
        if (currentRotation.x < 90)
        {
            pitch += Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        }
        if (currentRotation.x >= 88 && currentRotation.x <= 92)
        {
            smoothingVelocity = smoothingVelocity * 2;
        }
        if (currentRotation.x > 90)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 180), ref smoothingVelocity, rotationsmoothTime);
        }
    }
    public void nonInvertY()
    {
        if (currentRotation.x < 90)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        }
        if (currentRotation.x >= 88 && currentRotation.x <= 92)
        {
            smoothingVelocity = smoothingVelocity * 2;
        }
        if (currentRotation.x > 90)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 180), ref smoothingVelocity, rotationsmoothTime);
        }
    }
    public void switchInverseY()
    {
        invY = !invY;
    }
    public void switchInverseX()
    {
        invX = !invX;
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