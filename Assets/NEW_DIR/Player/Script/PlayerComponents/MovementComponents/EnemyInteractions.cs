using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteractions : MonoBehaviour
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
    protected readonly float decelFactor = 0.14f;

    //public - to test balance etc.
    //for movement


    private float frictionCoeff = 0.2f;

    public bool grounded;

    public bool enteredScript;

    //for jumps



    PlayerClass player;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerClass>();

        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cammy = camObject.GetComponent<Camera>();
        enteredScript = true;
    }

    public void OnEnable()
    {
        enteredScript = true;
    }

    public void OnDisable()
    {
        enteredScript = false;
    }

    //public void Stuck()
    //{
    //    //same movement controls as normal, execpt slowe
    //    //movement based on direction camera is facing
    //    Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
    //    Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
    //    cammyRight.y = 0;
    //    cammyFront.y = 0;
    //    cammyRight.Normalize();
    //    cammyFront.Normalize();

    //    //rotates the direction the character is facing to the correct direction based on camera
    //    player.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical + cammyRight * horizontal, crouchRotateSpeed * Time.fixedDeltaTime, 0.0f));
    //}
}
