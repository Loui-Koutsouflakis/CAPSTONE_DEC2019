// Created by Loui Koutsouflakis:   2019-07-19

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Depending on how much the enemies differ, 
    // we may use a pool for each enemy
    public Enemy[] enemyPool;
    
    public int enemyIndex;

    public void SpawnEnemy(Vector3 spawnPoint, EnemyType type)
    {

        enemyPool[enemyIndex].gameObject.transform.position = spawnPoint;
        enemyPool[enemyIndex].Spawn(type);
        HandleIndex();
    }

    public void SpawnEnemy(Vector3 spawnPoint, Vector3 spawnEulers, EnemyType type)
    {
        enemyPool[enemyIndex].gameObject.transform.position = spawnPoint;
        enemyPool[enemyIndex].gameObject.transform.rotation = Quaternion.Euler(spawnEulers);
        enemyPool[enemyIndex].Spawn(type);
        HandleIndex();
    }

    public void SpawnEnemy(EnemyData enemyData)
    {
        enemyPool[enemyIndex].gameObject.transform.position = enemyData.spawnPoint;
        enemyPool[enemyIndex].gameObject.transform.rotation = Quaternion.Euler(enemyData.spawnEulers);
        enemyPool[enemyIndex].Spawn(enemyData.type);
        HandleIndex();
    }

    public void HandleIndex()
    {
        if (enemyIndex >= enemyPool.Length - 1) enemyIndex = 0;
        else enemyIndex++;
    }
}

// Put enemy types here
public enum EnemyType
{
    Shmenemy,
    Todds,
    Tod
}

// You can use a serializable struct, accessible in editor 
// If you decide to use this, cache EnemyData arrays in the Level Manager for each level
// and enter their data there in the inspector
[System.Serializable]
public struct EnemyData
{
    public string name;
    public EnemyType type;
    public Vector3 spawnPoint;
    public Vector3 spawnEulers;
}

