using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spiderlings : MonoBehaviour, IKillable
{
    [Range(1, 5)]
    public int rotationSpeed = 4;
    public int speed = 3;
    public float fireRange = 4f;
    public GameObject spineret;
    public SpiderlingAnimController spiderlingAnimController;
    //public FlockingController flockController;
    public int hitPoints = 1;
    private bool stuckPlayer = false;
    private float angle;
    private Enemy enemyScript;

    private Transform player;
    private Vector3 destination;
    private Vector3 newDirection;

    //The modified direction for the boid.
    private Vector3 targetDirection;
    //The Boid's current direction.
    private Vector3 direction;
    public bool kill = false;
    Vector3 debugCurrentAxis;
    float debugAngleToTurn;
    public GameObject posVisual;
    public GameObject posVisual2;
    private List<Vector3> obstaclePos = new List<Vector3>();
    public Vector3 Direction { get { return direction; } }
    Vector3 startPos;


    public void SetPlayer(Transform player)
    {
        this.player = player; 
    }
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
    }


    private void OnEnable()
    {
        startPos = transform.position;
        if(player != null) destination = player.position;        
    }

    private void OnDisable()
    {
        SetLineRend(spineret.transform.position);
        transform.position = startPos;
    }


    private void SetLineRend(Vector3 end)
    {
        GetComponent<LineRenderer>().SetPosition(0, spineret.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, end);
    }

    public void Step()
    {
        transform.position += (transform.forward * (3 * Time.deltaTime));
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }



    private void TestForCollisions()
    {
        obstaclePos = new List<Vector3>();
        int test = 0;
        int pos = 0;
        Vector3 lineStart = new Vector3();
        Vector3 lineEnd = new Vector3();

        while (test < 8)
        {
            DebugFindNewAxis(pos);
            lineStart = (debugCurrentAxis * 1.1f) + transform.position;
            posVisual.transform.position = lineStart;
            lineEnd = (debugCurrentAxis * 2.5f) + transform.position;
            posVisual2.transform.position = lineEnd;
            RaycastHit hit;
            if(Physics.Linecast(lineStart, lineEnd, out hit))
            {
                obstaclePos.Add(hit.transform.position);
            }

            pos++;
            if (pos > 7) pos = 0;
            test++;
        }
        Debug.Log("Its done");
    }

    void Update()
    {
        if (kill)
        {
            kill = false;
            StartCoroutine(Die());
        }

        if (player != null) destination = player.position;
        if (GetComponent<NavMeshAgent>().isPathStale) GetComponent<NavMeshAgent>().ResetPath();
        if ((destination - transform.position).magnitude <= fireRange / 2 && !stuckPlayer)
        {

            stuckPlayer = true;

        }

        if ((destination - transform.position).magnitude > fireRange)
        {
            SetLineRend(spineret.transform.position);
            stuckPlayer = false;
            GetComponent<NavMeshObstacle>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = true;

        }

        if (!stuckPlayer /*&& GetComponent<NavMeshAgent>().enabled*/)
        {
            GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            spiderlingAnimController.TrapPlayer(false);

        }
        else if (stuckPlayer/*GetComponent<NavMeshAgent>().enabled*/)
        {
            GetComponent<NavMeshAgent>().SetDestination(transform.position);
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<NavMeshObstacle>().enabled = true;


            spiderlingAnimController.TrapPlayer(true);
            SetLineRend(player.position);
        }
    }

    private void FixedUpdate()
    {
        if (stuckPlayer)
        {
            newDirection = destination - transform.position;
            newDirection.y = transform.forward.y;
            angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

            if (angle > 5) transform.Rotate(transform.up, -rotationSpeed);
            if (angle < -5) transform.Rotate(transform.up, rotationSpeed);
        }
    }



    public void DebugFindNewAxis(int rayPos)
    {
        debugCurrentAxis = transform.right;
        debugAngleToTurn = (45 * rayPos);

        float x = debugCurrentAxis.x;

        float z = debugCurrentAxis.z;

        debugCurrentAxis.x = x * Mathf.Cos(debugAngleToTurn * Mathf.Deg2Rad) + z * Mathf.Sin(debugAngleToTurn * Mathf.Deg2Rad);
        debugCurrentAxis.z = -x * Mathf.Sin(debugAngleToTurn * Mathf.Deg2Rad) + z * Mathf.Cos(debugAngleToTurn * Mathf.Deg2Rad);

        x = debugCurrentAxis.x;

        z = debugCurrentAxis.z;

        debugCurrentAxis.x = x * Mathf.Cos(90 * Mathf.Deg2Rad) + z * Mathf.Sin(90 * Mathf.Deg2Rad);
        debugCurrentAxis.z = -x * Mathf.Sin(90 * Mathf.Deg2Rad) + z * Mathf.Cos(90 * Mathf.Deg2Rad);

    }

    public IEnumerator CheckHit(bool x)
    {
        Debug.Log("this is working");
        if(x) StartCoroutine(Die());
        else StartCoroutine(TakeDamage());
        yield return 0;
    }

    public IEnumerator TakeDamage()
    {
        hitPoints --;
        Debug.Log("Spiderling hit");

        if (hitPoints < 1)
        {
            StartCoroutine(Die());

        }


        yield return 0;
    }

    public IEnumerator Die()
    {
        Debug.Log("Spiderling died");
        gameObject.SetActive(false);
        transform.position = startPos;
        yield return 0;
    }
}
