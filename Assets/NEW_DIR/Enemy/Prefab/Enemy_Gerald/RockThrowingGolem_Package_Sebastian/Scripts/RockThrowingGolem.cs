using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Created By Sebastian Borkowski: 10/10/2018.
// Last Updated: 10/31/2019.
public class RockThrowingGolem : MonoBehaviour, IKillable
{
    //Animator
    public Animator animator;

    // Misc
    public int health = 1;
    private Vector3 imp = new Vector3(0, 15, 0);

    // Rock Throwing Portion
    public Transform RockSpawnPosition;
    [Tooltip("This has to have a rigidbody with usegravity set to off, collider disabled")]
    public GameObject RockProjectileObject;
    [Tooltip("Speed at which the Rock is hurtling towards the player.")]
    public float RockSpeed;
    private Transform Player;
    private GameObject Rock;

    // Lead Target Calculation Requirements
    private Vector3 AITarget;
    private Rigidbody PlayerRigidBody;
    private float DistanceCheckPlayer;
    private float MinusTargetHeight = 5; // 4.4 , 5

    // Find New Sleep Position
    [Tooltip("Use an Empty GameObject to mark the middle of his return area. Make sure it is level with the ground.")]
    public Transform middle;
    [Tooltip("Set the size of x and z to whatever you deem necessary, but KEEP y ZERO !!")]
    public Vector3 size;
    //private float WalkSpeed = 1;
    private Vector3 center;
    private Vector3 pos;
    private bool SleepBool;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerRigidBody = Player.GetComponent<Rigidbody>();
        center = new Vector3(middle.position.x, middle.position.y, middle.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        DistanceCheckPlayer = Vector3.Distance(this.transform.position, Player.position);
        CalculateLead();
        AITarget = new Vector3(AITarget.x, AITarget.y + DistanceCheckPlayer / MinusTargetHeight, AITarget.z); // Calculates how high it has to aim to hit the Player.
        RockSpawnPosition.LookAt(AITarget);


        if (SleepBool == true && animator.GetBool("isInWakeUpRange") == false)
        {
            Vector3 TargetRotation = new Vector3(pos.x, animator.transform.position.y, pos.z); // pos.y
            var r = Quaternion.LookRotation(TargetRotation - animator.transform.position);
            animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, 180 * Time.deltaTime);
            //animator.transform.position = Vector3.MoveTowards(animator.transform.position, pos, WalkSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(this.transform.position, pos) < 1)
        {
            SleepBool = false;
            animator.SetBool("HasArrivedAtNewSleepPos", true);
        }

        if (animator.GetBool("StartNewSleepPosFunction") == true)
        {
            StartCoroutine(SetNewSleepPos());
        }
    }

    IEnumerator SetNewSleepPos()
    {
        pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        SleepBool = true;
        yield return new WaitForSeconds(0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size); // Gizmo for the area of the potential sleep position.

        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, 20); // Green zone is for throw.
        Gizmos.color = new Color(0, 0, 1, 0.2f);
        Gizmos.DrawSphere(transform.position, 15); // Blue zone is for the charge.
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, 5); // Red zone is for the melee.
        Gizmos.color = new Color(1, 1, 1, 0.2f);
        Gizmos.DrawSphere(pos, 1); // Shows where the new sleep posiyion is in the red area.
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(AITarget, 0.1f); // Shows a floating white spere that the AI is aiming for. ie: Target Leader.
    }

    

    public void PickUpRock()
    {
        Rock = Instantiate(RockProjectileObject, RockSpawnPosition.position, RockSpawnPosition.rotation);
        Rock.transform.parent = RockSpawnPosition.transform;
    }

    public void ThrowRock()
    {
        Rock.transform.parent = null;
        Rock.GetComponent<Rigidbody>().useGravity = enabled;
        Rock.GetComponent<Rigidbody>().velocity = Rock.transform.forward * RockSpeed;
    }

    public void ActivateRockCol()
    {
        Rock.GetComponent<Collider>().enabled = true;
    }

    Vector3 CalculateLead() // Calucates how far it has to shoot ahead to hit the player.
    {
        Vector3 V = PlayerRigidBody.velocity;
        Vector3 D = Player.position - this.transform.position;
        float A = V.sqrMagnitude - RockSpeed * RockSpeed;
        float B = 2 * Vector3.Dot(D, V);
        float C = D.sqrMagnitude;
        if (A >= 0)
        {
            //Debug.LogError("No Lead Target solution exists");
            return Player.position;
        }
        else
        {
            float rt = Mathf.Sqrt(B * B - 4 * A * C);
            float dt1 = (-B + rt) / (2 * A);
            float dt2 = (-B - rt) / (2 * A);
            float dt = (dt1 < 0 ? dt2 : dt1);
            return AITarget = Player.position + V * dt;
        }
    }

    public IEnumerator CheckHit(bool x)
    {
        Player.GetComponent<Rigidbody>().AddForce(imp, ForceMode.Impulse);
        Debug.Log("Has been hit!!");
        if(health > 0)
        {
            StartCoroutine(TakeDamage());
        }
        else
        {
            StartCoroutine(Die());
        }
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator TakeDamage()
    {
        health -= health;
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator Die()
    {

        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

}
