using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpinGolem : MonoBehaviour, IKillable
{
    public Rigidbody rb;
    public Transform playerTf;
    public Animator anim;
    public Collider[] colliders;
    public State state;
    float spinRate = 820f;
    public float moveSpeed = 16.1f;
    public float spinAccel = 6f;
    bool hitByPlayer;
    int wanderCheckLimit;
    float wanderAmount;
    readonly float vulnerableTime = 8f;
    readonly float viewRadius = 24f;
    readonly float spinRadius = 12f;
    readonly float wanderCheckDistance = 3f;
    Vector3 wanderRand;
    Vector3 directionToPlayer;
    RaycastHit wanderHit;

    public NavMeshAgent agent;

    [Header("RENDER DEBUG")]
    public MeshRenderer[] rends;
    public Material dissolveMat;

    bool isStepping;
    float currentStepLerp;
    Vector3 currentStepDestination;
    float dissolveStrength = 0f;
    readonly float dissolveLerp = 5f;
    
    private void Start()
    {
        StartCoroutine(WanderSequence());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G) && state == State.Vulnerable)
        {
            SwitchState(State.Dying);
        }

        switch (state)
        {
            case State.Idle:
                if(PlayerInRange(viewRadius))
                {
                    SwitchState(State.Spin);
                }
                break;
            case State.Move:

                if (PlayerInRange(viewRadius))
                {
                    SwitchState(State.Spin);
                }
                else
                {
                    //rb.AddForce(wanderRand * moveSpeed);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerTf.position - transform.position, Vector3.up), 0.28f * Time.deltaTime);
                    //transform.position += wanderRand * moveSpeed * Time.deltaTime;
                }
                break;
            case State.Spin:
                directionToPlayer = (playerTf.position - transform.position).normalized;
                spinRate += Time.deltaTime * 100f;
                directionToPlayer.y = 0f;

                //agent.SetDestination(playerTf.position);

                //rb.velocity = directionToPlayer * spinAccel;
                //rb.angularVelocity = transform.forward * spinRate; //PLACEHOLDER

                if(PlayerInRange(spinRadius))
                {
                    StartCoroutine(SlamSequence());
                }
                break;
            case State.Attack:
                //transform.LookAt(transform.position + directionToPlayer); //PLACEHOLDER
                break;
            case State.Vulnerable:
                if(hitByPlayer)
                {
                    StartCoroutine(Die());
                }
                break;
            case State.Readjusting:
                //transform.LookAt(transform.position + directionToPlayer); //PLACEHOLDER
                break;
            case State.Dying:
                //rb.angularVelocity += Vector3.left * 50f * Time.deltaTime;
                dissolveStrength = Mathf.Lerp(dissolveStrength, 1, dissolveLerp * Time.deltaTime);

                foreach(MeshRenderer mr in rends)
                {
                    mr.material.SetFloat("_Strength", dissolveStrength);
                }

                break;
        }
    }

    public bool PlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, playerTf.position) < range;
    }

    public IEnumerator Step(float duration, float lerp, Vector3 localDestination)
    {
        currentStepDestination = localDestination;
        currentStepLerp = lerp;
        isStepping = true;
        yield return new WaitForSeconds(duration);
        isStepping = false;
    }

    public IEnumerator CheckHit(bool x) // added by lilly to make it work with updated Ikillable
    {
        yield return new WaitForSeconds(0.1f);
        if (!hitByPlayer)
        {
            StartCoroutine(TakeDamage());
        }
    }

    public IEnumerator TakeDamage()
    {
        hitByPlayer = true;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Die());
    }

    public IEnumerator Die()
    {
        SwitchState(State.Dying);
        yield return new WaitForSeconds(2f);
        SwitchState(State.Dead);
    }

    public IEnumerator SlamSequence()
    {
        SwitchState(State.Attack);
        yield return new WaitForSeconds(2f);
        SwitchState(State.Vulnerable);
        yield return new WaitForSeconds(8f);
        
        if(!hitByPlayer)
        {
            SwitchState(State.Spin);
        }
    }

    public IEnumerator ReadjustSequence()
    {
        SwitchState(State.Readjusting);
        yield return new WaitForSeconds(1f);

        if(PlayerInRange(viewRadius))
        {
            SwitchState(State.Spin);
        }
        else
        {
            StartCoroutine(WanderSequence());
        }
    }

    public IEnumerator WanderSequence()
    {
        Debug.Log("Wander Sequence");

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        wanderAmount = Random.Range(1f, 2.5f);
        wanderRand.x = Random.Range(-6f, 6f);
        wanderRand.z = Random.Range(-6f, 6f);
        Physics.Raycast(transform.position, wanderRand, out wanderHit, wanderCheckDistance);

        if(wanderHit.collider == null || wanderCheckLimit > 12)
        {
            //
            // Add Floor Check for potential destination here
            // 

            SwitchState(State.Move);
            yield return new WaitForSeconds(wanderAmount);
            SwitchState(State.Idle);
            wanderCheckLimit = 0;
            StartCoroutine(WanderSequence());
            
        }
        else
        {
            yield return new WaitForSeconds(0.03f);
            wanderCheckLimit++;
            StartCoroutine(WanderSequence());
        }
    }

    public void SwitchState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.Idle:
                //Debug.Log("SpinGolem: Idle");
                rb.angularVelocity = Vector3.zero;
                
                break;
            case State.Move:
                anim.SetTrigger("Walk");
                //Debug.Log("SpinGolem: Move");
                break;
            case State.Spin:
                //Debug.Log("SpinGolem: Spin");
                anim.SetTrigger("Spin");
                break;
            case State.Attack:
                //Debug.Log("SpinGolem: Slam");
                anim.SetTrigger("Attack");
                break;
            case State.Vulnerable:
                //Debug.Log("SpinGolem: Vulnerable");

                break;
            case State.Readjusting:
                //Debug.Log("SpinGolem: Readjusting");
                anim.SetTrigger("Recover");

                int rand = Random.Range(0, 6);

                if(rand < 3)
                {
                    anim.SetBool("ToSpin", true);
                }
                else
                {
                    anim.SetBool("ToSpin", false);
                }

                break;
            case State.Dying:
                //Debug.Log("SpinGolem: Dying");
                anim.SetTrigger("Hurt");
                break;
            case State.Dead:
                //Debug.Log("SpinGolem: Dead");

                rb.isKinematic = true;

                foreach(Collider col in colliders)
                {
                    col.enabled = false;
                }

                break;
        }
    }

    public enum State
    {
        Idle,
        Move,
        Spin,
        Attack,
        Vulnerable,
        Readjusting,
        Dying,
        Dead
    }
}
