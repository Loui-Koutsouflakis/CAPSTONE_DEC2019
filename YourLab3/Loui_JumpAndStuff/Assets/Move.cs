using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    #region Public Variables
    [Header("Player Movement")]
    public Rigidbody rb;
    public bool grounded;

    [Header("Camera")]
    public float speedH = 2.2f;
    public float speedV = 2.2f;
    public Transform camPivot;
    public Transform refPivot;
    public Transform camFollow;
    #endregion Public Variables

    #region Object Variables
    private Camera cammy;
    private CapsuleCollider capCol;
    private RaycastHit footHit;
    #endregion Object Variables

    #region Vector Variables
    private Vector3 fw;
    private Vector3 rt;
    private Vector3 storeVelocity;
    private Vector3 movementDir;
    private readonly Vector3 flyForce = new Vector3(0f, 4f, 0f);
    private readonly Vector3 lessOneY = new Vector3(0f, 0.7f, 0f);
    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);
    private readonly Vector3 dive = new Vector3(0, 2.5f, 21f);
    #endregion Vector Variables

    #region Primitive Variables
    private float horizontal;
    private float vertical;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float walkAccel = 66f;
    private float decel = 11.8f;
    private float gravityArc = 0.9f;
    private bool deadJoy;
    private bool jumpHoldy;
    private bool canAccel;
    private readonly float jumpForce = 13.2f;
    private readonly float jumpHeight = 28f;
    private readonly float airMax = 13f;
    private readonly float flyHeight = 10f;
    private readonly float walkMax = 8f;
    private readonly float deadZone = 0.028f;
    private readonly float groundGravity = 4.15f;
    private readonly float dropGravity = 7.1f;
    private readonly float gravitySlope = 1.7f;
    private readonly float initJumpGravity = 0.9f;
    private readonly float camLerpFactor = 0.042f;
    private readonly float camPitchMax = 60f;
    private readonly float decelFactor = 0.14f;
    private readonly float velocityDivider = 3.6f;
    private readonly float airControlAccel = 29.2f;
    private readonly float groundControlAccel = 66f;
    private readonly float diveMultiplierXZ = 55f;
    private readonly float diveMultiplierXZ2 = 920f;
    private readonly float diveMultiplierY = 1020f;
    private readonly float diveMultiplierY2 = 1750f;
    #endregion Primitive Types

    void Start()
    {
        grounded = true;
        jumpHoldy = false;
        cammy = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ControlInput();
        GravityControl();
        CamUpdate();
    }

    #region Methods
    public void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJ");
        vertical = Input.GetAxis("VerticalJ");
        yaw += speedH * Input.GetAxis("CamX");
        pitch -= speedV * Input.GetAxis("CamY");
        fw = refPivot.gameObject.transform.forward;
        rt = cammy.gameObject.transform.right;
        movementDir = (fw * vertical) + (rt * horizontal);
        movementDir.Normalize();

        if(pitch > camPitchMax)
        {
            pitch = camPitchMax;
        }

        if(pitch < 0f)
        {
            pitch = 0f;
        }

        if (Input.GetButtonDown("JumpJ"))
        {
            Jump();
        }

        if (Input.GetButtonUp("JumpJ"))
        {
            jumpHoldy = false;
        }

        if (Input.GetButtonDown("BeakJ"))
        {
            if (!grounded)
            {
                Dive();
            }
        }

        if (horizontal > deadZone || horizontal < -deadZone || vertical > deadZone || vertical < -deadZone)
        {
            Run();
        }
        else if (horizontal <= deadZone && horizontal >= -deadZone && vertical <= deadZone && vertical >= -deadZone)
        {
            Decel();
        }

        if (grounded)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, walkMax);
        }
        else if (!grounded)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, airMax);
        }
    }

    void GravityControl()
    {
        if (jumpHoldy)
        {
            gravityArc += Time.deltaTime * gravitySlope;
            rb.AddForce(Physics.gravity * rb.mass * gravityArc);
        }
        else if (!jumpHoldy)
        {
            gravityArc = initJumpGravity;

            if (grounded)
            {
                rb.AddForce(Physics.gravity * rb.mass * groundGravity);
            }

            else if (!grounded)
            {
                rb.AddForce(Physics.gravity * rb.mass * dropGravity);
            }
        }
    }

    void CamUpdate()
    {
        cammy.transform.position = Vector3.Slerp(cammy.transform.position, camFollow.transform.position, camLerpFactor);
        cammy.transform.rotation = Quaternion.Slerp(cammy.transform.rotation, camPivot.transform.rotation, camLerpFactor);
        camPivot.transform.eulerAngles = new Vector3(Mathf.Clamp(pitch, 0f, camPitchMax), yaw, 0f);
    }

    public void Run()
    {
        transform.eulerAngles = new Vector3(0f, camPivot.rotation.eulerAngles.y, 0f);

        if (deadJoy)
        {
            deadJoy = false;
        }
        rb.rotation.SetLookRotation(movementDir, Vector3.up);
        rb.AddForce(movementDir * walkAccel, ForceMode.Force);
    }

    public void Decel()
    {
        if (!deadJoy)
        {
            deadJoy = true;
        }

        if (rb.velocity.magnitude > 1f && grounded)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelFactor);
        }
    }

    public void Jump()
    {
        if (grounded)
        {
            rb.AddForce(new Vector3(0, jumpForce + rb.velocity.magnitude / velocityDivider), ForceMode.Impulse);
            walkAccel = airControlAccel;
            jumpHoldy = true;
        }
        grounded = false;
    }

    public void Dive()
    {
        if (!deadJoy)
        {
            if (jumpHoldy)
            {
                rb.AddForce(movementDir * diveMultiplierXZ + Vector3.up * diveMultiplierY, ForceMode.Acceleration);
            }

            if(!jumpHoldy)
            {
                rb.AddForce(movementDir * diveMultiplierXZ2 + Vector3.up * diveMultiplierY2, ForceMode.Acceleration);
            }
        }

        if(deadJoy)
        {
            rb.AddRelativeForce(dive, ForceMode.Impulse);
        }
    }
    #endregion Methods

    #region Collision Control

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            if(Physics.BoxCast(transform.position, halves, Vector3.down, out footHit, Quaternion.identity, halves.y))
            {
                Debug.Log(footHit.collider.gameObject.name);

                if(footHit.collider.gameObject.tag == "Ground")
                {
                    walkAccel = groundControlAccel;
                    grounded = true;
                }
            }
        }
    }

    #endregion Collision Control
}
