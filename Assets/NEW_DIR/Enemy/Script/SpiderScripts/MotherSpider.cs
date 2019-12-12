using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MotherSpider : MonoBehaviour, IKillable
{
    #region variables
    public GameObject eggSackPrefab;
    public Transform spinneret;
    public SpiderMotherAnimControls animController;
    public int hitPoints = 3;
    public int eggSackPoolSize = 5;
    public float maxSpawnRange = 5f;
    public bool seesPlayer = false;
    public float meleeAttackRange = 5f;

    private Transform playerTransform;
    private SpiderState currentState;
    private Vector3 locationSpawned;
    private Vector3 destination;
    private NavMeshAgent navAgent;
    private List<GameObject> eggSacks = new List<GameObject>();
    private List<Collider> meshColliders = new List<Collider>();
    private List<Collider> meshCollidersToes = new List<Collider>();
    #endregion

    #region Awake and Update
    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        locationSpawned = transform.position;
        GetChildRecursive(gameObject);


        for (int i = 0; i < eggSackPoolSize; i++)
        {
            eggSacks.Add(Instantiate(eggSackPrefab));
            eggSacks[i].SetActive(false);
        }

        SetState(SpiderState.Hiding);
    }

    // Update is called once per frame
    void Update()
    {
        if (seesPlayer && currentState == SpiderState.Hiding)
        {
            SetState(SpiderState.Reveal);
        }

        if( currentState == SpiderState.ChasePlayer)
        {
            SetState(currentState);
        }

        //if (playerTransform != null 
        //    && (playerTransform.position - transform.position).magnitude < meleeAttackRange)
        //{
        //    SetState(SpiderState.Attack);   
        //}
    }
    #endregion

    #region set state
    public void SetState(SpiderState state)
    {
        switch(state)
        {
            case SpiderState.Hiding:
                if(meshColliders[0].enabled == true)
                {
                    ToggleColliders();
                    navAgent.enabled = false;
                }
                destination = transform.position;
                currentState = SpiderState.Hiding;
                break;

            case SpiderState.Reveal:

                animController.TriggerReveal();
                currentState = SpiderState.Reveal;
                break;

            case SpiderState.ChasePlayer:
                if (navAgent.enabled == false) navAgent.enabled = true;
                navAgent.SetDestination(playerTransform.position);
                currentState = SpiderState.ChasePlayer;

                break;

            case SpiderState.Attack:

                currentState = SpiderState.Attack;
                StartCoroutine(Attack());
                break;

            case SpiderState.Vulnerable:

                destination = transform.position;

                currentState = SpiderState.Vulnerable;
                break;

            case SpiderState.Wonder:

                currentState = SpiderState.Wonder;
                break;

            case SpiderState.Burry:
                
                animController.TriggerBurry();

                currentState = SpiderState.Burry;
                break;
        }
    }
    #endregion

    #region get all colliders
    private void GetChildRecursive(GameObject obj)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            if (child.GetComponent<MeshCollider>() != null)
            {
                if (child.gameObject.tag == "SpiderToes") meshCollidersToes.Add(child.GetComponent<MeshCollider>());
                else meshColliders.Add(child.GetComponent<MeshCollider>());
            }

            GetChildRecursive(child.gameObject);
        }
    }
    #endregion

    #region ToggleColliders
    public void ToggleColliders()
    {
        if (meshColliders[0].enabled == true)
        {
            for (int i = 0; i < meshColliders.Count; i++)
            {
                meshColliders[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < meshColliders.Count; i++)
            {
                meshColliders[i].enabled = true;
            }
        }
    }
    #endregion

    #region SpawnEggSack
    public void SpawnEggSack()
    {
        RaycastHit hit;
        bool rotateDown = true;
        while (rotateDown)
        {
            if (Physics.Raycast(spinneret.position, -spinneret.forward, out hit, maxSpawnRange))
            {
                rotateDown = false;

                for (int i = 0; i < eggSacks.Count; i++)
                {
                    if (!eggSacks[i].GetComponent<EggSack>().SpiderlingsDead())
                    {
                        eggSacks[i].transform.position = hit.point + (transform.forward * 5);
                        eggSacks[i].SetActive(true);
                        i = eggSacks.Count;
                    }
                }

            }
            else
            {
                Debug.Log("Hi");
                spinneret.transform.Rotate(transform.right, 5);
            }
        }
    }
    #endregion

    float rotationSpeed = 5;
    private IEnumerator Attack()
    {
        bool rotated = false;
        bool rotating = true;

        while(rotating)
        {
            Vector3 newDirection = playerTransform.position - transform.position;
            newDirection.y = transform.forward.y;
            float angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

            if (angle > 5)
            {
                rotated = true;
                transform.Rotate(transform.up, -rotationSpeed);
            }
            if (angle < -5)
            {
                rotated = true;
                transform.Rotate(transform.up, rotationSpeed);
            }
            if (!rotated)
            {
                rotating = false;
            }

            yield return new WaitForSecondsRealtime(1/60);
        }
        animController.TriggerAttack();
    }

    #region Trigger enter and exit
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.layer == 15 && (other.transform.position - transform.position).magnitude > 5)
        {
            seesPlayer = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.layer == 15 && (other.transform.position - transform.position).magnitude > 5)
        {
            seesPlayer = false;
            playerTransform = null;
        }
    }
    #endregion

    #region Ikillable interface
    public IEnumerator CheckHit(bool x)
    {
        StartCoroutine(TakeDamage());
        yield return 0;
    }

    public IEnumerator TakeDamage()
    {
        hitPoints--;
        Debug.Log("SpiderMother hit");

        if (hitPoints < 1)
        {
            StartCoroutine(Die());
        }
        yield return 0;
    }

    public IEnumerator Die()
    {
        Debug.Log("SpiderMother died");
        gameObject.SetActive(false);
        yield return 0;
    }
    #endregion
}

public enum SpiderState
{
    Hiding,
    Reveal,
    Burry,
    Wonder,
    ChasePlayer,
    Attack,
    Vulnerable
}
