// Jumping Enemy AI Script
// Created by Brianna Stone [18/02/2019]

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class JumpingEnemy : MonoBehaviour {

    public float speed;
    [Header("Speed modifier for split enemies")]
    public float speedMod = 1.3f;
    public int jumpForce;
    public float jumpDelay; // Time between jumps

    public int maxHP = 2;
    public int currentHP;

    [SerializeField] bool isGrounded;
    [SerializeField] bool hasJumped;
    bool split;

    [Header("Insert prefab for enemy instatiation")]
    public GameObject entity;
    private int rotation;

    public List<Transform> spawnPoints = new List<Transform>();

    public LayerMask pLayer;

    private Rigidbody rB;

    void Start ()
    {
        rB = GetComponent<Rigidbody>();
        rB.drag = 0.3f;

        jumpForce = UnityEngine.Random.Range(6, 12);
        jumpDelay = UnityEngine.Random.Range(3, 6);

        currentHP = maxHP;
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

        rotation = UnityEngine.Random.Range(1, 361);

        if (currentHP <= 0)
        {
            gameObject.SetActive(false);
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

        jumpForce = UnityEngine.Random.Range(6, 12);
        jumpDelay = UnityEngine.Random.Range(2, 4);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Player")
        {
            RaycastHit hit;

            // Would prefer this to be box or sphere cast but I can't get either working?

            if (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, pLayer))
            {
                currentHP--;

                //Change size and speed of new spawned enemies
                entity.transform.localScale /= 1.2f;
                speed *= speedMod;

                //Split enemy into smaller enemies
                for (int x = 0; x < 4; x++)
                {
                    //Check if spawned, and minimum size to spawn new enemies
                    if (!split && entity.transform.localScale.y > 0.9f)
                    {
                        Instantiate(entity, spawnPoints[x].transform.position, Quaternion.Euler(0, rotation, 0));
                        split = true;
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
            hasJumped = true;
        }

        if (collision.gameObject.tag == "Player")
        {
            split = false;
        }
    }
}
