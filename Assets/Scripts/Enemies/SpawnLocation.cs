using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public EnemyManager pool;
    public EnemyType type;
    public bool spawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Spawn()
    {
        pool.SpawnEnemy(transform.position, type);
        spawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn) Spawn();   
    }
}
