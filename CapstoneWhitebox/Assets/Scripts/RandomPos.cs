using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Blink Enemy by Fatin Majumder 
// Enemy teleports around the player within a fixed distance in a set intervals
// Follows and attacks the player before the next blink

public class RandomPos : MonoBehaviour
{
    // Variable to store the duration before blink
    private float BlinkTimer = 15f;

    // Variable to store the duration of fire
    public float FireTimer = 3f;

    // Referance to the game object which is the player
    public GameObject Player;

    // Arrays to store the position values from the player 
    private int[] xPos = { 0, 4, 8, -4, -8 };
    private int[] zPos = { 0, 4, 8, -4, -8};
    private int[] yPos = { 2, 4, 8 };

    // Offset uses the values from the arrays to generate a new blink point/ location
    private Vector3 offset;
    
    // Used with VEctor3.lerp to follow the player slowly
    private float lurpSpeed = 0.5f;

    // Used with Slerp to rotate and face the player all the time
    private int dampSpeed = 3;


    //Projectile shooting variables, reference to the game object and the spawnpoints
    public GameObject projectile;
    public Transform projectilespawnPoint;
    private float projectileForce = 20f;


   
    // Start is called before the first frame update
    void Start()
    {
        // Initializing the first offset from the player
        // Without it the enemy lerps to the player and touches him if the player doesn't move
        offset = new Vector3(0, 4, 8);
    }

    // Update is called once per frame
    void Update()
    {      
        TransformPos();
        
    }

    // Starts the blink timer
    // Selects a random position from the arrays and assigns to the offset
    // Resets the blink timer
    // Vector3.lerp is used to follow the player while timer starts back
    // Shoots projectiles at a set interval of time
    void TransformPos()
    {
        // Starts the countdown backwards
        BlinkTimer -= Time.deltaTime;
        if (BlinkTimer <= 0)
        {
            // Randomizes the x,y,z values for the offset
            int RandX = Random.Range(0, (xPos.Length));
            int RandZ = Random.Range(0, (zPos.Length));
            int RandY = Random.Range(0, (yPos.Length));

            // Assigns the offset values
            offset = new Vector3(xPos[RandX], yPos[RandY], zPos[RandZ]);

            // Assigns the position of the enemy which is offset + the position of the player
            // This does not have lerp so that it blinks / teleportes
            transform.position = Player.transform.position + offset;

            // Resets the blink timer
            BlinkTimer = 20f;           
        }

        // This time it has the lerp so that while the player is moving the enemy moves slowly with him
        transform.position = Vector3.Lerp(transform.position, Player.transform.position + offset, lurpSpeed * Time.deltaTime);

        // calculates the angle it has to be rotated
        var rotationAngle = Quaternion.LookRotation(Player.transform.position - transform.position);

        // Rotates the enemy to face the player with slerp and dampSpeed 
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, Time.deltaTime * dampSpeed); 

        // Fires while the timer is between a set range of time 
        if (BlinkTimer <= 10 && BlinkTimer >= 5)
        {
            Fire();
        }
    }

    // Fire function to shoot projectile at the player at set intervals. 
    // Starts the backwards countdown of firetime
    // Instantiates the object from the spawnpoint which is an empty gameObject - child of the enemy
    // Shoots straight from the spawnpoint 
    // Resets timer
    void Fire()
    {
        // Fire timer starts backwards countdown
        FireTimer -= Time.deltaTime;
        if(FireTimer <=0)
        {
            // Instantiates a projectile from the spawnpoint which is a child gameObject to the enemy. 
            GameObject spawnedProjectile = Instantiate(projectile, projectilespawnPoint.position, Quaternion.identity);
            

            // Shoots the projectile foward to which the enemy is facing with an impulse instead of constant force
            spawnedProjectile.GetComponent<Rigidbody>().AddForce(projectileForce * transform.forward, ForceMode.Impulse);

            // TImer resets
            FireTimer = 2f;
        }
    }
}
