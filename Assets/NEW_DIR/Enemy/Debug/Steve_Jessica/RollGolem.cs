using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollGolem : MonoBehaviour, IKillable
{
    public Rigidbody rb;
    public Animator anim;
    public Transform playerTf;
    public MeshCollider wheelCol;
    public MeshCollider spinCollider;
    public SphereCollider headColVulnerable;
    public SphereCollider headColSpin;
    public BoxCollider[] handColliders;
    public State state;
    public Transform child;
    public ParticleSystem[] groundScrapePs;
    public ParticleSystem[] groundSpillPs;

    public float playerHeightOffset = 0.85f;

    int health = 2;
    readonly int raycastLayerMask = 0;
    bool seesPlayer;
    bool canBounceOff;
    bool canReverse;
    float viewRadius = 22f;
    readonly float rollSpeed = 10f;
    readonly float groundCheckDistance = 0.2f;
    readonly float wallCheckDistance = 3f;
    Vector3 direction;
    Vector3 localBurrowSpot = new Vector3(0f, -5f, 0f);
    Vector3 downward;
    Vector3 forwardOffset;
    Vector3 groundOffset;
    RaycastHit[] wallHit;
    RaycastHit[] groundHit;
    Coroutine bounceOffRoutine;
    Coroutine digRoutine;

    const string rollUpAnimTrig = "RollUp";
    const string startDigAnimTrig = "StartDig";
    const string takeDamageAnimTrig = "TakeDamage";
    const string crashAnimTrig = "Crash";
    const string readjustAnimTrig = "Readjust";
    const string enemyWeakSpotTag = "EnemyWeakSpot";
    const string enemyTag = "Enemy";

    [Header("RENDER DEBUG")]
    public SkinnedMeshRenderer[] rends;
    float dissolveStrength = 0f;
    readonly float dissolveLerp = 1.2f;

    private void Start()
    {
        canReverse = true;

        wallHit = new RaycastHit[9];
        groundHit = new RaycastHit[9];

        foreach(Collider col in handColliders)
        {
            col.enabled = false;
        }
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.T))
    //    {
    //        if (state == State.Vulnerable)
    //        {
    //            anim.SetTrigger(takeDamageAnimTrig);
    //            StopAllCoroutines();
    //            StartCoroutine(CheckHit());
    //        }
    //        else if(state == State.SurfacingSpin)
    //        {
    //            StopAllCoroutines();
    //            StartCoroutine(CheckHit());
    //        }
    //    }
    //}

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                if (Vector3.Distance(transform.position, playerTf.position) < viewRadius)
                {
                    StartCoroutine(RollUpSequence());
                }

                //CheckGround();

                break;
            case State.Balling:
                direction = (playerTf.position - transform.position).normalized;
                direction.y = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 12f * Time.fixedDeltaTime);
                
                //CheckGround();

                break;
            case State.Rolling:

                transform.position += transform.forward * rollSpeed * Time.fixedDeltaTime;

                for (int i = 0; i < wallHit.Length; i++)
                {
                    switch(i)
                    {
                        case 1:
                            forwardOffset.x = 0.3f;
                            groundOffset.x = 1f;
                            break;
                        case 2:
                            forwardOffset.x = -0.3f;
                            groundOffset.x = -1f;
                            break;
                        case 3:
                            forwardOffset.y = 1f;
                            groundOffset.z = 1f;
                            break;
                        case 4:
                            //forwardOffset.y = -0.1f;
                            groundOffset.z = -1f;
                            break;
                        case 5:
                            forwardOffset.x = 0.3f;
                            forwardOffset.y = 1f;
                            groundOffset.x = 1f;
                            groundOffset.z = 1f;
                            break;
                        case 6:
                            forwardOffset.x = -0.3f;
                            forwardOffset.y = 1f;
                            groundOffset.x = -1f;
                            groundOffset.z = 1f;
                            break;
                        case 7:
                            forwardOffset.x = 0.3f;
                            //forwardOffset.y = -0.1f;
                            groundOffset.x = 1f;
                            groundOffset.z = -1f;
                            break;
                        case 8:
                            forwardOffset.x = -0.3f;
                            //forwardOffset.y = -0.1f;
                            groundOffset.x = -1f;
                            groundOffset.z = -1f;
                            break;
                    }
                    
                    Physics.Raycast(transform.position, transform.forward + forwardOffset, out wallHit[i], wallCheckDistance);

                    if (wallHit[i].collider != null && (wallHit[i].collider.gameObject.layer == 14 || wallHit[i].collider.gameObject.layer == 15))
                    {
                        Debug.Log("RollGolem Raycast hit #: " + i + " against " + wallHit[i].collider.gameObject.name);

                        bounceOffRoutine = StartCoroutine(BounceOffSequence(true));
                    }

                    
                }

                CheckGround();

                break;
            case State.Burrowing:
                child.localPosition = Vector3.Lerp(child.localPosition, localBurrowSpot, 2.8f * Time.fixedDeltaTime);
                break;
            case State.Underground:
                if(playerTf.position.y < transform.position.y + playerHeightOffset)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(playerTf.position.x, playerTf.position.y - playerHeightOffset, playerTf.position.z), 3.6f * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(playerTf.position.x, transform.position.y, playerTf.position.z), 3.6f * Time.deltaTime);
                }

                break;
            case State.SurfacingSpin:
                child.localPosition = Vector3.Lerp(child.localPosition, Vector3.zero, 2f * Time.fixedDeltaTime);
                //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, playerTf.position.y - playerHeightOffset, transform.position.z), 3.6f * Time.deltaTime);
                break;
            case State.Dying:
                dissolveStrength = Mathf.Lerp(dissolveStrength, 1, dissolveLerp * Time.fixedDeltaTime);
                foreach (SkinnedMeshRenderer smr in rends)
                {
                    smr.material.SetFloat("_Strength", dissolveStrength);
                }
                break;
            case State.ReadjustingToRoll:
                direction = (playerTf.position - transform.position).normalized;
                direction.y = 0f;
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 12f * Time.fixedDeltaTime);

                //CheckGround();

                break;

            case State.Vulnerable:
                //CheckGround();
                break;
        }
    }

    public void CheckGround()
    {
        for (int i = 0; i < wallHit.Length; i++)
        {
            Physics.Raycast(transform.position + (Vector3.down * 0.3f), Vector3.down, out groundHit[i], groundCheckDistance);

            if (groundHit[i].collider == null /*&& canReverse*/)
            {
                if (downward.y == 0f)
                {
                    foreach (ParticleSystem ps in groundScrapePs)
                    {
                        ps.Stop();
                    }
                }

                downward += (Physics.gravity / 9f) * Time.fixedDeltaTime;
                transform.position += (downward / 9f) * Time.fixedDeltaTime;

            }
            else if (downward.y != 0f)
            {
                downward.y = 0f;

                foreach (ParticleSystem ps in groundScrapePs)
                {
                    ps.Play();
                }
            }
        }
    }

    public void SwitchState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.Idle:
                rb.isKinematic = true;
                break;
            case State.Balling:

                //anim.SetTrigger("RollUp");
                
                break;
            case State.Rolling:
                canBounceOff = true;
                headColVulnerable.enabled = false;
                wheelCol.enabled = true;
                break;
            case State.BounceOff:
                canBounceOff = false;
                anim.SetTrigger(crashAnimTrig);
                break;
            case State.Vulnerable:
                headColVulnerable.enabled = true;
                break;
            case State.TakingDamage:
                break;
            case State.Burrowing:

                anim.SetTrigger(startDigAnimTrig);
                rb.isKinematic = true;
                rb.useGravity = false;

                foreach(ParticleSystem ps in groundSpillPs)
                {
                    ps.Play();
                }

                break;
            //case State.Underground:
                //break;
            //case State.SurfacingSpin:
                //break;
            case State.Dying:

                foreach(SkinnedMeshRenderer smr in rends)
                {
                    smr.receiveShadows = false;
                    smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                foreach (ParticleSystem ps in groundSpillPs)
                {
                    ps.Stop();
                }

                //foreach(BoxCollider col in handColliders)
                //{
                //    col.enabled = false;
                //}

                wheelCol.enabled = false;
                headColVulnerable.enabled = false;

                anim.enabled = false;
                break;

            case State.ReadjustingToRoll:
                anim.SetTrigger(readjustAnimTrig);
                break;
            case State.Dead:
                foreach(SkinnedMeshRenderer smr in rends)
                {
                    smr.enabled = false;
                }
                break;
        }
    }

    public IEnumerator RollUpSequence()
    {
        wheelCol.enabled = true;
        anim.SetTrigger(rollUpAnimTrig);
        SwitchState(State.Balling);
        yield return new WaitForSeconds(2.2f);
        
        SwitchState(State.Rolling);
    }

    public IEnumerator ReadjustSequence()
    {
        foreach (BoxCollider col in handColliders)
        {
            col.enabled = false;
        }

        wheelCol.enabled = true;

        SwitchState(State.ReadjustingToRoll);
        yield return new WaitForSeconds(0.75f);
        SwitchState(State.Balling);
        yield return new WaitForSeconds(1f);
        SwitchState(State.Rolling);
        canBounceOff = true;
    }

    public IEnumerator DigSequence()
    {
        headColVulnerable.enabled = false;
        spinCollider.enabled = true;

        SwitchState(State.Burrowing);
        yield return new WaitForSeconds(2f);
        SwitchState(State.Underground);
        yield return new WaitForSeconds(3f);
        SwitchState(State.SurfacingSpin);
        headColSpin.enabled = true;
        yield return new WaitForSeconds(5f);
        headColSpin.enabled = false;
        StartCoroutine(DigSequence());
    }

    public IEnumerator BounceOffSequence(bool isWallBounce)
    {
        canBounceOff = false;
        SwitchState(State.BounceOff);

        foreach (BoxCollider col in handColliders)
        {
            col.enabled = true;
        }

        yield return new WaitForSeconds(1.2f);
        wheelCol.enabled = false;
        headColVulnerable.enabled = true;
        SwitchState(State.Vulnerable);
        yield return new WaitForSeconds(4f);
        StartCoroutine(ReadjustSequence());
    }

    public IEnumerator CheckHit(bool isGroundPound)
    {
        Debug.Log("CheckHit() called for Steve");

        yield return 0f;

        if(state == State.Vulnerable)
        {
            StopCoroutine("BounceOffSequence");
            StopCoroutine(bounceOffRoutine);
            StartCoroutine(TakeDamage());
        }
        else if(state == State.Burrowing || state == State.SurfacingSpin)
        {
            StopCoroutine("DigSequence");
            StopCoroutine(digRoutine);
            StartCoroutine(Die());
        }
    }

    public IEnumerator TakeDamage()
    {
        Debug.Log("TakeDamage() called for Steve");

        foreach (BoxCollider col in handColliders)
        {
            col.enabled = false;
        }

        spinCollider.enabled = true;

        SwitchState(State.TakingDamage);

        yield return new WaitForSeconds(2f);

        digRoutine = StartCoroutine(DigSequence());
    }

    public IEnumerator Die()
    {

        Debug.Log("Die() called for Steve");

        spinCollider.enabled = false;
        headColSpin.enabled = false;

        SwitchState(State.Dying);
        yield return new WaitForSeconds(2f);
        SwitchState(State.Dead);
    }

    public IEnumerator ReverseBuffer()
    {
        canReverse = false;
        yield return new WaitForSeconds(2f);
        canReverse = true;
    }

    public enum State
    {
        Idle,
        Balling,
        Rolling,
        BounceOff,
        Vulnerable,
        TakingDamage,
        ReadjustingToRoll,
        Burrowing,
        Underground,
        SurfacingSpin,
        ReadjustingToSpin,
        Dying,
        Dead
    }
}
