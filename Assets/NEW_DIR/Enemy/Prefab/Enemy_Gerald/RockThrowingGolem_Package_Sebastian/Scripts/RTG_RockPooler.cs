using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTG_RockPooler : MonoBehaviour
{
    public static RTG_RockPooler SharedInstance;

    public List<GameObject> pooledRocks;
    public GameObject RocksToPool;
    public int AmountToPool;
    public bool shouldExpand = true;

    

    // Start is called before the first frame update
    void Awake()
    {
        SharedInstance = this;
        pooledRocks = new List<GameObject>();
        for(int i = 0; i < AmountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(RocksToPool);
            obj.SetActive(false);
            pooledRocks.Add(obj);
        }
    }

    public GameObject GetPooledRock()
    {
        for (int i = 0; i < pooledRocks.Count; i++)
        {
            if (!pooledRocks[i].activeInHierarchy)
            {
                return pooledRocks[i];
            }
        }
        if (shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(RocksToPool);
            obj.SetActive(false);
            pooledRocks.Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
