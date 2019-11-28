using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMole : MonoBehaviour, IKillable
{
    [SerializeField]
    private Transform[] waypoints;

    [SerializeField]
    private Transform playerTf;
    private PlayerClass player;
    [SerializeField]
    private Animator anim;

    [SerializeField] [Range(1,4)]
    private int moleType;

    [SerializeField]
    private ParticleSystem[] groundPs;

    [SerializeField]
    private Collider[] colliders;

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

    private void Start()
    {
        alive = true;
        playerTf = FindObjectOfType<PlayerClass>().transform;

        if(moleType < 2)
        { 
            surfaceTime = 3f;
            digTime = 1.6f;
            moleType = 1;
        }
        else if(moleType == 2)
        {
            surfaceTime = 2.5f;
            digTime = 1.4f;
        }
        else if(moleType == 3)
        {
            surfaceTime = 2f;
            digTime = 1.2f;
        }
        else if(moleType > 3)
        {
            surfaceTime = 1.5f;
            digTime = 1f;
            //shootTime = 2f;
            moleType = 4;
        }

        moleSequence = StartCoroutine(MoleSequence());
    }

    //This block is just for testing
    //private void Update()
    //{
    //    //if(Input.GetKeyDown(KeyCode.M))
    //    //{
    //    //    StartCoroutine(CheckHit(false));
    //    //}
    //}

    public IEnumerator MoleSequence()
    {
        for(int i = 1; i <= moleType; i++)
        {
            yield return new WaitForSeconds(0.2f);

            anim.SetTrigger(digAnimName);
            yield return new WaitForSeconds(digTime);

            transform.parent.position = waypoints[i - 1].position;
            lookDirection = playerTf.position - transform.parent.position;
            lookDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(lookDirection);
            anim.SetTrigger(surfaceAnimName);

            foreach(ParticleSystem ps in groundPs)
            {
                ps.Play();
            }

            yield return new WaitForSeconds(surfaceTime);

            //if(moleType == 4)
            //{
            //    anim.SetTrigger(shootAnimName);
            //    yield return new WaitForSeconds(shootTime);
            //}
        }

        if(alive)
        {
            moleSequence = StartCoroutine(MoleSequence());
        }
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

        foreach(Collider col in colliders)
        {
            col.enabled = false;
        }

        StopCoroutine(moleSequence);
        anim.SetTrigger(dieAnimName);

        yield return new WaitForSeconds(0.8f);

        Destroy(gameObject, 0.8f);
    }
}
