//Stackable Enemy
//written by Michael Elkin 12-10-2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnowMan : MonoBehaviour, IKillable
{
    public enum SnowManStatus { Idle, Move, Chase, CoolDown, Expelled, Grow, Dead }
    [SerializeField]
    SnowManStatus status;


    // Cached for reference or size adjustment
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
    

    #region Course Plotting
    [SerializeField]
    Vector3 homePosition;
    [SerializeField]
    Vector3 movePosition;

    Vector3 rdmDirection;

    float rdmDistance = 0.0f;

    #endregion

    #region Grow
    // Collision Sort
    [SerializeField, Header("Stack Rank"), Range(0, 1)]
    float gScale = 1.0f;

    int _collisionRank = 0;
    public int collisionRank
    {
        get { return _collisionRank; }
        private set { _collisionRank = value; }
    }
    [SerializeField]
    SnowMan[] absorbed = new SnowMan[5];
    [SerializeField]
    int _absorbedIndex;
    public int absorbedIndex
    {
        get { return _absorbedIndex; }
        private set { _absorbedIndex = value; }
    }
    [SerializeField]
    Vector3 origScale;

    [SerializeField]
    bool _hasGrown;
    public bool hasGrown
    {
        get { return _hasGrown; }
        private set { _hasGrown = value; }

    }
    [SerializeField]
    float growRate = 0.0f;
    
    [SerializeField]
    bool animTrigg;

    #endregion

    #region Expelled

    Vector3 _landing;
    public Vector3 landing
    {
        get { return _landing; }
        set { _landing = value; }
    }


    #endregion

    [SerializeField, Range(0, 5)]
    float idleAnimTime = 1.0f;
    [SerializeField, Range(0, 5)]
    float coolDownAnimTime = 1.0f;
    [SerializeField, Range(0, 5)]
    float growAnimTime = 1.0f;
    [SerializeField, Range(0, 5)]
    float deathAnimTime = 1.0f;
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

    #region Status Bools

    [SerializeField]
    bool needIdleMovePos;
    [SerializeField]
    bool notMoving;
    [SerializeField]
    bool isChasing;
    [SerializeField]
    bool isGrowing;
    [SerializeField]
    bool coolingDown;
    [SerializeField]
    bool dying;

    #endregion



    private void Awake()
    {
        absorbedIndex = 0;
        origScale = transform.lossyScale;
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
                //boxTriggerCollider.size = new Vector3(1.0f, 0.125f, 1.0f);
                //boxTriggerCollider.center = new Vector3(0, 0.125f, 0);
            }
            else
            {
                boxCollider = box;
            }
        }
        GenCollisionRank();
        rb = GetComponent<Rigidbody>();
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        needIdleMovePos = true;
        status = SnowManStatus.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case SnowManStatus.Idle:
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
                    SetEnemyStatus(SnowManStatus.Chase);
                }
                break;
            case SnowManStatus.Move:
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

                        needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }
                    if(navAgent.pathStatus == NavMeshPathStatus.PathPartial || navAgent.isPathStale)
                    {
                        needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }
                }
                break;
            case SnowManStatus.Chase:
                if (PlayerInRange())
                {
                    if (playerRef.position != navAgent.destination)
                    {
                        SetDestination(playerRef.position);
                    }
                }
                else
                {
                    SetEnemyStatus(SnowManStatus.Idle);
                }
                break;
            case SnowManStatus.CoolDown:
                if (!coolingDown)
                {
                    coolingDown = true;
                    StartCoroutine(CooldownAnimation(coolDownAnimTime));// Change coolDownAnimTime to animation length
                }
                break;
            case SnowManStatus.Grow:
                if(!isGrowing)
                {
                    isGrowing = true;
                    growRate = 0.0f;
                    StartCoroutine(GrowAnimation(growAnimTime));
                }
                else
                {
                    growRate += Time.deltaTime ;
                    
                    transform.localScale = Vector3.Lerp(transform.lossyScale, transform.lossyScale + (origScale * gScale), growRate);

                    if (growRate > 1.0f)
                    {
                        needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }
                }
                
                break;
            case SnowManStatus.Expelled:
                if(transform.position != landing)
                {
                    if (growRate < 1.0f)
                    {
                        growRate += Time.deltaTime;
                    }
                    transform.position = Vector3.Lerp(transform.position, landing, growRate);
                }
                else
                {
                    SetEnemyStatus(SnowManStatus.Idle);
                    needIdleMovePos = true;
                }


                break;

            case SnowManStatus.Dead:
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
        if (o.gameObject.GetComponent<SnowMan>() && status != SnowManStatus.Grow)
        {
            if (!hasGrown && !o.GetComponent<SnowMan>().hasGrown)
            {
                if (collisionRank > o.GetComponent<SnowMan>().collisionRank)
                {
                    absorbed[absorbedIndex] = o.GetComponent<SnowMan>();
                    absorbed[absorbedIndex].gameObject.SetActive(false);
                    absorbedIndex++;
                    navAgent.isStopped = true;
                    SetEnemyStatus(SnowManStatus.Grow);
                    isGrowing = false;
                    hasGrown = true;
                }
            }
            else
            {
                if (hasGrown && !o.GetComponent<SnowMan>().hasGrown)
                {
                    absorbed[absorbedIndex] = o.GetComponent<SnowMan>();
                    absorbed[absorbedIndex].gameObject.SetActive(false);
                    absorbedIndex++;
                    navAgent.isStopped = true;
                    SetEnemyStatus(SnowManStatus.Grow);
                    isGrowing = false;
                }
            }
        }
    }    

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

    public void SetEnemyStatus(SnowManStatus newstatus)
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

    void GenCollisionRank()
    {
        collisionRank = Random.Range(1, 100);
    }

    

    


    #region Animation Coroutines

    IEnumerator IdleAnimation(float t)
    {
        // Insert Idle Stacking Trigger here

        yield return new WaitForSeconds(t);
        //Debug.Log("Idle wait finished");
        notMoving = true;
        SetEnemyStatus(SnowManStatus.Move);

    }

    IEnumerator CooldownAnimation(float t)
    {
        // Insert CoolDown Animation trigger here

        yield return new WaitForSeconds(t);
        needIdleMovePos = true;
        SetEnemyStatus(SnowManStatus.Idle);

    }

    IEnumerator DeathAnimation(float t)
    {
        // Insert Death Animation trigger here
        boxTriggerCollider.enabled = false;
        boxCollider.enabled = false;
        //navAgent.enabled = false;
        yield return new WaitForSeconds(t);
        transform.gameObject.SetActive(false);

    } 
    
    IEnumerator GrowAnimation(float t)
    {
        // Insert Grow Animation Trigger here
        anim.SetTrigger("Absorbed");
        
        yield return new WaitForSeconds(t);

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
    public float GetCollisionRank()
    {
        return collisionRank;
    }
    
    
    // Player Collision functions
    public void StartCoolDown()
    {
        coolingDown = false;
        SetEnemyStatus(SnowManStatus.CoolDown);
    }
    public void SetDead()
    {
        dying = false;
        SetEnemyStatus(SnowManStatus.Dead);
    }
    #endregion


    


    public IEnumerator CheckHit(bool x)
    {
        yield return 0f;
        if (!x)
        {
            if (absorbed.Length > 0)
            {
                int rdmNum = Random.Range(1, 4);
                switch (rdmNum)
                {
                    case 1:
                        absorbed[absorbedIndex - 1].transform.position = transform.position;
                        absorbed[absorbedIndex - 1].landing = transform.right * 3.0f;
                        absorbed[absorbedIndex - 1].SetEnemyStatus(SnowManStatus.Expelled);
                        absorbed[absorbedIndex - 1].growRate = 0.0f;
                        absorbed[absorbedIndex - 1] = null;
                        absorbedIndex--;

                        break;
                    case 2:
                        absorbed[absorbedIndex - 1].transform.position = transform.position;
                        absorbed[absorbedIndex - 1].landing = -transform.right * 3.0f;
                        absorbed[absorbedIndex - 1].SetEnemyStatus(SnowManStatus.Expelled);
                        absorbed[absorbedIndex - 1].growRate = 0.0f;
                        absorbed[absorbedIndex - 1] = null;
                        absorbedIndex--;

                        break;
                    case 3:
                        absorbed[absorbedIndex - 1].transform.position = transform.position;
                        absorbed[absorbedIndex - 1].landing = transform.forward * 3.0f;
                        absorbed[absorbedIndex - 1].SetEnemyStatus(SnowManStatus.Expelled);
                        absorbed[absorbedIndex - 1].growRate = 0.0f;
                        absorbed[absorbedIndex - 1] = null;
                        absorbedIndex--;

                        break;
                    case 4:
                        absorbed[absorbedIndex - 1].transform.position = transform.position;
                        absorbed[absorbedIndex - 1].landing = -transform.forward * 3.0f;
                        absorbed[absorbedIndex - 1].SetEnemyStatus(SnowManStatus.Expelled);
                        absorbed[absorbedIndex - 1].growRate = 0.0f;
                        absorbed[absorbedIndex - 1] = null;
                        absorbedIndex--;

                        break;
                }
            }
        }
        else
        {
            SetEnemyStatus(SnowManStatus.Dead);
            dying = false;
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
