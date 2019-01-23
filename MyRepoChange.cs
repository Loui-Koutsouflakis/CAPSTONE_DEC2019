using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{

    public float forwardSpeed = 10;
    public float sidewaySpeed = 5;
    private static PlayerControls Instance;
    private Rigidbody rb;
    public int index;
    public int Windex;



    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public static PlayerControls GetInstance()
    {
        return Instance;
    }
    private void Move()
    {
        Vector3 forward = Input.GetAxis("Vertical") * forwardSpeed * transform.forward;
        Vector3 sideways = Input.GetAxis("Horizontal") * sidewaySpeed * transform.right;
        Vector3 moveDir = forward + sideways;
        anim.SetFloat("DirectionX", Vector3.Dot(moveDir.normalized, transform.right));
        anim.SetFloat("DirectionY", Vector3.Dot(moveDir.normalized, transform.forward));
        rb.AddForce(forward + sideways);
    }
}
