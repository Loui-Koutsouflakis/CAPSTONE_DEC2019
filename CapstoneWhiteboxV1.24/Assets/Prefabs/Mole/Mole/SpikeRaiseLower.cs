using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
[AddComponentMenu("Lilly's Scripts")] 

public class SpikeRaiseLower : MonoBehaviour
{
    public Transform SpawnLocation;
    ////a
    [Range(1.0f, 100.0f)]
    public float SpikePeak;

    ////h
    [Range(-10.0f, 100.0f)]
    public float SpikeStart;

    //b
    [Range(1.0f, 100.0f)]
    public float SpikeRaiseFrequency;

    ////k
    [Range(-100.0f, 100.0f)]
    public float SpikeWaveHieght;

    [Range(0.5f, 100.0f)] // also used as (k * 2)
    public float SpikeMoveSpeed;

    [Range(0.0f, 100.0f)]
    public float SpikeTimer;

    [Range(1.0f, 100.0f)]
    public float forwardSpeed;

    //private float SpikePeak = 1.5f; // a
    //private float SpikeStart = 1;// h
    //private float SpikeWaveHieght = -1.0f; // k
    private float forwardMove;
    private Vector3 SpikeNewPosition;

    // Use this for initialization
    void Start ()
    {
        forwardMove = 0;
        SpikeNewPosition = gameObject.transform.forward;

        SpikeNewPosition.x = SpikeNewPosition.x * (forwardSpeed / 100);
        SpikeNewPosition.z = SpikeNewPosition.z * (forwardSpeed / 100);
    }

    // Update is called once per frame
    void Update ()
    {
        SpikeWave();
        SpikeTimer -= Time.deltaTime;

        if(SpikeTimer <= 0)
        {
            Destroy(gameObject);
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
   
        //===================================WorkingCircularSineWave===================
    //void SpikeWave()
    //{
    //    SpikeNewPosition.z = SpikeNewPosition.z * 1.002f;
    //    SpikeNewPosition.x = SpikeNewPosition.x * 1.002f;
    //    forwardMove += Time.deltaTime * SpikeMoveSpeed;
    //    SpikeNewPosition.y = SpikePeak * Mathf.Sin((forwardMove - SpikeStart) / SpikeRaiseFrequency * 10) + SpikeWaveHieght;
    //    gameObject.transform.position = SpikeNewPosition;
    //}
        //=================================Keep============================================


    //=========================================Original===============================
    //void SpikeWave()
    //{
    //    //y = a sin((x - h) / b) + k --> sinwave equation
    //    SpikeNewPosition.Set(gameObject.transform.position.x, SpikePeak * Mathf.Sin(
    //        (gameObject.transform.position.z - SpikeStart)
    //            / /*SpikeRaiseFrequency*/(SpikeMoveSpeed * 2) * 10) + SpikeWaveHieght,
    //                gameObject.transform.position.z + (SpikeMoveSpeed * Time.deltaTime));
    //    Debug.Log("Tweet" + SpikeNewPosition);
    //    gameObject.transform.position = SpikeNewPosition;
    //}
    //=========================================Keep====================================
}
