using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
[AddComponentMenu("Lilly's Scripts")] 

public class SpikeRaiseLower : MonoBehaviour
{

    public int damage;

    #region slider version of variables
    ////a
    //[Range(1.0f, 100.0f)]
    //public float SpikePeak;

    ////h
    //[Range(-10.0f, 100.0f)]
    //public float SpikeStart;

    ////b
    //[Range(1.0f, 100.0f)]
    //public float SpikeRaiseFrequency;

    ////k
    //[Range(-100.0f, 100.0f)]
    //public float SpikeWaveHieght;

    //[Range(0.5f, 100.0f)] // also used as (k * 2)
    //public float SpikeMoveSpeed;

    //[Range(0.0f, 100.0f)]
    //public float SpikeTimer;

    //[Range(1.0f, 100.0f)]
    //public float forwardSpeed;
    #endregion

    #region Private variables
    private float SpikePeak = 1.5f; // a
    private float SpikeStart = 1;// h
    private float SpikeRaiseFrequency = 53.9f; //b
    private float SpikeWaveHieght = -1.0f; // k
    private float SpikeTimer = 3;
    private float SpikeMoveSpeed = 27.4f;
    private float forwardSpeed = 5;
    private float forwardMove;
    private Vector3 SpikeNewPosition;
    private Vector3 startLocation;
    private float SpikeTimerStart;
    #endregion

    void Start ()
    {
        SpikeTimerStart = SpikeTimer;
        startLocation = gameObject.transform.position;
        forwardMove = 0;
        SpikeNewPosition = gameObject.transform.forward;
        SpikeNewPosition.x = SpikeNewPosition.x * (forwardSpeed / 100);
        SpikeNewPosition.z = SpikeNewPosition.z * (forwardSpeed / 100);
    }

    void OnEnable()
    {
        forwardMove = 0;
        SpikeNewPosition = gameObject.transform.forward;
        SpikeNewPosition.x = SpikeNewPosition.x * (forwardSpeed / 100);
        SpikeNewPosition.z = SpikeNewPosition.z * (forwardSpeed / 100);
    }

    void Update ()
    {
        SpikeWave();
        SpikeTimer -= Time.deltaTime;

        if(SpikeTimer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HitBox>().TakeDamage(damage);
        }
    }

    void SpikeWave()
    {
        forwardMove += Time.deltaTime * SpikeMoveSpeed;
        SpikeNewPosition.y = SpikePeak * Mathf.Sin((forwardMove - SpikeStart) / SpikeRaiseFrequency * 10) + SpikeWaveHieght;
  
        Vector3 temp = gameObject.transform.position;
        temp.y = 0;
        gameObject.transform.position = temp + SpikeNewPosition;
    }

    void OnDisable()
    {
        gameObject.transform.position = startLocation;
        SpikeTimer = SpikeTimerStart;
    }
}
