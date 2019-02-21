// Jumping Enemy AI Script
// Created by Brianna Stone [18/02/2019]

using UnityEngine;

public class JumpingEnemy : MonoBehaviour {

    public int speed;
    public int jumpForce;
    public float jumpDelay; // Time between jumps

    [SerializeField] bool isGrounded;
    [SerializeField] bool hasJumped;

    private Rigidbody rB;

    void Start ()
    {
        rB = GetComponent<Rigidbody>();
        rB.drag = 0.3f;

        jumpForce = Random.Range(6, 12);
        jumpDelay = Random.Range(3, 6);
    }
	
	void Update ()
    {
        if (jumpDelay > 0)
        {
            jumpDelay -= Time.deltaTime;
        }
        
        if (jumpDelay <= 0 && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        // Change enemy rotation for jumping back and forth

        if (isGrounded && hasJumped)
        {
            Vector3 backFace = new Vector3(0, 180, 0);

            transform.Rotate(backFace, Space.Self);
        }

        rB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        rB.AddForce(transform.forward * speed, ForceMode.Impulse);

        // Set new randomized jumpForce and jumpTime

        jumpForce = Random.Range(6, 12);
        jumpDelay = Random.Range(2, 4);
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
            hasJumped = true;
        }
    }
}
