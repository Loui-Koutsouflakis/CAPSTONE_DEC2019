using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MotherSpider : MonoBehaviour, IKillable
{
    #region variables
    public GameObject eggSackPrefab;
    public List<GameObject> crystals = new List<GameObject>();
    public Transform spinneret;
    public SpiderMotherAnimControls animController;
    public Material disolveShader;
    public int hitPoints = 3;
    public int eggSackPoolSize = 5;
    public float maxSpawnRange = 5f;
    public bool seesPlayer = false;
    public bool vulnerable = false;
    public float vulnerableTime = 5;
    public float meleeAttackRange = 5f;

    private Transform playerTransform;
    public SpiderMotherState currentState;
    private Vector3 locationSpawned;
    [SerializeField]
    private Vector3 destination;
    private NavMeshAgent navAgent;
    private List<GameObject> eggSacks = new List<GameObject>();
    [SerializeField]
    private List<Collider> spiderColliders = new List<Collider>();
    //[SerializeField]
    //private List<>
    [SerializeField]
    private List<Collider> meshCollidersToes = new List<Collider>();
    private NavMeshPath path;
    [SerializeField]
    private List<GameObject> spiderlings = new List<GameObject>();
    //[SerializeField]
    //private MeshCollider attackCollider;
    private Material newMat;
    private Material mat;

    #endregion


    #region Awake and Update
    private void Awake()
    {
        newMat = new Material(disolveShader);

        navAgent = GetComponent<NavMeshAgent>();
        locationSpawned = transform.position;
        GetChildRecursive(gameObject);
        GameObject spiderling;

        for (int i = 0; i < eggSackPoolSize; i++)
        {
            eggSacks.Add(Instantiate(eggSackPrefab));
            eggSacks[i].SetActive(false);

            for(int a = 0; a < eggSacks[i].GetComponent<EggSack>().SpiderlingsPool().Length; a++)
            {
                spiderling = eggSacks[i].GetComponent<EggSack>().SpiderlingsPool()[a];
                spiderling.GetComponent<Spiderlings>().SetSpiderTransform(transform);
                spiderlings.Add(spiderling);
            }
        }

        SetState(SpiderMotherState.Hiding);
    }

    private void Start()
    {
        //attackCollider = GetComponent<MeshCollider>();

    }

    bool pathOkay = true;
    int wonderCounter = 0;
    // Update is called once per frame
    void Update()
    {
        pathOkay = true;


        if(hitPoints < 1)
        {
            if(navAgent.enabled) navAgent.SetDestination(transform.position);
            navAgent.enabled = false;
            currentState = SpiderMotherState.Dead;
            return;
        }

        if (navAgent.enabled && playerTransform != null)
        {
            path = new NavMeshPath();
            navAgent.CalculatePath(playerTransform.position, path);
            if (path.status == NavMeshPathStatus.PathPartial
                || path.status == NavMeshPathStatus.PathInvalid)
            {
                pathOkay = false;
            }
        }

        if (seesPlayer && currentState == SpiderMotherState.Hiding && pathOkay)
        {
            SetState(SpiderMotherState.Reveal);
            //Debug.Log("In Here");
        }



        if (seesPlayer &&
            pathOkay
            && (currentState == SpiderMotherState.Wonder || currentState == SpiderMotherState.ChasePlayer))
        {
            SetState(SpiderMotherState.ChasePlayer);
        }
        else if (seesPlayer && currentState == SpiderMotherState.ChasePlayer 
            && !pathOkay)
        {
            SetState(SpiderMotherState.Wonder);

        }
        if(currentState == SpiderMotherState.Wonder && (destination - transform.position).sqrMagnitude < 5)
        {
            SetState(SpiderMotherState.Wonder);
            wonderCounter++;
        }

        if (!seesPlayer && currentState == SpiderMotherState.ChasePlayer)
        {
            SetState(SpiderMotherState.Wonder);
        }


        if(playerTransform != null && (playerTransform.position - transform.position).magnitude < meleeAttackRange && currentState == SpiderMotherState.ChasePlayer)
        {
            SetState(SpiderMotherState.Attack);
        }

        if(wonderCounter > 3)
        {
            SetState(SpiderMotherState.Burry);
            wonderCounter = 0;
        }
    }
    #endregion

    public bool burry = false;
    public bool reveal = false;

    #region set state
    public void SetState(SpiderMotherState state)
    {
        switch(state)
        {
            case SpiderMotherState.Hiding:
                if(spiderColliders[0].enabled == true)
                {
                    ToggleColliders();
                    navAgent.enabled = false;
                }
                destination = transform.position;
                currentState = SpiderMotherState.Hiding;
                break;

            case SpiderMotherState.Reveal:

                animController.TriggerReveal();
                StartCoroutine(RotateOnReveale());
                currentState = SpiderMotherState.Reveal;
                break;

            case SpiderMotherState.ChasePlayer:
                wonderCounter = 0;
                if (navAgent.enabled == false) navAgent.enabled = true;
                if (playerTransform != null) destination = playerTransform.position;
                else SetState(SpiderMotherState.Wonder);
                navAgent.SetDestination(destination);
                for (int i = 0; i < spiderlings.Count; i++)
                {
                    if (spiderlings[i].activeSelf) spiderlings[i].GetComponent<Spiderlings>().SetState(SpiderlingState.Chaseplayer);
                }
                currentState = SpiderMotherState.ChasePlayer;

                break;

            case SpiderMotherState.Attack:
                navAgent.isStopped = true;
                currentState = SpiderMotherState.Attack;
                StartCoroutine(Attack());
                //gameObject.layer = 10;

                break;

            case SpiderMotherState.Vulnerable:
                if (gameObject.layer != 0) gameObject.layer = 0;
       
                currentState = SpiderMotherState.Vulnerable;

                EnterVulnerable();

                break;

            case SpiderMotherState.Wonder:
                destination  = new Vector3(
                    Random.Range(locationSpawned.x - 40, locationSpawned.x + 40),
                    transform.position.y,
                    Random.Range(locationSpawned.z - 40, locationSpawned.z + 40));
                NavMeshPath path = new NavMeshPath();
                navAgent.CalculatePath(destination, path);

                while (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                {
                    destination = new Vector3(
                        Random.Range(locationSpawned.x - 20, locationSpawned.x + 20),
                        transform.position.y,
                        Random.Range(locationSpawned.z - 20, locationSpawned.z + 20));

                    navAgent.CalculatePath(destination, path);

                }
                for (int i = 0; i < spiderlings.Count; i++)
                {
                    if (spiderlings[i].activeSelf) spiderlings[i].GetComponent<Spiderlings>().SetState(SpiderlingState.FollowSpiderMother);
                }
                currentState = SpiderMotherState.Wonder;
                navAgent.SetDestination(destination);
                break;

            case SpiderMotherState.Burry:

                navAgent.isStopped = true;
                animController.TriggerBurry();
                if (spiderColliders[0].enabled == true)
                {
                    //ToggleColliders();
                    navAgent.enabled = false;
                }
                destination = transform.position;
                currentState = SpiderMotherState.Burry;
                break;

            case SpiderMotherState.Dead:


                break;
        }
    }
    #endregion

    public IEnumerator RotateOnReveale()
    {
        bool rotated = false;
        bool rotating = true;

        while (rotating)
        {
            Vector3 newDirection = playerTransform.position - transform.position;
            newDirection.y = transform.forward.y;
            float angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

            if (angle > 3f)
            {
                rotated = true;
                transform.Rotate(transform.up, -rotationSpeed);
            }
            else if (angle < -3)
            {
                rotated = true;
                transform.Rotate(transform.up, rotationSpeed);
            }
            else
            {
                rotated = false;
            }

            if (!rotated)
            {
                rotating = false;
            }

            yield return new WaitForSecondsRealtime(1 / 60);
        }

    }

    #region EnterVulnerable
    float retreatDistance = 0;
    private void EnterVulnerable()
    {


        float tempASpeed = navAgent.angularSpeed;
        navAgent.angularSpeed = 0;
        destination = transform.position;

        navAgent.destination = destination;

        destination = transform.position;

        navAgent.isStopped = true;
        navAgent.angularSpeed = tempASpeed;
        vulnerable = true;
        StartCoroutine(VulnerableTimer());
        animController.SetVaulnerableBool(true);
    }
    #endregion

    

    #region get all colliders
    private void GetChildRecursive(GameObject obj)
    {
        if (null == obj)
            return;

        mat = newMat;
        mat.SetFloat("_Strength", 0.0f);

        if (spiderColliders.Contains(gameObject.GetComponent<BoxCollider>()) == false) spiderColliders.Add(GetComponent<BoxCollider>());
        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            if (child.GetComponent<Collider>() != null)
            {
                if (child.gameObject.tag == "SpiderToes") meshCollidersToes.Add(child.GetComponent<Collider>());
                else /*if (meshColliders.Contains(gameObject.GetComponent<BoxCollider>()) == false)*/ spiderColliders.Add(child.GetComponent<Collider>());
            }

            if(child.gameObject.tag == "SpiderToes" || child.gameObject.tag == "SpiderBody")
            {
                child.gameObject.GetComponent<Renderer>().material = mat;
            }

            if(child.gameObject.tag == "Crystals")
            {
                crystals.Add(child.gameObject);
            }

            GetChildRecursive(child.gameObject);
        }


    }
    #endregion

    public bool stillRotatingEgg = true;

    #region ToggleColliders
    public void ToggleColliders()
    {
        if (spiderColliders[0].enabled == true)
        {
            for (int i = 0; i < spiderColliders.Count; i++)
            {
                spiderColliders[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < spiderColliders.Count; i++)
            {
                spiderColliders[i].enabled = true;
            }
        }
    }
    #endregion

    #region SpawnEggSack
    public void SpawnEggSack()
    {
        Vector3 raycastPosS = transform.position + (transform.forward * maxSpawnRange);
        Vector3 raycastPosE = raycastPosS - new Vector3(0, 15, 0);
        RaycastHit hit;
        Physics.Linecast(raycastPosS, raycastPosE, out hit);

        stillRotatingEgg = false;
        for (int i = 0; i < eggSacks.Count; i++)
        {
            if (!eggSacks[i].GetComponent<EggSack>().SpiderlingsDead())
            {
                eggSacks[i].transform.position = hit.point + (transform.forward * maxSpawnRange);
                eggSacks[i].SetActive(true);
                i = eggSacks.Count;
            }
        }

    }
    #endregion

    float rotationSpeed = 5;
    private IEnumerator Attack()
    {
        if (currentState == SpiderMotherState.Attack)
        {
            bool rotated = false;
            bool rotating = true;
            navAgent.destination = transform.position;
            navAgent.enabled = false;
            animController.TriggerAttack();
            while (rotating)
            {
                Vector3 newDirection = playerTransform.position - transform.position;
                newDirection.y = transform.forward.y;
                float angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

                if (angle > 5)
                {
                    rotated = true;
                    transform.Rotate(transform.up, -rotationSpeed);
                }
                else if (angle < -5)
                {
                    rotated = true;
                    transform.Rotate(transform.up, rotationSpeed);
                }
                else
                {
                    rotated = false;
                }
                if (!rotated)
                {
                    rotating = false;
                }

                if (hitPoints < 1) rotating = false;
                if (currentState != SpiderMotherState.Attack) rotating = false;

                yield return new WaitForSecondsRealtime(1 / 60);
            }

        }

    }

    #region VulnerableTimer

    public IEnumerator VulnerableTimer()
    {

        for (int i = 0; i < vulnerableTime; i++)
        {
            yield return new WaitForSecondsRealtime(1);
        }
        vulnerable = false;
        if(navAgent.enabled)
        navAgent.isStopped = false;
        animController.SetVaulnerableBool(false);
    }

    #endregion

    #region Trigger enter and exit
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "InteractTriggerSphere" && other.gameObject.layer == 15 && (other.transform.position - transform.position).magnitude > 5)
        {
            seesPlayer = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "InteractTriggerSphere" && other.gameObject.layer == 15 && (other.transform.position - transform.position).magnitude > 5)
        {
            seesPlayer = false;
            playerTransform = null;
        }
    }
    #endregion
    
    #region Ikillable interface
    public IEnumerator CheckHit(bool x)
    {

        if (vulnerable && gameObject.activeSelf) StartCoroutine(TakeDamage());
        if (x && currentState == SpiderMotherState.ChasePlayer || currentState == SpiderMotherState.Wonder /*|| currentState == SpiderMotherState.Attack*/) SetState(SpiderMotherState.Vulnerable);
        yield return 0;
    }

    public IEnumerator TakeDamage()
    {
        hitPoints--;
        animController.TriggerHit();
        //if(vulnerable)
        //{
            vulnerable = false;
        //}
        for(int i = 0; i < crystals.Count; i++)
        {
            if(crystals[i].activeSelf)
            {
                crystals[i].SetActive(false);
                i = crystals.Count;
            }
        }
        Debug.Log("SpiderMother hit");

        if (hitPoints < 1)
        {
            currentState = SpiderMotherState.Dead;
            StartCoroutine(Die());
        }


        yield return 0;
    }

    public IEnumerator Die()
    {
        Debug.Log("SpiderMother died");

        animController.TriggerDeath();
        yield return 0;
    }
    #endregion

    public IEnumerator DeactivateSpider()
    {
        for (float i = 0; i < 0.8f; i += 0.05f)
        {

            mat.SetFloat("_Strength", i);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        gameObject.SetActive(false);

    }
}

public enum SpiderMotherState
{
    Hiding,
    Reveal,
    Burry,
    Wonder,
    ChasePlayer,
    Attack,
    Vulnerable,
    SpawnEggSack, 
    Dead
}
