using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectPools : MonoBehaviour
{
    // Trigger Pool
    PFS_Trigger[] savePointsArray;// Progression Save Points Array
    PFS_Trigger[] cineTrigsArray;// Cinematics Trigger Array
    PFS_Trigger[] startingTriggerArray;// Sorting Array
    // Array Size Ints
    int cineTrigsArraySize = 0;// Used To Dynamically Create Array cineTrigsArray
    int savePointsArraySize = 0;// Used To Dynamically Create Array savePointsArray

    // Enemy Pool
    EnemyPlaceHolderScript[] enemiesAll;// Used For Sorting Enemy Types
    EnemyPlaceHolderScript[] enemyType1;// Squirrel Array
    EnemyPlaceHolderScript[] enemyType2;// Armadillo Array
    EnemyPlaceHolderScript[] enemyType3;// Spiderling Array
    EnemyPlaceHolderScript[] enemyType4;// Spider Array
    EnemyPlaceHolderScript[] enemyType5;// Tree Array
    // Array Size Ints
    int eType1Size = 0;// Used To Dynamically Create Squirrel Enemy Array
    int eType2Size = 0;// Used To Dynamically Create Armadillo Enemy Array
    int eType3Size = 0;// Used To Dynamically Create Spiderling Enemy Array
    int eType4Size = 0;// Used To Dynamically Create Spider Enemy Array
    int eType5size = 0;// Used To Dynamically Create Tree Enemy Array
    // Used for sorting of EnemyPlaceHolderScript objects
    int eType1Index = 0;
    int eType2Index = 0;
    int eType3Index = 0;
    int eType4Index = 0;
    int eType5Index = 0;


    void Awake()
    {
        GenerateObjectPools();
    }

    void GenerateObjectPools()
    {
        GenerateTriggerPools();
        GenerateEnemyPools();
    }

    void GenerateTriggerPools()
    {
        startingTriggerArray = GameObject.FindObjectsOfType<PFS_Trigger>();
        for (int i = 0; i < startingTriggerArray.Length; i++)
        {
            if (startingTriggerArray[i].triggerType == PFS_Trigger.PlayerTriggerTypes.Cinematics)
            {
                cineTrigsArraySize++;
            }
            else if (startingTriggerArray[i].triggerType == PFS_Trigger.PlayerTriggerTypes.LevelProgression)
            {
                savePointsArraySize++;
            }
        }
        savePointsArray = new PFS_Trigger[savePointsArraySize];
        cineTrigsArray = new PFS_Trigger[cineTrigsArraySize];
        for (int i = 0; i < startingTriggerArray.Length; i++)
        {
            if (startingTriggerArray[i].triggerType == PFS_Trigger.PlayerTriggerTypes.Cinematics)
            {
                cineTrigsArray[i] = startingTriggerArray[i];
            }
            else if (startingTriggerArray[i].triggerType == PFS_Trigger.PlayerTriggerTypes.LevelProgression)
            {
                savePointsArray[i] = startingTriggerArray[i];
            }
        }
    }

    void GenerateEnemyPools()
    {
        enemiesAll = GameObject.FindObjectsOfType<EnemyPlaceHolderScript>();
        for (int i = 0; i < enemiesAll.Length; i++)
        {
            if(enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Squirrle)
            {
                eType1Size++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Armadillo)
            {
                eType2Size++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Spiderling)
            {
                eType3Size++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Spider)
            {
                eType4Size++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Tree)
            {
                eType5size++;
            }
        }
        enemyType1 = new EnemyPlaceHolderScript[eType1Size];
        enemyType2 = new EnemyPlaceHolderScript[eType2Size];
        enemyType3 = new EnemyPlaceHolderScript[eType3Size];
        enemyType4 = new EnemyPlaceHolderScript[eType4Size];
        enemyType5 = new EnemyPlaceHolderScript[eType5size];
        for (int i = 0; i < enemiesAll.Length; i++)
        {
            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Squirrle)
            {
                enemyType1[eType1Index] = enemiesAll[i];
                eType1Index++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Armadillo)
            {
                enemyType2[eType2Index] = enemiesAll[i];
                eType2Index++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Spiderling)
            {
                enemyType3[eType3Index] = enemiesAll[i];
                eType3Index++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Spider)
            {
                enemyType4[eType4Index] = enemiesAll[i];
                eType4Index++;
            }

            if (enemiesAll[i].type == EnemyPlaceHolderScript.EnemyType.Tree)
            {
                enemyType5[eType5Index] = enemiesAll[i];
                eType5Index++;
            }
        }

    }
}
