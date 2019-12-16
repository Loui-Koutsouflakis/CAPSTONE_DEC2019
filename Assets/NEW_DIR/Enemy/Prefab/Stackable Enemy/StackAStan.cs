//Stackable Enemy
//written by Michael Elkin 12-09-2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackAStan : MonoBehaviour, IKillable
{
    public enum StackableEnemyStatus { Idle, Move, Chase, CoolDown, Passenger, Stacking, Dead }
    [SerializeField]
    StackableEnemyStatus status;

    #region Cached References    

    [SerializeField]
    BoxCollider boxCollider;
    [SerializeField]
    BoxCollider boxTriggerCollider;
    [SerializeField]
    NavMeshAgent navAgent;
    [SerializeField]
    Animator anim;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Transform playerRef;
    [SerializeField]
    Transform backPack;//Drag and Drop emptyobject centered on Object.
    [SerializeField]
    StackAStan parent;

    #endregion

    #region Course Plotting
    [SerializeField]
    Vector3 homePosition;
    [SerializeField]
    Vector3 movePosition;

    Vector3 rdmDirection;

    float rdmDistance = 0.0f;

    #endregion

    #region Stacking
    // Stacking Sort & Position
    [SerializeField, Header("Stack Rank")]
    int stackingRank = 0;
    int passengerCount = 0;
    [SerializeField]
    int passengerNum = 0;

    // Stacking Movement
    [SerializeField]
    int lerpNumer = 0;
    [SerializeField]
    int LerpDenom = 1;
    [SerializeField, Range(0.0f, 5.0f)]
    float offset;

    //Stacking bools
    [SerializeField]
    bool hasPassengers;
    [SerializeField]
    bool isAPassenger;
    [SerializeField]
    bool animTrigg;

    // Stack Destination
    //[SerializeField]
    Vector3 dest;
    #endregion

    #region Anim Timers

    [SerializeField, Range(0, 5)]
    float idleAnimTime = 0.0f;
    [SerializeField, Range(0, 5)]
    float coolDownAnimTime = 0.0f;
    [SerializeField, Range(0, 5)]
    float StackAnimTime = 0.0f;
    [SerializeField, Range(0, 5)]
    float deathAnimTime = 0.0f;

    #endregion

    #region Movement Settings
        
    [SerializeField, Range(0, 10)]
    float chaseRange = 5.0f;
    [SerializeField, Range(0, 5)]
    float minMoveDist = 5.0f;
    [SerializeField, Range(5, 10)]
    float maxMoveDist = 5.0f;
    [SerializeField, Range(0, 5)]
    float maxSpeed = 5.0f;
    [SerializeField, Range(0, 5)]
    float accel = 5.0f;

    #endregion

    #region Status Bools

    [SerializeField]
    bool needIdleMovePos;
    [SerializeField]
    bool notMoving;
    [SerializeField]
    bool isChasing;
    [SerializeField]
    bool coolingDown;
    [SerializeField]
    bool dying;

    #endregion

    private void Awake()
    {
        homePosition = transform.position;
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = maxSpeed;
        navAgent.acceleration = accel;
        BoxCollider[] temp = GetComponents<BoxCollider>();
        foreach (BoxCollider box in temp)
        {
            if (box.isTrigger)
            {
                boxTriggerCollider = box;                
            }
            else
            {
                boxCollider = box;
            }
        }
        GenStackRank();
        rb = GetComponent<Rigidbody>();
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        needIdleMovePos = true;
        status = StackableEnemyStatus.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case StackableEnemyStatus.Idle:
                if (!PlayerInRange())
                {
                    if (needIdleMovePos)
                    {
                        if (!hasPassengers)
                        {
                            anim.SetBool("IsMoving", false);
                        }
                        else
                        {
                            anim.SetBool("IsMovingStacked", false);
                        }
                        needIdleMovePos = false;
                        StartCoroutine(IdleAnimation(idleAnimTime));// Change idleAnimTime to animation length
                    }
                }
                else
                {
                    
                    SetEnemyStatus(StackableEnemyStatus.Chase);
                }
                break;
            case StackableEnemyStatus.Move:
                if (!PlayerInRange())
                {
                    if (notMoving)
                    {
                        notMoving = false;
                        SetDestination(GetIdleMoveDest());
                        navAgent.isStopped = false;
                        // Trigger move animation start
                        if (!hasPassengers)
                        {
                            anim.SetBool("IsMoving", true);
                        }
                        else
                        {
                            anim.SetBool("IsMovingStacked", true);
                        }
                    }
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        needIdleMovePos = true;
                        SetEnemyStatus(StackableEnemyStatus.Idle);
                    }
                }
                break;
            case StackableEnemyStatus.Chase:
                if (PlayerInRange())
                {
                    if (playerRef.position != navAgent.destination)
                    {
                        anim.SetBool("IsMoving", true);
                        SetDestination(playerRef.position);
                    }
                }
                else
                {
                    SetEnemyStatus(StackableEnemyStatus.Idle);
                }
                break;
            case StackableEnemyStatus.CoolDown:
                if (!coolingDown)
                {                    
                    coolingDown = true;
                    StartCoroutine(CooldownAnimation(coolDownAnimTime));// Change coolDownAnimTime to animation length
                }
                break;
            case StackableEnemyStatus.Passenger:                
                if(parent.status == StackableEnemyStatus.Move)
                {
                    anim.SetBool("PassMove", true);
                    anim.speed = 0.7f;
                }
                else
                {
                    anim.SetBool("PassMove", false);
                    anim.speed = 1.0f;
                }
                if (Vector3.Angle(transform.forward, transform.parent.forward) > 0)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, 2.5f);
                }
                break;
            case StackableEnemyStatus.Stacking:
                if(hasPassengers)
                {
                    if (animTrigg)
                    {
                        animTrigg = false;
                        StartCoroutine(StackingAnimationWait(StackAnimTime));
                    }
                }
                else
                {
                    if(passengerNum == parent.GetBackPack().childCount)
                    {
                        if(animTrigg)
                        {
                            animTrigg = false;
                            lerpNumer = 0;
                            SetLerpDenom(StackAnimTime);
                            StartCoroutine(StackingAnimationJump(StackAnimTime));
                        }
                        else
                        {
                            transform.localPosition = Vector3.Lerp(transform.localPosition, GetJumpDestinantion(), (float)lerpNumer / (float)LerpDenom);
                            if (lerpNumer < LerpDenom)
                            {
                                lerpNumer++;
                            }
                        }
                    }
                    else
                    {
                        if(animTrigg)
                        {
                            animTrigg = false;
                            lerpNumer = 0;
                            SetLerpDenom(StackAnimTime);
                            StartCoroutine(StackingAnimationWait(StackAnimTime));
                        }
                    }
                }
                break;
            case StackableEnemyStatus.Dead:
                if (!dying)
                {
                    dying = true;
                    StartCoroutine(DeathAnimation(deathAnimTime));// Change deathAnimTime to animation length
                }
                break;            
        }
    }

    private void OnTriggerEnter(Collider o)
    {
        if (status != StackableEnemyStatus.Stacking)
        {

            if (o.gameObject.GetComponent<StackAStan>() && o.GetComponent<StackAStan>().GetBackPack().childCount == 0)
            {
                if (!hasPassengers)
                {
                    if (o.transform.GetComponent<StackAStan>().GetStackingRank() < stackingRank)
                    {
                        o.GetComponent<StackAStan>().GetAnimator().SetBool("IsMoving", false);
                        anim.SetBool("IsMoving", false);
                        navAgent.isStopped = true;
                        SetEnemyStatus(StackableEnemyStatus.Stacking);
                        animTrigg = true;
                        hasPassengers = true;
                        o.GetComponent<StackAStan>().SetStackingStatus();
                        o.GetComponent<StackAStan>().SetNewParent(backPack);
                        o.GetComponent<StackAStan>().SetPassengerNum(backPack.childCount);
                        passengerCount = backPack.childCount;

                    }
                }
                else
                {
                    if (backPack.childCount < 2)
                    {
                        o.GetComponent<StackAStan>().GetAnimator().SetBool("IsMoving", false);
                        anim.SetBool("IsMovingStacked", false);
                        navAgent.isStopped = true;
                        o.GetComponent<StackAStan>().SetStackingStatus();
                        o.GetComponent<StackAStan>().SetNewParent(backPack);
                        o.GetComponent<StackAStan>().SetPassengerNum(backPack.childCount);
                        passengerCount = backPack.childCount;
                        backPack.GetChild(this.backPack.childCount - this.backPack.childCount).GetComponent<StackAStan>().SetEnemyStatus(StackableEnemyStatus.Stacking);
                        backPack.GetChild(this.backPack.childCount - this.backPack.childCount).GetComponent<StackAStan>().animTrigg = true;
                        SetEnemyStatus(StackableEnemyStatus.Stacking);
                        animTrigg = true;
                    }
                }
            }
        }
    }

    #region Internal Functions

    bool PlayerInRange()
    {
        if (Vector3.Distance(playerRef.position, transform.position) <= chaseRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetEnemyStatus(StackableEnemyStatus newstatus)
    {
        status = newstatus;
    }

    void SetDestination(Vector3 target)
    {
        navAgent.SetDestination(target);
    }

    void SetLerpDenom(float num2Convert)
    {
        LerpDenom = (int)(num2Convert * 60.0f);
    }

    void GenStackRank()
    {
        stackingRank = Random.Range(1, 100);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    Vector3 GetIdleMoveDest()
    {
        rdmDirection = new Vector3(Random.Range(-180, 180), transform.position.y, Random.Range(-180, 180)).normalized;
        rdmDistance = Random.Range(minMoveDist, maxMoveDist);
        movePosition = homePosition + (rdmDirection * rdmDistance);

        return movePosition;

    }

    Vector3 GetJumpDestinantion()
    {
        dest = (Vector3.up * (offset * parent.GetBackPack().childCount));

        //switch (parent.GetBackPack().childCount)
        //{
        //    case 1:
        //        dest = parent.transform.position;
        //        break;
        //    case 2:
        //        dest = parent.transform.position + (Vector3.up * offset);
        //        break;
        //}
        //Vector3 dest = parent.transform.position + Vector3.up * (offset * parent.GetBackPack().childCount);
        return dest;
    }

    #endregion

    #region Animation Coroutines

    IEnumerator IdleAnimation(float t)
    {
        // Insert Idle Stacking Trigger here


        yield return new WaitForSeconds(t);
        //Debug.Log("Idle wait finished");
        notMoving = true;
        SetEnemyStatus(StackableEnemyStatus.Move);



    }

    IEnumerator CooldownAnimation(float t)
    {
        // Insert CoolDown Animation trigger here
        anim.SetTrigger("PlayerHit");
        yield return new WaitForSeconds(t);
        needIdleMovePos = true;
        SetEnemyStatus(StackableEnemyStatus.Idle);

    }

    IEnumerator DeathAnimation(float t)
    {
        // Insert Death Animation trigger here
        anim.SetBool("IsDead", true);
        boxTriggerCollider.enabled = false;
        boxCollider.enabled = false;
        Destroy(navAgent);
        rb.isKinematic = false;
        rb.useGravity = true;
        //navAgent.enabled = false;
        yield return new WaitForSeconds(t);
        //transform.gameObject.SetActive(false);

    }

    IEnumerator StackingAnimationJump(float t)
    {
        //Insert Stacking Jump Animation Trigger Here
        anim.SetTrigger("StackingJump");
        yield return new WaitForSeconds(t);
        lerpNumer = 0;
        
        SetEnemyStatus(StackableEnemyStatus.Passenger);

    }

    IEnumerator StackingAnimationWait(float t)
    {
        //Insert Stacking Wait Animation Trigger Here
        anim.SetBool("IsMoving", false);
        yield return new WaitForSeconds(t);
        needIdleMovePos = true;
        notMoving = true;
        //SetEnemyStatus(SnowManStatus.Idle);
        if (hasPassengers)
        {
            SetEnemyStatus(StackableEnemyStatus.Idle);
            anim.SetBool("IsMoving", false);
        }
        else
        {
            SetEnemyStatus(StackableEnemyStatus.Passenger);
        }


    }
    
    #endregion

    #region Public Functions
    //Set Parent Functions
    public void SetNewParent(Transform bp)
    {
        transform.parent = bp;
        parent = bp.parent.GetComponent<StackAStan>();
    }
    public void SetParentNull()
    {
        transform.parent = null;
    }
    //Sorting Functions
    public Transform GetBackPack()
    {
        return backPack;
    }
    public float GetStackingRank()
    {
        return stackingRank;
    }
    public void SetPassengerNum(int num)
    {
        passengerNum = num;
    }
    public void SetStackingStatus()
    {
        StopAllCoroutines();
        //navAgent.isStopped = true;
        navAgent.enabled = false;
        boxTriggerCollider.enabled = false;
        //isAPassenger = true;
        animTrigg = true;
        SetEnemyStatus(StackableEnemyStatus.Stacking);

    }
    // Player Collision functions
    public void StartCoolDown()
    {
        coolingDown = false;
        SetEnemyStatus(StackableEnemyStatus.CoolDown);

    }
    public void SetDead()
    {
        dying = false;
        SetEnemyStatus(StackableEnemyStatus.Dead);
    }
    public Animator GetAnimator()
    {
        return anim;
    }
    #endregion

    public IEnumerator CheckHit(bool x)
    {
        yield return 0f; //new WaitForSeconds(1f);
        if (!x)
        {

            SetDead();
        }
        else
        {

            for (int i = 0; i < parent.GetBackPack().childCount; i++)
            {
                parent.GetBackPack().GetChild(i).GetComponent<StackAStan>().SetDead();
            }
            parent.SetDead();

        }
    }

    public IEnumerator TakeDamage()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Die()
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        //if (catHead == null)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(transform.position + (Vector3.up * (transform.lossyScale.y * 0.5f)), transform.lossyScale.y * 0.5f);
        //}

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(homePosition, maxMoveDist);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(movePosition, movePosition + (Vector3.up * 2));
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(homePosition, homePosition + (Vector3.up * 2));
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(homePosition, maxMoveDist);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(movePosition, movePosition + (Vector3.up * 2));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(homePosition, homePosition + (Vector3.up * 2));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

    }

}
