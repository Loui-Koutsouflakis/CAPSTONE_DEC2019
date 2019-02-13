using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControl : MonoBehaviour {

    Animator anim;
    Rigidbody rb;

    public float speedF;
    public float startSpeed;
    public float maxSpeed;
    public float rotateSpeed;

    public float HP;

    public float jumpForce;
    public bool isGrounded;

    

    //private float sideSpeed = 500;
    //private float forwardSpeed = 1000;


    // Use this for initialization
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        isGrounded = true;
        speedF = startSpeed;
        HP = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP > 0)
        {
            rotate();
            accel();
            jump();
            attack();

            anim.GetComponent<Animator>().SetFloat("speed", speedF);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            HP = 0;
            anim.SetTrigger("dead");
        }

        
    }

    private void FixedUpdate()
    {
        if (HP > 0)
        {
            Move();
        }
    }


    void Move()
    {
        Vector3 forwards = Input.GetAxis("Vertical") * speedF * transform.forward * Time.fixedDeltaTime;
        Vector3 sides = Input.GetAxis("Horizontal") * 0 * transform.right * Time.fixedDeltaTime;

        rb.AddForce(forwards + sides);      
    }

    void rotate()
    {
        if (Input.GetKey("a"))
        {
            gameObject.transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        }
        else if (Input.GetKey("d"))
        {
            gameObject.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    void accel()
    {
        if (Input.GetKey("w") && speedF<=maxSpeed)
        {
            speedF+= 10;
        }

        if (Input.GetKeyUp("w"))
        {
            speedF = startSpeed;
        }
    }


    void jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            anim.SetTrigger("jump");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            isGrounded = true;
        }
    }
       
    private void attack()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("shoot1");
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            anim.SetTrigger("shoot2");
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("punch");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("spell");
        }
    }

    
}
