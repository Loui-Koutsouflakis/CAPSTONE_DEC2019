using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public int rotationSpeed;
    public int MoveSpeed;
    public float bulletSpeed;

    public GameObject Player;
    public Transform tPlayer;    
    public Rigidbody rb;
    public GameObject Bullet;
   
    public AIControl aiControl;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       

        Move();
        Rotate();
        Detected();
        if (Input.GetMouseButton(0))
        {
            Fireball();
        }
       
    }

    void Move()
    {
        Vector3 horizontal = Input.GetAxis("Horizontal") * MoveSpeed * transform.right;   
        Vector3 forward = Input.GetAxis("Vertical") * MoveSpeed * transform.forward;

        rb.AddForce(horizontal + forward);
       
    }

    void Rotate()
    {
        float deltaRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        Vector3 newLookAtDirection = transform.forward + deltaRotation * transform.right;
        Vector3 newLookAtPoint = transform.position + newLookAtDirection;
        transform.LookAt(newLookAtPoint);
    }

    void Fireball()
    {
        GameObject tempBullet = Instantiate(Bullet, transform.position, transform.rotation) as GameObject;
        Rigidbody tempRigidBodyBullet = tempBullet.GetComponent<Rigidbody>();
        tempRigidBodyBullet.AddForce(tempRigidBodyBullet.transform.forward * bulletSpeed);
        Destroy(tempBullet, 1);
    }

    void Detected()
    {

        //Debug.DrawRay(transform.position, transform.forward, Color.red, 0f, true);
        Vector3 Start = transform.position;
        Vector3 Direction = transform.forward;
        Direction.Normalize();

        float Distance = 3.5f;

        RaycastHit[] hits = Physics.RaycastAll(Start, Direction, Distance);
        bool EnemyFound = false;

        for (int it = 0; it < hits.Length; it++)
        {
            if (hits[it].collider != null && hits[it].collider.tag == "Enemy")
            {
                EnemyFound = true;
                Debug.Log("Enemy Found");
            }
        }

        if (EnemyFound)
        {
            Debug.DrawLine(Start, Start + (Direction * Distance), Color.green, 1f, false);
            aiControl.MoveSpeed = 0;
        }
        else
        {
            Debug.DrawLine(Start, Start + (Direction * Distance), Color.red, 1f, false);
            aiControl.MoveSpeed = 0.5f;
        }   
    }
}
