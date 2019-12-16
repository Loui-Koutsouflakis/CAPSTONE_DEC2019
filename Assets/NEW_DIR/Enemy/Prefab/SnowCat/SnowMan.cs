//Stackable Enemy
//written by Michael Elkin 12-10-2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnowMan : MonoBehaviour, IKillable
{
    public enum SnowManStatus { Idle, Move, Chase, CoolDown, Absorbed, Expelled, Grow, Shrink, Dead }
    [SerializeField, Header("Current State")]
    SnowManStatus status;


    // Cached for reference or size adjustment
    [SerializeField, Header("Cached References")]
    BoxCollider boxCollider;
    [SerializeField]
    BoxCollider boxTriggerCollider;
    [SerializeField]
    NavMeshAgent navAgent;
    [SerializeField]
    Animator anim;
    [SerializeField]
    LayerMask hittableObjects;

    //[SerializeField]
    //Rigidbody rb;
    [SerializeField]
    Transform playerRef;
    [SerializeField]
    Transform backPack;//Drag and Drop emptyobject centered on Object.
    [SerializeField]
    StackAStan parent;
    [SerializeField]
    Transform expelPoint;
    
    MeshRenderer[] rend;

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
    [SerializeField, Header("Growth Factor"), Range(0, 1)]
    float gScale = 1.0f;
    int _collisionRank = 0;
    public int collisionRank
    {
        get { return _collisionRank; }
        private set { _collisionRank = value; }
    }
    [SerializeField, Header("Enemies Held")]
    SnowMan[] absorbed = new SnowMan[5];
    //[SerializeField]
    int _absorbedIndex;
    public int absorbedIndex
    {
        get { return _absorbedIndex; }
        private set { _absorbedIndex = value; }
    }
    //[SerializeField]
    Vector3 origScale;
    //[SerializeField]
    bool _hasGrown;
    public bool hasGrown
    {
        get { return _hasGrown; }
        private set { _hasGrown = value; }
    }
    //[SerializeField]
    float growRate = 0.0f;
    
    #endregion

    #region Expelled

    Vector3 _landing;
    public Vector3 landing
    {
        get { return _landing; }
        set { _landing = value; }
    }
    [SerializeField, Range(3, 15), Header("Distance Expelled")]
    int expelDist = 15;
    Vector3 direction;

    #endregion

    [SerializeField, Range(0, 5), Header("Coroutine Timers")]
    float idleAnimTime = 1.0f;
    [SerializeField, Range(0, 5)]
    float coolDownAnimTime = 1.0f;
    [SerializeField, Range(0, 5)]
    float growAnimTime = 1.0f;
    [SerializeField, Range(0, 5)]
    float deathAnimTime = 1.0f;

    [SerializeField, Range(0, 10), Header("Movement Settings")]
    float chaseRange = 5.0f;
    [SerializeField, Range(0, 5)]
    float minMoveDist = 5.0f;
    [SerializeField, Range(5, 10)]
    float maxMoveDist = 5.0f;
    [SerializeField, Range(0, 5)]
    float moveSpeed = 5.0f;
    [SerializeField, Range(0, 5)]
    float moveAccel = 5.0f;
    [SerializeField, Range(0, 5)]
    float chaseSpeed = 5.0f;
    [SerializeField, Range(0, 5)]
    float chaseAccel = 5.0f;

    #region Status Bools

    [SerializeField, Header("State Bools")]
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
    [SerializeField]
    bool animTrigg;
    [SerializeField]
    bool groundPound;
    [SerializeField]
    bool b_Absorbed;
    #endregion



    private void Awake()
    {
        absorbedIndex = 0;
        origScale = transform.lossyScale;
        homePosition = transform.position;
        rend = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;
        navAgent.acceleration = moveAccel;
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
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        needIdleMovePos = true;
        status = SnowManStatus.Idle;
        
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(gameObject.name.ToString() + navAgent.pathStatus.ToString());
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
                    anim.speed = 1.5f;
                    navAgent.speed = chaseSpeed;
                    navAgent.acceleration = chaseAccel;
                    SetEnemyStatus(SnowManStatus.Chase);
                }
                break;
            case SnowManStatus.Move:
                if (!PlayerInRange())
                {
                    if (notMoving)
                    {
                        // Trigger move animation start
                        anim.SetBool("Moving", true);
                        notMoving = false;
                        //navAgent.isStopped = false;
                        SetDestination(GetIdleMoveDest());
                        navAgent.isStopped = false;
                    }
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        anim.SetBool("Moving", false);
                        needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }
                    if(navAgent.pathStatus == NavMeshPathStatus.PathPartial || navAgent.isPathStale)
                    {
                        anim.SetBool("Moving", false);
                        needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }
                }
                else
                {
                    anim.speed = 1.5f;
                    navAgent.speed = chaseSpeed;
                    navAgent.acceleration = chaseAccel;
                    SetEnemyStatus(SnowManStatus.Chase);
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
                    anim.speed = 1.0f;
                    navAgent.speed = moveSpeed;
                    navAgent.acceleration = moveAccel;
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
                    
                    transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale + (origScale * gScale), growRate);

                    if (growRate > 1.0f)
                    {
                        needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }
                }
                
                break;
            case SnowManStatus.Shrink:
                if (groundPound)
                {
                    if (!isGrowing)
                    {
                        isGrowing = true;
                        growRate = 0.0f;
                        StartCoroutine(GrowAnimation(growAnimTime));
                    }
                    else
                    {
                        growRate += Time.deltaTime;

                        transform.localScale = Vector3.Lerp(transform.lossyScale, origScale, growRate);      

                        if (growRate > 1.0f)
                        {
                            needIdleMovePos = true;
                            groundPound = false;
                            transform.localScale = origScale;
                            SetEnemyStatus(SnowManStatus.Idle);
                        }
                    }
                }
                else
                {
                    if (!isGrowing)
                    {
                        isGrowing = true;
                        growRate = 0.0f;
                        StartCoroutine(GrowAnimation(growAnimTime));
                    }
                    else
                    {
                        growRate += Time.deltaTime;

                        if (transform.lossyScale.magnitude > origScale.magnitude)
                        {
                            transform.localScale = Vector3.Lerp(transform.lossyScale, transform.lossyScale - (origScale * gScale), growRate);
                        }

                        if (growRate > 1.0f)
                        {
                            if (transform.localScale.magnitude < 0)
                            {
                                transform.localScale = origScale;

                            }
                            needIdleMovePos = true;
                            SetEnemyStatus(SnowManStatus.Idle);
                        }
                    }
                }

                break;
            case SnowManStatus.Absorbed:
                

                break;
            case SnowManStatus.Expelled:
                if(transform.position != landing)
                {
                    if (growRate < 1.0f)
                    {
                        growRate += Time.deltaTime * 0.25f;
                    }
                    transform.position = Vector3.Lerp(transform.position, landing, growRate);
                    if (Physics.Raycast(transform.position, (landing - transform.position).normalized, 1.0f, hittableObjects))
                    {
                        RaycastHit downhit;
                        Physics.Raycast(transform.position, -transform.up, out downhit, 10.0f);
                        transform.position = transform.position - (-transform.up * downhit.distance);
                        boxCollider.enabled = true;
                        boxTriggerCollider.enabled = true;
                        homePosition = transform.position;
                        navAgent.enabled = true;
                        //needIdleMovePos = true;
                        SetEnemyStatus(SnowManStatus.Idle);
                    }

                }
                else
                {                    
                    boxCollider.enabled = true;
                    boxTriggerCollider.enabled = true;
                    homePosition = transform.position;
                    navAgent.enabled = true;
                    //needIdleMovePos = true;
                    SetEnemyStatus(SnowManStatus.Idle);
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
        if (o.gameObject.GetComponent<SnowMan>() && status != SnowManStatus.Grow || status != SnowManStatus.Shrink || status != SnowManStatus.Expelled)
        {
            if (!hasGrown && !o.GetComponent<SnowMan>().hasGrown)
            {
                if (collisionRank > o.GetComponent<SnowMan>().collisionRank)
                {
                    absorbed[absorbedIndex] = o.GetComponent<SnowMan>();
                    //absorbed[absorbedIndex].gameObject.SetActive(false);
                    absorbed[absorbedIndex].SetAbsorbed(transform);
                    navAgent.isStopped = true;
                    SetEnemyStatus(SnowManStatus.Grow);
                    isGrowing = false;
                    hasGrown = true;
                    absorbedIndex++;
                }
            }
            else
            {
                if (hasGrown && !o.GetComponent<SnowMan>().hasGrown)
                {
                    absorbed[absorbedIndex] = o.GetComponent<SnowMan>();
                    //absorbed[absorbedIndex].gameObject.SetActive(false);
                    absorbed[absorbedIndex].SetAbsorbed(transform);
                    navAgent.isStopped = true;
                    SetEnemyStatus(SnowManStatus.Grow);
                    isGrowing = false;
                    absorbedIndex++;
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

    void BoolReset()
    {
        needIdleMovePos = true;
        notMoving = false;
        coolingDown = false;
        isGrowing = false;
        groundPound = false;
        dying = false;
    }

    //void CheckLandingPos(Vector3 landingPos)
    //{
    //    Vector3 checkPos = transform.position + (Vector3.up * 0.25f);
    //    for (int i = expelDist; i > 0; i--)
    //    {
    //        if(!Physics.Raycast(checkPos, (landingPos - checkPos).normalized, i, hittableObjects))
    //        {
    //            Debug.Log("Mag = " + (landingPos - expelPoint.position).magnitude);
    //            Debug.DrawRay(expelPoint.position, (landingPos - expelPoint.position).normalized, Color.red, expelDist);
    //            landing = (landingPos - checkPos).normalized * i;
    //            i = 0;
    //        }
    //        else
    //        {
    //            Debug.DrawRay(expelPoint.position, (landingPos - expelPoint.position).normalized, Color.red, expelDist);
    //            landing = (landingPos - checkPos).normalized * i;
    //        }
    //    }
    //}


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
        if(navAgent.enabled)
        {
            navAgent.enabled = false;
        }
        anim.SetTrigger("Dying");
        //navAgent.enabled = false;
        yield return new WaitForSeconds(t);
        //transform.gameObject.SetActive(false);

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
    public void SetAbsorbed(Transform parentSet)
    {
        navAgent.enabled = false;
        boxCollider.enabled = false;
        boxTriggerCollider.enabled = false;
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = false;
        }
        transform.parent = parentSet;
        transform.position = transform.parent.position;
        b_Absorbed = true;
        SetEnemyStatus(SnowManStatus.Absorbed);
        
    }
    
    // Player Collision functions
    public void StartCoolDown()
    {
        coolingDown = false;
        SetEnemyStatus(SnowManStatus.CoolDown);
    }
    public void SetExpelled()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = true;
        }

        BoolReset();

        transform.parent = null;
        transform.localScale = origScale;
        SetEnemyStatus(SnowManStatus.Expelled);
        isGrowing = false;
    }
    public void SetDead()
    {
        dying = false;
        SetEnemyStatus(SnowManStatus.Dead);
    }
    public void DeactivateEnemy()
    {
        gameObject.SetActive(false);
    }
    #endregion


    


    public IEnumerator CheckHit(bool x)
    {
        yield return 0f;
        if (!x)
        {
            if (absorbedIndex > 0)
            {
                int rdmNum = Random.Range(1, 4);
                switch (rdmNum)
                {
                    case 1:
                        expelPoint.position = transform.position + (Vector3.up * 1.5f) + (transform.right * 0.5f);
                        absorbed[absorbedIndex - 1].transform.position = expelPoint.position;
                        absorbed[absorbedIndex - 1].transform.rotation = transform.rotation;                        
                        absorbed[absorbedIndex - 1].landing = (transform.position + (Vector3.up * 0.25f)) + (transform.right * expelDist);

                        //CheckLandingPos(landing);
                        //absorbed[absorbedIndex - 1].SetExpelled();                        
                        //absorbed[absorbedIndex - 1] = null;
                        //absorbedIndex--;

                        //SetEnemyStatus(SnowManStatus.Shrink);
                        //isGrowing = false;
                        //groundPound = false;

                        break;
                    case 2:
                        expelPoint.position = transform.position + (Vector3.up * 1.5f) + (-transform.right * 0.5f);
                        absorbed[absorbedIndex - 1].transform.position = expelPoint.position;
                        absorbed[absorbedIndex - 1].transform.rotation = transform.rotation;                       
                        absorbed[absorbedIndex - 1].landing = (transform.position + (Vector3.up * 0.25f)) + (-transform.right * expelDist);

                        //CheckLandingPos(landing);
                        //absorbed[absorbedIndex - 1].SetExpelled();
                        //absorbed[absorbedIndex - 1] = null;
                        //absorbedIndex--;

                        //SetEnemyStatus(SnowManStatus.Shrink);
                        //isGrowing = false;
                        //groundPound = false;

                        break;
                    case 3:
                        expelPoint.position = transform.position + (Vector3.up * 1.5f) + (transform.forward * 0.5f);
                        absorbed[absorbedIndex - 1].transform.position = expelPoint.position;
                        absorbed[absorbedIndex - 1].transform.rotation = transform.rotation;
                        absorbed[absorbedIndex - 1].landing = (transform.position + (Vector3.up * 0.25f)) + (transform.forward * expelDist);

                        //CheckLandingPos(landing);
                        //absorbed[absorbedIndex - 1].SetExpelled();
                        //absorbed[absorbedIndex - 1] = null;
                        //absorbedIndex--;

                        //SetEnemyStatus(SnowManStatus.Shrink);
                        //isGrowing = false;
                        //groundPound = false;

                        break;
                    case 4:
                        expelPoint.position = transform.position + (Vector3.up * 1.5f) + (-transform.forward * 0.5f);
                        absorbed[absorbedIndex - 1].transform.position = expelPoint.position;
                        absorbed[absorbedIndex - 1].transform.rotation = transform.rotation;                        
                        absorbed[absorbedIndex - 1].landing = (transform.position + (Vector3.up * 0.25f)) + (-transform.forward * expelDist);

                        //CheckLandingPos(landing);
                        //absorbed[absorbedIndex - 1].SetExpelled();
                        //absorbed[absorbedIndex - 1] = null;
                        //absorbedIndex--;

                        //groundPound = false;
                        //isGrowing = false;
                        //SetEnemyStatus(SnowManStatus.Shrink);

                        break;
                }
                //CheckLandingPos(landing);
                absorbed[absorbedIndex - 1].SetExpelled();
                absorbed[absorbedIndex - 1] = null;
                absorbedIndex--;

                groundPound = false;
                isGrowing = false;
                SetEnemyStatus(SnowManStatus.Shrink);
            }
            else
            {
                SetDead();
            }
        }
        else
        {
            if (absorbedIndex > 0)
            {
                
                for (int i = 0; i < absorbedIndex; i++)
                {
                    switch (i)
                    {
                        case 0:
                            direction = transform.forward;
                            break;
                        case 1:
                            direction = transform.right;
                            break;
                        case 2:
                            direction = -transform.forward;
                            break;
                        case 3:
                            direction = -transform.right;
                            break;
                        case 4:
                            direction = transform.forward;
                            break;                        
                    }
                    absorbed[absorbedIndex - 1 - i].transform.position = expelPoint.position + (direction * 0.5f);
                    absorbed[absorbedIndex - 1 - i].transform.rotation = transform.rotation;
                    absorbed[absorbedIndex - 1 - i].landing = transform.position + direction * expelDist;
                    //CheckLandingPos(landing);
                    absorbed[absorbedIndex - 1 - i].SetExpelled();
                    absorbed[absorbedIndex - 1 - i] = null;
                    //absorbedIndex--;

                }
                absorbedIndex = 0;
                groundPound = true;
                hasGrown = false;
                isGrowing = false;
                SetEnemyStatus(SnowManStatus.Shrink);
            }
            else
            {
                SetDead();
            }
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
