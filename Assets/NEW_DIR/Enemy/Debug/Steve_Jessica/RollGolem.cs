using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollGolem : MonoBehaviour, IKillable
{
    public Rigidbody rb;
    public Animator anim;
    public Transform playerTf;
    public MeshCollider wheelCol;
    public BoxCollider standCol;
    public BoxCollider[] handColliders;
    public State state;
    public Transform child;
    public ParticleSystem[] groundScrapePs;
    public ParticleSystem[] groundSpillPs;
    
    int health = 2;
    readonly int raycastLayerMask = 0;
    bool seesPlayer;
    bool canBounceOff;
    bool canReverse;
    float viewRadius = 32f;
    readonly float rollSpeed = 23f;
    //readonly float forwardForce = 12.6f;
    readonly float groundCheckDistance = 1.6f;
    readonly float wallCheckDistance = 4f;
    Vector3 direction;
    Vector3 localBurrowSpot = new Vector3(0f, -5f, 0f);
    Vector3 downward;
    //readonly Vector3 angularForward = new Vector3(16f, 0f, 0f);
    //readonly Vector3 angularRight = new Vector3(0f, 0f, -16f);
    RaycastHit wallHit;
    RaycastHit groundHit;

    const string rollUpAnimTrig = "RollUp";
    const string startDigAnimTrig = "StartDig";

    [Header("RENDER DEBUG")]
    public SkinnedMeshRenderer[] rends;
    float dissolveStrength = 0f;
    readonly float dissolveLerp = 1.2f;

    private void Start()
    {
        canReverse = true;
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.T))
        //{
        //    if (state == State.Vulnerable)
        //    {
        //        anim.SetTrigger("TakeDamage");
        //        StopAllCoroutines();
        //        StartCoroutine(CheckHit());
        //    }
        //    else if(state == State.SurfacingSpin)
        //    {
        //        StopAllCoroutines();
        //        StartCoroutine(CheckHit());
        //    }
        //}
    }

    void FixedUpdate()
    {

        switch (state)
        {
            case State.Idle:
                if (Vector3.Distance(transform.position, playerTf.position) < viewRadius)
                {
                    StartCoroutine(RollUpSequence());
                }
                break;
            case State.Balling:
                direction = (playerTf.position - transform.position).normalized;
                direction.y = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 12f * Time.fixedDeltaTime);
                break;
            case State.Rolling:

                transform.position += transform.forward * rollSpeed * Time.fixedDeltaTime;

                //rb.AddForce(direction * forwardForce);
                //rb.angularVelocity += direction.z * Vector3.right * 16f * Time.fixedDeltaTime;

                Physics.Raycast(transform.position + (Vector3.down * 0.3f), Vector3.down, out groundHit, groundCheckDistance);
                Physics.Raycast(transform.position, transform.forward, out wallHit, wallCheckDistance);
                
                //if (groundHit.collider == null && canBounceOff)
                //{
                //    Debug.Log("<color=red>POOP</color>");
                //    StartCoroutine(BounceOffSequence(false));
                //}
                //else 
                if (wallHit.collider != null && wallHit.collider.tag != "Enemy")
                {
                    StartCoroutine(BounceOffSequence(true));
                    //rb.velocity = Vector3.zero;
                    //rb.angularVelocity = Vector3.zero;
                }

                if(groundHit.collider == null /*&& canReverse*/)
                {
                    if(downward.y == 0f)
                    {
                        foreach (ParticleSystem ps in groundScrapePs)
                        {
                            ps.Stop();
                        }
                    }

                    downward += Physics.gravity * Time.fixedDeltaTime;
                    transform.position += downward * Time.fixedDeltaTime;

                    //Debug.Log("<color=red>POOP</color>");
                    //transform.Rotate(0f, 180f, 0f);
                    //StartCoroutine(ReverseBuffer());
                }
                else if(downward.y != 0f)
                {
                    downward.y = 0f;

                    foreach(ParticleSystem ps in groundScrapePs)
                    {
                        ps.Play();
                    }
                }

                break;
            case State.BounceOff:
                break;
            case State.Vulnerable:
                break;
            case State.TakingDamage:
                break;
            case State.Burrowing:
                child.localPosition = Vector3.Lerp(child.localPosition, localBurrowSpot, 2.8f * Time.fixedDeltaTime);
                break;
            case State.Underground:
                transform.position = Vector3.Lerp(transform.position, new Vector3(playerTf.position.x, transform.position.y, playerTf.position.z), 3.6f * Time.deltaTime);
                break;
            case State.SurfacingSpin:
                child.localPosition = Vector3.Lerp(child.localPosition, Vector3.zero, 2f * Time.fixedDeltaTime);
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
                break;
        }
    }

    public void SwitchState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.Idle:
                //Debug.Log("RollGolem: Idle");
                rb.isKinematic = true;
                break;
            case State.Balling:
                //Debug.Log("RollGolem: Balling");
                //anim.SetTrigger("RollUp");
                
                break;
            case State.Rolling:
                //Debug.Log("RollGolem: Rolling");
                canBounceOff = true;
                //rb.isKinematic = false;
                standCol.enabled = false;
                wheelCol.enabled = true;
                break;
            case State.BounceOff:
                //Debug.Log("RollGolem: BounceOff");
                //rb.isKinematic = false;
                //rb.useGravity = true;
                //rb.velocity = Vector3.zero;
                //rb.angularVelocity = Vector3.zero;
                //rb.AddForce((-direction + Vector3.up) * 10f, ForceMode.Impulse);
                canBounceOff = false;
                anim.SetTrigger("Crash");
                break;
            case State.Vulnerable:
                //Debug.Log("RollGolem: Vulnerable");
                //foreach (SkinnedMeshRenderer mr in rends)
                //{
                //    mr.material = vulnerableMat;
                //}
                standCol.enabled = true;
                break;
            case State.TakingDamage:
                //Debug.Log("RollGolem: TakingDamage");
                //foreach (SkinnedMeshRenderer mr in rends)
                //{
                //    mr.material = hurtMat;
                    
                //}
                break;
            case State.Burrowing:
                //Debug.Log("RollGolem: Burrowing");
                anim.SetTrigger("StartDig");
                rb.isKinematic = true;
                rb.useGravity = false;

                foreach(ParticleSystem ps in groundSpillPs)
                {
                    ps.Play();
                }

                //foreach (SkinnedMeshRenderer mr in rends)
                //{
                //    mr.material = burrowingMat;
                //}
                break;
            case State.Underground:
                //Debug.Log("RollGolem: Underground");
                break;
            case State.SurfacingSpin:
                //Debug.Log("RollGolem: SurfacingSpin");
                break;
            case State.Dying:
                //Debug.Log("RollGolem: Dying");
                foreach(SkinnedMeshRenderer smr in rends)
                {
                    smr.receiveShadows = false;
                    smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                foreach (ParticleSystem ps in groundSpillPs)
                {
                    ps.Stop();
                }

                foreach(BoxCollider col in handColliders)
                {
                    col.enabled = false;
                }

                wheelCol.enabled = false;
                standCol.enabled = false;

                anim.enabled = false;
                break;

            case State.ReadjustingToRoll:
                //Debug.Log("ReadjustingToRoll");
                //foreach (SkinnedMeshRenderer mr in rends)
                //{
                //    mr.material = readjustMat;
                //}
                anim.SetTrigger("Readjust");
                break;
            case State.ReadjustingToSpin:
                //foreach (SkinnedMeshRenderer mr in rends)
                //{
                //    mr.material = readjustMat;
                //}
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
        anim.SetTrigger("RollUp");
        SwitchState(State.Balling);
        yield return new WaitForSeconds(2.2f);
        SwitchState(State.Rolling);
    }

    public IEnumerator ReadjustSequence()
    {
        SwitchState(State.ReadjustingToRoll);
        yield return new WaitForSeconds(0.75f);
        SwitchState(State.Balling);
        yield return new WaitForSeconds(1f);
        SwitchState(State.Rolling);
        canBounceOff = true;
    }

    public IEnumerator DigSequence()
    {
        SwitchState(State.Burrowing);
        yield return new WaitForSeconds(2f);
        SwitchState(State.Underground);
        yield return new WaitForSeconds(3f);
        SwitchState(State.SurfacingSpin);
        yield return new WaitForSeconds(5f);
        StartCoroutine(DigSequence());
    }

    public IEnumerator BounceOffSequence(bool isWallBounce)
    {
        //Debug.Log("BounceOffSequence");

        //if(isWallBounce)
        //{
            canBounceOff = false;
            SwitchState(State.BounceOff);
            yield return new WaitForSeconds(1.2f);
            SwitchState(State.Vulnerable);
            yield return new WaitForSeconds(4f);
            StartCoroutine(ReadjustSequence());
            

        //}
        //else
        //{
        //    canBounceOff = false;
        //    yield return new WaitForSeconds(1.5f);
        //    canBounceOff = true;
        //}
    }

    public IEnumerator CheckHit(bool x)
    {
        yield return new WaitForSeconds(0.01f);

        if(state == State.Vulnerable)
        {
            StartCoroutine(TakeDamage());
        }
        else if(state == State.Burrowing || state == State.SurfacingSpin)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator TakeDamage()
    {
        SwitchState(State.TakingDamage);

        //foreach (SkinnedMeshRenderer mr in rends)
        //{
        //    mr.material = hurtMat;
        //}

        yield return new WaitForSeconds(2f);

        StartCoroutine(DigSequence());
    }

    public IEnumerator Die()
    {
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
