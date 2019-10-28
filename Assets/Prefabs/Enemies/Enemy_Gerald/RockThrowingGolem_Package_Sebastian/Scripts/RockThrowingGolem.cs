using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Created By Sebastian Borkowski: 10/10/2018.
// Last Updated: 10/24/2019.
public class RockThrowingGolem : MonoBehaviour //, IKillable
{
    public Animator animator;
    public Transform RockSpawnPosition;
    [Tooltip("This has to be a cube primitive with a rigidbody usegravity set to off, box collider disabled")]
    public GameObject RockProjectileObject; // For now the rock has to be a cude with RigidBody gravity set to off and Box collider disabled.
    public float RockSpeed;

    private Transform Player;

    public Transform middle;
    public Vector3 size;
    private Vector3 center;
    private float WalkSpeed = 1;

    //public NavMeshAgent agent;

    bool test;

    GameObject Rock;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        center = new Vector3(middle.position.x, middle.position.y, middle.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        RockSpawnPosition.LookAt(Player);


        if (test == true && animator.GetBool("isInWakeUpRange") == false)
        {
            // No Navmesh
            Vector3 TargetRotation = new Vector3(pos.x, pos.y, pos.z);
            var r = Quaternion.LookRotation(TargetRotation - animator.transform.position);
            animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, r, 180 * Time.deltaTime);
            animator.transform.position = Vector3.MoveTowards(animator.transform.position, pos, WalkSpeed * Time.deltaTime);

        }

        if(Vector3.Distance(this.transform.position, pos) < 1)
        {
            test = false;
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
        test = true;
        yield return new WaitForSeconds(0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size); // transform.localPosition + 

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, 20);
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, 15);
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, 5);
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(pos, 1);
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
        Rock.GetComponent<BoxCollider>().enabled = true;
        Rock.GetComponent<Rigidbody>().velocity = Rock.transform.forward * RockSpeed;
    }

    //public IEnumerator CheckHit()
    //{
    //    Debug.Log("CheckHit()");
    //    yield return new WaitForSeconds(1f);
    //}

    //public IEnumerator TakeDamage()
    //{
    //    Debug.Log("TakeDamage()");
    //    yield return new WaitForSeconds(1f);
    //}
    //public IEnumerator Die()
    //{
    //    Debug.Log("Die()");
    //    yield return new WaitForSeconds(1f);
    //}
}

