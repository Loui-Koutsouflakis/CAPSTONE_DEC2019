//Stackable Enemy
//written by Michael Elkin 11-26-2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackableEnemy : MonoBehaviour, IKillable
{
    public enum StackableEnemyStatus { Idle, Move, Chase, CoolDown, Passenger, Stacking, Dead }
    [SerializeField]
    StackableEnemyStatus status;

    public enum EnemyType { Rock, Snow}
    [SerializeField]
    EnemyType type;

    

    // Body Parts
    [SerializeField]
    Stack catHead;
    [SerializeField]
    Stack catMid;
    [SerializeField]
    Stack catBase;
    [SerializeField]
    Stack rockMonst;
    [SerializeField]
    Stack activeStack;

    // Cached for resizing
    [SerializeField]
    BoxCollider bCollider;
    [SerializeField]
    CapsuleCollider cCollider;
    [SerializeField]
    Transform playerRef;
    [SerializeField]
    Transform backPack;//Drag and Drop emptyobject centered on Object.

    [SerializeField]
    Rigidbody rb;

    [SerializeField, Range(0, 5)]
    float idleAnimTime = 0.0f;
    [SerializeField, Range(0, 5)]
    float coolDownAnimTime = 0.0f;
    [SerializeField, Range(0, 5)]
    float StackAnimTime = 0.0f;
    [SerializeField, Range(0, 5)]
    float deathAnimTime = 0.0f;
    [SerializeField, Range(0, 10)]
    float chaseRange = 5.0f;
    


    [SerializeField, Range(0, 5)]
    float minMoveDist = 0.0f;
    [SerializeField, Range(5, 10)]
    float maxMoveDist = 0.0f;

    NavMeshAgent navAgent;

    #region Course Plotting
    [SerializeField]
    Vector3 homePosition;
    [SerializeField]
    Vector3 movePosition;

    Vector3 rdmDirection;
    float rdmDistance = 0.0f;

    

    #endregion

    #region Stacking
    [SerializeField, Header("Stack Rank")]
    int stackingRank = 0;
    int passengerCount = 0;
    [SerializeField]
    int passengerNum = 0;

    [SerializeField]
    int lerpNumer = 0;
    [SerializeField]
    int LerpDenom = 1;
    Vector3 offset;

    //Stacking bools
    [SerializeField]
    bool hasPassengers;
    [SerializeField]
    bool isAPassenger;
    [SerializeField]
    bool animTrigg;

    #endregion

    #region Bools
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
        navAgent = GetComponent<NavMeshAgent>();
        bCollider = GetComponent<BoxCollider>();
        cCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        GenStackRank();
        homePosition = transform.position;
        SetActiveStack();
        needIdleMovePos = true;
        status = StackableEnemyStatus.Idle;
    }


    // Start is called before the first frame update
    void Start()
    {
        
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
                        needIdleMovePos = false;
                        StartCoroutine(IdleAnimation(idleAnimTime));// Change idleAnimTime to animation length
                    }
                }
                else
                {
                    //isChasing = true;
                    SetEnemyStatus(StackableEnemyStatus.Chase);
                }
                break;
            case StackableEnemyStatus.Move:
                if (!PlayerInRange())
                {
                    if (notMoving)
                    {
                        // Trigger move animation start
                        notMoving = false;
                        SetDestination(GetIdleMoveDest());
                        navAgent.isStopped = false;
                    }
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        // Trigger move animation start
                        //notMoving = true;
                        needIdleMovePos = true;
                        SetEnemyStatus(StackableEnemyStatus.Idle);
                    }
                }
                else
                {
                    //isChasing = true;
                    needIdleMovePos = true;
                    SetEnemyStatus(StackableEnemyStatus.Chase);
                }
                break;
            case StackableEnemyStatus.Chase:
                if (PlayerInRange())
                {
                    if (playerRef.position != navAgent.destination)
                    {
                        SetDestination(playerRef.position);
                    }
                }
                else
                {
                    SetEnemyStatus(StackableEnemyStatus.Idle);
                }
                break;
            case StackableEnemyStatus.CoolDown:
                if(!coolingDown)
                {
                    coolingDown = true;
                    StartCoroutine(CooldownAnimation(coolDownAnimTime));// Change coolDownAnimTime to animation length
                }
                break;
            case StackableEnemyStatus.Passenger:
                if(Vector3.Angle(transform.forward, transform.parent.forward) > 0)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, 2.5f);
                }
                break;
            case StackableEnemyStatus.Stacking:
                if(hasPassengers)
                {
                    if(animTrigg)
                    {
                        animTrigg = false;
                        StartCoroutine(StackingAnimationWait(StackAnimTime));
                    }
                }
                else
                {
                    if (passengerNum == transform.parent.parent.GetComponent<StackableEnemy>().backPack.childCount)
                    {
                        if (animTrigg)
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
                        if (animTrigg)
                        {
                            animTrigg = false;
                            lerpNumer = 0;
                            SetLerpDenom(StackAnimTime);
                            StartCoroutine(StackingAnimationWait(StackAnimTime));
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
                }
                break;
            case StackableEnemyStatus.Dead:
                if(!dying)
                {
                    dying = true;
                    StartCoroutine(DeathAnimation(deathAnimTime));// Change deathAnimTime to animation length
                }
                break;           
        }
    }

 

    // Animation Coroutines
    IEnumerator IdleAnimation(float t)
    {
        // Insert Idle Stacking Trigger here


        yield return new WaitForSeconds(t);
        Debug.Log("Idle wait finished");
        notMoving = true;
        SetEnemyStatus(StackableEnemyStatus.Move);
       


    }

    IEnumerator CooldownAnimation(float t)
    {
        // Insert CoolDown Animation trigger here

        yield return new WaitForSeconds(t);
        needIdleMovePos = true;
        SetEnemyStatus(StackableEnemyStatus.Idle);

    }

    IEnumerator DeathAnimation(float t)
    {
        // Insert Death Animation trigger here

        yield return new WaitForSeconds(t);
        transform.gameObject.SetActive(false);
    }

    IEnumerator StackingAnimationJump(float t)
    {
        //Insert Stacking Jump Animation Trigger Here

        yield return new WaitForSeconds(t);
        lerpNumer = 0;
        SetActiveStack();
        SetEnemyStatus(StackableEnemyStatus.Passenger);

    }

    IEnumerator StackingAnimationWait(float t)
    {
        //Insert Stacking Wait Animation Trigger Here
        SetActiveStack();
        yield return new WaitForSeconds(t);
        needIdleMovePos = true;
        notMoving = true;
        //SetEnemyStatus(StackableEnemyStatus.Idle);
        if (hasPassengers)
        {
            SetEnemyStatus(StackableEnemyStatus.Idle);

        }
        else
        {
            SetEnemyStatus(StackableEnemyStatus.Passenger);
        }


    }


    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.GetComponent<StackableEnemy>() && o.GetComponent<StackableEnemy>().backPack.childCount == 0)
        {
            if (!hasPassengers)
            {
                if (o.transform.GetComponent<StackableEnemy>().GetStackingRank() < stackingRank)
                {
                    navAgent.isStopped = true;
                    SetEnemyStatus(StackableEnemyStatus.Stacking);
                    animTrigg = true;
                    hasPassengers = true;                                     
                    o.GetComponent<StackableEnemy>().SetPassangerStatus();
                    o.transform.parent = backPack;
                    o.GetComponent<StackableEnemy>().SetPassengerNum(backPack.childCount);
                    passengerCount = backPack.childCount;
                    //o.GetComponent<StackableEnemy>().SetEnemyStatus(StackableEnemyStatus.Stacking);
                    //o.GetComponent<StackableEnemy>().animTrigg = true;
                }
            }
            else
            {
                if (backPack.childCount < 2)
                {
                    navAgent.isStopped = true;
                    o.GetComponent<StackableEnemy>().SetPassangerStatus();
                    o.transform.parent = backPack;
                    o.GetComponent<StackableEnemy>().SetPassengerNum(backPack.childCount);
                    passengerCount = backPack.childCount;
                    //o.GetComponent<StackableEnemy>().SetEnemyStatus(StackableEnemyStatus.Stacking);
                    //o.GetComponent<StackableEnemy>().animTrigg = true;
                    backPack.GetChild(this.backPack.childCount - this.backPack.childCount).GetComponent<StackableEnemy>().SetEnemyStatus(StackableEnemyStatus.Stacking);
                    backPack.GetChild(this.backPack.childCount - this.backPack.childCount).GetComponent<StackableEnemy>().animTrigg = true;
                    SetEnemyStatus(StackableEnemyStatus.Stacking);
                    animTrigg = true;
                }
            }
        }
    }

    void GenStackRank()
    {
        stackingRank = Random.Range(1, 100);
    }

    Vector3 GetJumpDestinantion()
    {
        switch (type)
        {
            case EnemyType.Rock:
                switch (passengerNum)
                {
                    case 1:
                        offset = Vector3.up * rockMonst.GetModelInfo().offset;
                        break;
                    case 2:
                        offset = ((Vector3.up * rockMonst.GetModelInfo().offset) * passengerNum);
                        break;
                }                
                break;
            case EnemyType.Snow:
                switch (passengerNum)
                {
                    case 1:
                        if (transform.parent.parent.GetComponent<StackableEnemy>().backPack.childCount > 1)
                        {
                            offset = Vector3.up * catMid.GetModelInfo().offset;
                        }
                        else
                        {
                            offset = Vector3.up * catHead.GetModelInfo().offset;
                        }
                break;
                    case 2:
                        
                        offset = (Vector3.up * (catHead.GetModelInfo().offset)) + (Vector3.up 
                            * transform.parent.parent.GetComponent<StackableEnemy>().backPack.
                            GetChild( transform.parent.parent.GetComponent<StackableEnemy>().backPack.childCount 
                                - transform.parent.parent.GetComponent<StackableEnemy>().backPack.childCount).
                                GetComponent<StackableEnemy>().activeStack.GetModelInfo().offset);
                        break;
                }
                break;            
        }
        return offset;
    }

    void SetLerpDenom(float num2Convert)
    {
        LerpDenom = (int)(num2Convert * 60.0f);
    }

    #region Public Functions

    public float GetStackingRank()
    {
        return stackingRank;
    }
    public void SetPassengerNum(int num)
    {
        passengerNum = num;
    }
    public void SetPassangerStatus()
    {
        StopAllCoroutines();
        //navAgent.isStopped = true;
        navAgent.enabled = false;
        bCollider.enabled = false;
        isAPassenger = true;
        animTrigg = true;
        SetEnemyStatus(StackableEnemyStatus.Stacking);
        
    }
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
    public void ChangeType(EnemyType newType)
    {
        type = newType;
        SetActiveStack();
        if(backPack.childCount > 0)
        {
            foreach (var child in backPack)
            {
                GetComponent<StackableEnemy>().type = newType;
                GetComponent<StackableEnemy>().SetActiveStack();
            }
        }
    }


    #endregion

    bool PlayerInRange()
    {
        if(Vector3.Distance(playerRef.position, transform.position) <= chaseRange)
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

    Vector3 GetIdleMoveDest()
    {
        rdmDirection = new Vector3(Random.Range(-180, 180), transform.position.y, Random.Range(-180, 180)).normalized;
        rdmDistance = Random.Range(minMoveDist, maxMoveDist);
        movePosition = homePosition + (rdmDirection * rdmDistance);
        
        return movePosition;

    }

    void SetDestination(Vector3 target)
    {
        navAgent.SetDestination(target);
    }

    void SetModelParameters(Stack stack)
    {

        bCollider.center = stack.GetModelInfo().boxColliderCenter;
        bCollider.size = stack.GetModelInfo().boxColliderSize;

        cCollider.center = stack.GetModelInfo().capColliderCenter;
        cCollider.height = stack.GetModelInfo().capColliderHeight;
        cCollider.radius = stack.GetModelInfo().capColliderRadius;

        navAgent.height = stack.GetModelInfo().navMeshAgentHeight;
        navAgent.radius = stack.GetModelInfo().navMeshAgentRadius;

    }

    void SetActiveStack()
    {
        switch (type)
        {
            case EnemyType.Rock:
                activeStack = rockMonst;
                if(!activeStack.gameObject.activeSelf)
                {
                    activeStack.gameObject.SetActive(true);
                    SetModelParameters(activeStack);
                }
                break;
            case EnemyType.Snow:
                if (isAPassenger)
                {
                    switch (passengerNum)
                    {
                        case 1:
                            if (transform.parent.parent.GetComponent<StackableEnemy>().backPack.childCount == 1)
                            {
                                if (activeStack != null)
                                {
                                    activeStack.gameObject.SetActive(false);
                                }
                                activeStack = catHead;
                                if (!activeStack.gameObject.activeSelf)
                                {
                                    activeStack.gameObject.SetActive(true);
                                    SetModelParameters(activeStack);
                                }
                                
                            }
                            if (transform.parent.parent.GetComponent<StackableEnemy>().backPack.childCount == 2)
                            {
                                if (activeStack != null)
                                {
                                    activeStack.gameObject.SetActive(false);
                                }
                                activeStack = catMid;
                                if (!activeStack.gameObject.activeSelf)
                                {
                                    activeStack.gameObject.SetActive(true);
                                    SetModelParameters(activeStack);
                                }                                
                            }
                            break;
                        case 2:
                            if (activeStack != null)
                            {
                                activeStack.gameObject.SetActive(false);
                            }
                            activeStack = catHead;
                            if (!activeStack.gameObject.activeSelf)
                            {
                                activeStack.gameObject.SetActive(true);
                                SetModelParameters(activeStack);
                            }                            
                            break;
                    }
                }
                else
                {
                    switch (backPack.childCount)
                    {
                        case 0:
                            if (activeStack != null)
                            {
                                activeStack.gameObject.SetActive(false);
                            }
                            activeStack = catHead;
                            if (!activeStack.gameObject.activeSelf)
                            {
                                activeStack.gameObject.SetActive(true);
                                SetModelParameters(activeStack);
                            }
                            break;
                        case 1:
                            if (activeStack != null)
                            {
                                activeStack.gameObject.SetActive(false);
                            }
                            activeStack = catMid;
                            if (!activeStack.gameObject.activeSelf)
                            {
                                activeStack.gameObject.SetActive(true);
                                SetModelParameters(activeStack);
                            }
                            break;
                        case 2:
                            if (activeStack != null)
                            {
                                activeStack.gameObject.SetActive(false);
                            }
                            activeStack = catBase;
                            if (!activeStack.gameObject.activeSelf)
                            {
                                activeStack.gameObject.SetActive(true);
                                SetModelParameters(activeStack);
                            }
                            break;
                    }
                }
                break;
        }
        
    }

    public IEnumerator CheckHit(bool isGroundPound)
    {
        yield return 0f; //new WaitForSeconds(1f);

       

        if (!isGroundPound)
        {
            if (backPack.childCount > 0)
            {
                backPack.GetChild(backPack.childCount - 1).GetComponent<StackableEnemy>().SetDead();
                backPack.GetChild(backPack.childCount - 1).transform.parent = null;                
                if (backPack.childCount > 0)
                {
                    backPack.GetChild(backPack.childCount - 1).GetComponent<StackableEnemy>().SetActiveStack();
                }
                SetActiveStack();
            }
            else
            {
                SetDead();                
            }
        }
        else
        {
            foreach (var child in backPack)
            {
                GetComponent<StackableEnemy>().SetDead();
            }
            SetDead();
        }
    }
            //StartCoroutine(Die());
    

    public IEnumerator TakeDamage()
    {
        yield return 0f; //new WaitForSeconds(1f);
    }

    public IEnumerator Die()
    {
        //death anim
        //particle cue
        //collider off
        //cue sound fx

        yield return 0f;
        
        // maybe another particle effect here
        // mesh off
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
