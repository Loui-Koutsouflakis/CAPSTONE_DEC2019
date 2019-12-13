using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Created By Sebastian Borkowski: 10/10/2018.
// Last Updated: 12/9/2019.
public class RockThrowingGolem : MonoBehaviour, IKillable
{
    //Animator
    public Animator animator;

    // Misc
    public int health;
    //[Tooltip("This the knockback force")]
    //public Vector3 KnockBackForce = new Vector3(0, 5, 0);
    public Collider Weakpoint;
    [Tooltip("This is only for the DrawGizmos")]
    public float MeleeRadius, ChargeRadius, ThrowRadius;

    // Rock Throwing Portion
    public bool IsTurretEdition;
    public Transform RockSpawnPosition;
    //[Tooltip("This has to have a rigidbody with usegravity set to off, collider disabled")]
    //public GameObject RockProjectileObject;
    [Tooltip("Speed at which the Rock is hurtling towards the player.")]
    public float RockSpeed;
    private Transform Player;
    private GameObject Rock;
    public Collider LeftHand, RightHand;

    // Lead Target Calculation Requirements
    private Vector3 AITarget;
    private Rigidbody PlayerRigidBody;
    private float DistanceCheckPlayer;
    public float MinusTargetHeight; // 4.4 , 5

    // Find New Sleep Position
    [Tooltip("Use an Empty GameObject to mark the middle of his return area. Make sure it is level with the ground.")]
    public Transform middle;
    [Tooltip("Set the size of x and z to whatever you deem necessary, but KEEP Y ZERO !!")]
    public Vector3 size;
    private float WalkSpeed = 2;
    private Vector3 center;
    private Vector3 pos;
    private bool SleepBool;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        PlayerRigidBody = Player.GetComponent<Rigidbody>();
        center = new Vector3(middle.position.x, middle.position.y, middle.position.z);
        //Weakpoint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (IsTurretEdition)
        {
            if (DistanceCheckPlayer <= 50 && DistanceCheckPlayer >= 40)
            {
                MinusTargetHeight = 4f;
            }
            else if (DistanceCheckPlayer <= 40 && DistanceCheckPlayer >= 35)
            {
                MinusTargetHeight = 4.5f;
            }
            else if (DistanceCheckPlayer <= 35 && DistanceCheckPlayer >= 25)
            {
                MinusTargetHeight = 6f;
            }
            else if (DistanceCheckPlayer <= 25 && DistanceCheckPlayer >= 20)
            {
                MinusTargetHeight = 6.5f;
            }
            else if (DistanceCheckPlayer <= 20 && DistanceCheckPlayer >= 10)
            {
                MinusTargetHeight = 7f;
            }
            else if (DistanceCheckPlayer <= 10 && DistanceCheckPlayer >= 5)
            {
                MinusTargetHeight = 7.5f;
            }
        }

        DistanceCheckPlayer = Vector3.Distance(this.transform.position, Player.position);
        CalculateLead();
        AITarget = new Vector3(AITarget.x, AITarget.y + DistanceCheckPlayer / MinusTargetHeight, AITarget.z); // Calculates how high it has to aim to hit the Player.
        RockSpawnPosition.LookAt(AITarget);



        if (SleepBool == true && animator.GetBool("isInWakeUpRange") == false)
        {
            Vector3 TargetRotation = new Vector3(pos.x, animator.transform.position.y, pos.z); // pos.y
            var rotation = Quaternion.LookRotation(TargetRotation - animator.transform.position);
            animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, rotation, 180 * Time.deltaTime);
            animator.transform.position = Vector3.MoveTowards(animator.transform.position, pos, WalkSpeed * Time.deltaTime);
        }

        if (animator.GetBool("StartNewSleepPosFunction") == true)
        {
            StartCoroutine(SetNewSleepPos());
        }

        if (Vector3.Distance(this.transform.position, pos) < 1)
        {
            SleepBool = false;
            animator.SetBool("HasArrivedAtNewSleepPos", true);
        }
        else
        {
            animator.SetBool("HasArrivedAtNewSleepPos", false);
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
        Gizmos.DrawSphere(transform.position, ThrowRadius); // Green zone is for throw.
        Gizmos.color = new Color(0, 0, 1, 0.2f);
        Gizmos.DrawSphere(transform.position, ChargeRadius); // Blue zone is for the charge.
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, MeleeRadius); // Red zone is for the melee.
        Gizmos.color = new Color(1, 1, 1, 0.2f);
        Gizmos.DrawSphere(pos, 1); // Shows where the new sleep posiyion is in the red area.
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(AITarget, 0.1f); // Shows a floating white spere that the AI is aiming for. ie: Target Leader.
    }

    

    public void PickUpRock()
    {
        Rock = RTG_RockPooler.SharedInstance.GetPooledRock();
        if (Rock != null)
        {
            Rock.transform.position = RockSpawnPosition.position;
            Rock.transform.rotation = RockSpawnPosition.rotation;
            Rock.SetActive(true);
            Rock.transform.parent = RockSpawnPosition.transform;
        }

        Rock.transform.parent = RockSpawnPosition.transform;
        //Rock = Instantiate(RockProjectileObject, RockSpawnPosition.position, RockSpawnPosition.rotation);
    }

    public void ThrowRock()
    {

        Rock.transform.parent = null;
        Rock.GetComponent<Rigidbody>().useGravity = enabled;
        Rock.GetComponent<Rigidbody>().isKinematic = false;
        Rock.GetComponent<Rigidbody>().velocity = Rock.transform.forward * RockSpeed;
    }

    public void ActivateRockCol()
    {
        Rock.GetComponent<Collider>().enabled = true;
    }

    public void ActivateHandCol()
    {
        LeftHand.enabled = true;
        RightHand.enabled = true;
    }
    public void DeactivateHandCol()
    {
        LeftHand.enabled = false;
        RightHand.enabled = false;
    }

    public void ActivateWeakPoint()
    {
        Weakpoint.enabled = true;
    }
    public void DeactivateWeakPoint()
    {
        Weakpoint.enabled = false;
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
        //Player.GetComponent<Rigidbody>().AddForce(KnockBackForce, ForceMode.Impulse);
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
        health -= 1;
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator Die()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

}
