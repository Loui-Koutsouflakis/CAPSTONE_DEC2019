using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spiderlings : MonoBehaviour, IKillable
{

    public GameObject spineret;
    public SpiderlingAnimController spiderlingAnimController;
    public Material toonShader;
    public SpiderlingState currentState;
    public int hitPoints = 1;
    public float fireRange = 4f;

    [SerializeField]
    private Transform player;
    private LineRenderer lineRend;
    private NavMeshAgent navmeshAgent;
    private Vector3 destination;
    private Vector3 startPos;
    [SerializeField]
    private bool stuckPlayer = false;
    private Material useMaterial;
    private Material mat;
    [SerializeField]
    private Transform motherSpider;
    private float chasePlayerSpeed;
    private float motherSpiderSpeed;

    #region Awake, update, on enable, on disable
    private void Awake()
    {
        useMaterial = new Material(toonShader);
        lineRend = GetComponent<LineRenderer>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        chasePlayerSpeed = navmeshAgent.speed;

        startPos = transform.position;
        GetChildRecursive(gameObject);
    }

    private void GetChildRecursive(GameObject obj)
    {
        if (null == obj)
            return;

        mat = useMaterial;
        mat.SetFloat("_Strength", 0.0f);

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;

            if (child.gameObject.tag == "SpiderBody")
            {
                child.gameObject.GetComponent<Renderer>().material = mat;
            }


            GetChildRecursive(child.gameObject);
        }


    }

    private void OnEnable()
    {
        currentState = SpiderlingState.Chaseplayer;
        startPos = transform.position;
        if (player != null) destination = player.position;

        SetState(SpiderlingState.Chaseplayer);

        if (motherSpider != null) motherSpiderSpeed = motherSpider.gameObject.GetComponent<NavMeshAgent>().speed;
        else motherSpiderSpeed = 3.5f;
    }

    private void OnDisable()
    {
        if(mat != null) mat.SetFloat("_Strength", 0);

        SetLineRend(spineret.transform.position);
        transform.position = startPos;
    }

    NavMeshPath path = new NavMeshPath();


    void Update()
    {
        if (navmeshAgent.enabled && player != null)
        {
            path = new NavMeshPath();
            navmeshAgent.CalculatePath(player.position, path);
            Debug.Log(path.status + " Spiderling update");
        }
        if (motherSpider != null)
        {
            if (currentState == SpiderlingState.Chaseplayer) destination = player.position;
            else if (currentState == SpiderlingState.FollowSpiderMother && motherSpider != null) destination = motherSpider.position - (motherSpider.forward * 5);
        }
        else
        {
            if (stuckPlayer) destination = player.position;
            //if (path.status == NavMeshPathStatus.PathComplete) SetState(SpiderlingState.Chaseplayer);

            if (currentState == SpiderlingState.Chaseplayer && path.status == NavMeshPathStatus.PathComplete) destination = player.position;

            if (path.status != NavMeshPathStatus.PathComplete && (destination - transform.position).magnitude < 1) SetState(SpiderlingState.Wonder);
            
        }

        if (navmeshAgent.isPathStale) navmeshAgent.ResetPath();

        if ((destination - transform.position).magnitude <= fireRange / 2 && !stuckPlayer && currentState != SpiderlingState.Die && currentState != SpiderlingState.FollowSpiderMother)
        {

            stuckPlayer = true;
            currentState = SpiderlingState.StuckPlayer;
            player.gameObject.GetComponent<PlayerClass>().IncreaseWebs();
            //Debug.Log(player.gameObject.GetComponent<PlayerClass>().GetAttachedWebs() + " Increase");
       
        }

        if ((destination - transform.position).magnitude > fireRange && stuckPlayer && currentState != SpiderlingState.Die || currentState == SpiderlingState.FollowSpiderMother)
        {
            SetLineRend(spineret.transform.position);
            if (motherSpider == null) currentState = SpiderlingState.Chaseplayer;
            else SetState(SpiderlingState.Chaseplayer);

            stuckPlayer = false;
            player.gameObject.GetComponent<PlayerClass>().DecreaseWebs();
            //Debug.Log(player.gameObject.GetComponent<PlayerClass>().GetAttachedWebs() + " Decrease");
            GetComponent<NavMeshObstacle>().enabled = false;
            GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;

            navmeshAgent.enabled = true;

        }

        if (!stuckPlayer && currentState != SpiderlingState.Die)
        {
            spiderlingAnimController.TrapPlayer(false);

            navmeshAgent.SetDestination(destination);

        }
        else if (stuckPlayer && currentState != SpiderlingState.Die)
        {
            navmeshAgent.SetDestination(transform.position);
            navmeshAgent.enabled = false;
            GetComponent<NavMeshObstacle>().enabled = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            spiderlingAnimController.TrapPlayer(true);
            SetLineRend(player.position);
        }
        
        if (currentState == SpiderlingState.Die)
        {
            SetLineRend(spineret.transform.position);

            navmeshAgent.enabled = false;

        }
    }

    #endregion

    private void FindNextWonderDestination()
    {

    }

    public void SetSpiderTransform(Transform spider)
    {
        motherSpider = spider;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }



    public void SetState(SpiderlingState state)
    {
        switch(state)
        {
            case SpiderlingState.Chaseplayer:
                navmeshAgent.speed = chasePlayerSpeed;
                currentState = SpiderlingState.Chaseplayer;
                break;
            case SpiderlingState.FollowSpiderMother:
                navmeshAgent.speed = motherSpiderSpeed;
                currentState = SpiderlingState.FollowSpiderMother;

                break;
            case SpiderlingState.Wonder:
                destination = new Vector3(
                    Random.Range(startPos.x - 40, startPos.x + 40),
                    transform.position.y,
                    Random.Range(startPos.z - 40, startPos.z + 40));
                NavMeshPath path = new NavMeshPath();
                navmeshAgent.CalculatePath(destination, path);

                while (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                {
                    destination = new Vector3(
                        Random.Range(startPos.x - 20, startPos.x + 20),
                        transform.position.y,
                        Random.Range(startPos.z - 20, startPos.z + 20));

                    navmeshAgent.CalculatePath(destination, path);
                }

                navmeshAgent.SetDestination(destination);

                break;
        }

    }

    private void SetLineRend(Vector3 end)
    {
        lineRend.SetPosition(0, spineret.transform.position);
        lineRend.SetPosition(1, end);
    }

    


    #region Ikillable impementation
    public IEnumerator CheckHit(bool x)
    {
        //Debug.Log(x + " Why it no work");
        //Debug.Log("this is working");
        if (currentState != SpiderlingState.Die)
        {
            if (x) StartCoroutine(Die());
            else StartCoroutine(TakeDamage());
        }
        yield return 0;
    }

    public IEnumerator TakeDamage()
    {
        hitPoints--;
        //Debug.Log("Spiderling hit");

        if (hitPoints < 1)
        {
            StartCoroutine(Die());

        }


        yield return 0;
    }

    public IEnumerator Die()
    {
        currentState = SpiderlingState.Die;
        if(stuckPlayer) player.gameObject.GetComponent<PlayerClass>().DecreaseWebs();
        //Debug.Log(player.gameObject.GetComponent<PlayerClass>().GetAttachedWebs() + " Decrease in die");
        spiderlingAnimController.Death();
        //Debug.Log("Spiderling died");
        yield return 0;
    }

    public IEnumerator DeathDeactivate()
    {
        for (float i = 0; i < 0.8f; i += 0.05f)
        {

            mat.SetFloat("_Strength", i);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        gameObject.SetActive(false);

    }
    #endregion
}

public enum SpiderlingState
{
    Chaseplayer,
    Wonder,
    FollowSpiderMother,
    StuckPlayer,
    Die
}
