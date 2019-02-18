// Jumping Enemy AI Script
// Created by Brianna Stone [18/02/2019]

using UnityEngine;

public class JumpingEnemy : MonoBehaviour {

    public int speed = 3;
    public int jumpForce;
    public float jumpTime; // Time between jumps

    [SerializeField] bool isGrounded;

    private Rigidbody rB;

    void Start ()
    {
        rB = GetComponent<Rigidbody>();

        jumpForce = Random.Range(6, 12);
        jumpTime = Random.Range(3, 6);
    }
	
	void Update ()
    {
        if (jumpTime > 0)
        {
            jumpTime -= Time.deltaTime;
        }
        
        if (jumpTime <= 0 && isGrounded)
        {
            Jump();
        }

    }

    void Jump()
    {
        rB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //rB.AddForce(Vector3.forward * speed, ForceMode.Impulse);

        // Set new randomized jumpForce and jumpTime

        jumpForce = Random.Range(6, 12);
        jumpTime = Random.Range(2, 4);

        // Rotate enemy based on Jumping or Falling

        //if (transform.up.z > 0)
        //{
        //    rB.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        //}
        //if (transform.up.z < 0)
        //{
        //    rB.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        //}

        // Rotate enemy for jumping back and forth (used with forward AddForce above) --- NOT WORKING
        
        //if (isGrounded)
        //{
        //    Vector3 backFace = new Vector3(0, 180, 0);
        //    rB.rotation = Quaternion.Euler(backFace);
        //
        //    if (rB.rotation == Quaternion.Euler(backFace))
        //    {
        //        rB.rotation = Quaternion.identity;
        //    }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
