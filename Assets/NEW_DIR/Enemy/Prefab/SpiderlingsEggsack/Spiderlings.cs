using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiderlings : MonoBehaviour, IKillable
{
    [Range(1, 5)]
    public int rotationSpeed = 4;
    public int speed = 3;
    public float fireRange = 4f;
    public GameObject spineret;
    public SpiderlingAnimController spiderlingAnimController;
    public int hitPoints = 1;
    private bool stuckPlayer = false;
    private float angle;
    private Enemy enemyScript;

    private Transform player;
    private Vector3 destination;
    private Vector3 newDirection;
    public PlayerController p_Reference;
    bool gotYa = false;
    public void SetPlayer(Transform player)
    {
        this.player = player; 
    }
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        p_Reference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        if(player != null) destination = player.position;        
    }

    private void OnDisable()
    {
        SetLineRend(spineret.transform.position);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stuckPlayer)
        {
            p_Reference.spiderWebs += 1;
            gotYa = false;
        }
    }

    private void FixedUpdate()
    {
        if (player != null) destination = player.position;
        if ((destination - transform.position).magnitude <= fireRange / 2)
        {

            stuckPlayer = true;
            gotYa = true;
        }
        
        if((destination - transform.position).magnitude > fireRange)
        {
            SetLineRend(spineret.transform.position);
            stuckPlayer = false;
        }

        if (!stuckPlayer)
        {
            spiderlingAnimController.TrapPlayer(false);

            transform.position += (transform.forward * (speed * Time.deltaTime));

            newDirection = destination - transform.position;
            newDirection.y = transform.forward.y;
            angle = Vector3.SignedAngle(newDirection, transform.forward, transform.up);

            if (angle > 5) transform.Rotate(transform.up, -rotationSpeed);
            if (angle < -5) transform.Rotate(transform.up, rotationSpeed);
        }
        else
        {
            spiderlingAnimController.TrapPlayer(true);
            SetLineRend(player.position);
            
        }
    }

    public IEnumerator CheckHit(bool x)
    {
        yield return 0;
    }

    public IEnumerator TakeDamage()
    {

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
        yield return 0;
    }
}
