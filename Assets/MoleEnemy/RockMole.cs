using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMole : MonoBehaviour
{
    [SerializeField]
    private Transform[] waypoints;

    [SerializeField]
    private Transform playerTf;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    [Range(1, 4)]
    private int moleType;

    [SerializeField]
    private float startDelay = 1f;

    [SerializeField]
    private ParticleSystem[] groundPs;

    [SerializeField]
    private bool randomizedWaypoints;

    [SerializeField]
    private bool randomizedDigTime;

    [SerializeField]
    private bool randomizedSurfaceTime;

    [SerializeField]
    private float randomRange = 5f;

    private bool alive;

    private float surfaceTime;
    private float digTime;
    //private float shootTime;

    const string surfaceAnimName = "Surface";
    const string digAnimName = "Dig";
    //const string shootAnimName = "Shoot";
    const string dieAnimName = "Die";

    private Coroutine moleSequence;

    private Vector3 lookDirection;
    private Vector3 randomWaypoint;

    private void Start()
    {
        alive = true;

        if (moleType < 2)
        {
            surfaceTime = 4f;
            digTime = 1.6f;
            moleType = 1;
        }
        else if (moleType == 2)
        {
            surfaceTime = 3.5f;
            digTime = 1.4f;
        }
        else if (moleType == 3)
        {
            surfaceTime = 3f;
            digTime = 1.2f;
        }
        else if (moleType > 3)
        {
            surfaceTime = 2.5f;
            digTime = 1f;
            //shootTime = 2f;
            moleType = 4;
        }

        StartCoroutine(StartDelay());
    }

    //This block is just for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(CheckHit(false));
        }
    }

    public IEnumerator MoleSequence()
    {
        for (int i = 1; i <= moleType; i++)
        {
            yield return new WaitForSeconds(0.2f);

            anim.SetTrigger(digAnimName);

            if (randomizedDigTime)
            {
                yield return new WaitForSeconds(Random.Range(1f, 5f));
            }
            else
            {
                yield return new WaitForSeconds(digTime);
            }

            if (randomizedWaypoints)
            {
                randomWaypoint.x = Random.Range(-randomRange, randomRange);
                transform.parent.localPosition = randomWaypoint;
            }
            else
            {
                transform.parent.position = waypoints[i - 1].position;
            }

            lookDirection = playerTf.position - transform.parent.position;
            lookDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(lookDirection);
            anim.SetTrigger(surfaceAnimName);

            foreach (ParticleSystem ps in groundPs)
            {
                ps.Play();
            }

            if (randomizedSurfaceTime)
            {
                yield return new WaitForSeconds(Random.Range(1f, 5f));
            }
            else
            {
                yield return new WaitForSeconds(surfaceTime);
            }

            //if(moleType == 4)
            //{
            //    anim.SetTrigger(shootAnimName);
            //    yield return new WaitForSeconds(shootTime);
            //}
        }

        if (alive)
        {
            moleSequence = StartCoroutine(MoleSequence());
        }
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        moleSequence = StartCoroutine(MoleSequence());
    }

    public IEnumerator CheckHit(bool isGroundPound)
    {
        yield return 0f;

        StartCoroutine(Die());
    }

    public IEnumerator TakeDamage()
    {
        yield return 0f;
    }

    public IEnumerator Die()
    {
        alive = false;
        StopCoroutine(moleSequence);
        anim.SetTrigger(dieAnimName);

        yield return new WaitForSeconds(0.8f);

        Destroy(gameObject, 0.8f);
    }
}
