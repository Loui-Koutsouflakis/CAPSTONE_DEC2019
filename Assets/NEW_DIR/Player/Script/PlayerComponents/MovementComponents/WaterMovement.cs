using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : PlayerVariables
{

    //rigidbody
    public Rigidbody rb;

    //camera object
    public Camera cammy;

    //raycasts
    private RaycastHit footHit;

    //vectors
    private readonly Vector3 halves = new Vector3(0.34f, 0.385f, 0.34f);

    //priamtives
    //private
    protected float horizontal;
    protected float vertical;
    protected bool deadJoy;
    protected readonly float deadZone = 0.028f;
    protected readonly float decelFactor = 0.4f;
    public float waterCheckDist = 100;
    LayerMask p_LayerMask = 1 << 9;
    RaycastHit water;
    Vector2 g;
    private float frictionCoeff = 0.2f;

    public bool grounded;

    public bool enteredScript;
    PlayerClass player;

    // Start is called before the first frame update
    void Start()
    {
        g = Physics.gravity;
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerClass>();
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cammy = camObject.GetComponent<Camera>();
        p_LayerMask = ~p_LayerMask;
        enteredScript = false;
    }

    public void OnEnable()
    {
        enteredScript = true;
    }

    public void OnDisable()
    {
        enteredScript = false;
    }

    void FixedUpdate()
    {
        ControlInput();
    }

    public void swim()
    {
        if(rb.velocity.magnitude < swimMax)
        rb.AddForce(cammy.transform.forward * EaseIn(swimSpeed/swimMax), ForceMode.Impulse);
        Friction();
    }

    public void ControlInput()
    {
        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        Movement();
        Friction();
        ApplyVelocityCutoff();
    }

    public void Movement()
    {


        //rotates the direction the character is facing to the correct direction based on camera
        //player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront + cammyRight * horizontal, swimRotateSpeed * Time.fixedDeltaTime, 0.0f));
        player.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

    }

    void ApplyVelocityCutoff()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;

        horizontalVelocity = Mathf.Min(horizontalVelocity.magnitude, swimMax) * horizontalVelocity.normalized;

        rb.velocity = horizontalVelocity + rb.velocity.y * Vector3.up;
    }

    void Friction()
    {
        float friction = frictionCoeff * rb.mass;
        Vector3 groundNormal = footHit.normal;
        Vector3 normalComponentofVelocity = Vector3.Dot(rb.velocity, groundNormal) * groundNormal;
        Vector3 parallelComponentofVelocity = rb.velocity - normalComponentofVelocity;
        rb.AddForce((-friction * 2) * parallelComponentofVelocity);
    }

    public void Decel()
    { 
        if (rb.velocity.magnitude > 1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelFactor);
        }
    }

    public bool isUnderWater()
    {
        Vector3 lineStart = rb.transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y + waterCheckDist, lineStart.z);
        Debug.DrawLine(lineStart, vectorToSearch, Color.black);
        return Physics.Linecast(lineStart, vectorToSearch, out water, p_LayerMask);
    }
}
