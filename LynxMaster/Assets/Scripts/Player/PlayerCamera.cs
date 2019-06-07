using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform cameraIdlePosition;

    [Header("Turn on/off the mouse cursor")]
    //public bool lockCursor = false;

    [Header("Invert Camera Y (set to off for mouse control)")]
    public bool invY;

    [Header("Invert Camera X(NOT RECOMMENDED)")]
    public bool invX;

    [Header("Orbit/Hemisphere Camera(RightJoy Click)")]
    public bool orbit;

    [Header("Player and world position")]
    public Transform Player;

    [Header("Camera Sensitivity and rotation smoothing(sensitivity = 2, rotation = 3.54)")]
    public float sensitivity = 2;
    public float rotationsmoothTime = 0.351f;

    [Header("Distance from Player")]
    public float distFromPlayer = 10;
    public Vector2 pitchMinMax;

    [Header("Debug current camera rotations and eulers")]
    public Vector3 currentRotation;
    public Vector3 eul;
    Vector3 smoothingVelocity;
    Vector3 temp;
    float yaw;
    float pitch;

    void Start()
    {

        temp = transform.position;

        //if (lockCursor)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
    }
    private void Update()
    {
        if (Input.GetButtonDown("RightJoyClick"))
        {
            toggleOrbit();
        }

        if (Input.GetButton("RightJoyClick"))
        {
            StartCoroutine(holdJoy());
        }

    }
    private void LateUpdate()
    {
        CamMovement();
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
        if (orbit)
        {
            pitchMinMax = new Vector2(0, 180);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        }
        if (!orbit)
        {
            pitchMinMax = new Vector2(0, 90);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        }
        eul = transform.eulerAngles;
        eul.x = 0;
        transform.eulerAngles = currentRotation;
        transform.position = Player.position - transform.forward * distFromPlayer;
    }
    public void invertY()
    {
        if (currentRotation.x <= 90)
        {
            pitch += Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        }
        if (currentRotation.x >= 88 && currentRotation.x <= 92 && orbit)
        {
            smoothingVelocity = smoothingVelocity * 4;
        }
        if (currentRotation.x > 90 && orbit)
        {

            pitch += Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 180), ref smoothingVelocity, rotationsmoothTime);
        }
        if (currentRotation.x > 90 && !orbit)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        }
    }
    public void nonInvertY()
    {
        if (currentRotation.x <= 90)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
        }
        if (currentRotation.x >= 88 && currentRotation.x <= 92 && orbit)
        {
            smoothingVelocity = smoothingVelocity * 2;
        }
        if (currentRotation.x > 90 && orbit)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 180), ref smoothingVelocity, rotationsmoothTime);
        }
        if (currentRotation.x > 90 && !orbit)
        {
            pitch -= Input.GetAxis("CamY") * sensitivity + Input.GetAxis("MouseY") * sensitivity;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
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
    public void toggleOrbit()
    {
        orbit = !orbit;
    }
    public IEnumerator holdJoy()
    {
        yield return new WaitForSeconds(1f);
        transform.LookAt(Player);
        transform.position = cameraIdlePosition.position;
    }
}
