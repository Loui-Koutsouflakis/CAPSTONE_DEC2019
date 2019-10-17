// Written By Benjamin Young October 16/2019.  Last Updated October 16/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    //The variables that will hold the collectible info
    private int smallShards;
    private int bigShards;
    private int crystals;

    //Bool to decide which load scene function to call
    public bool loadingFromContinue;

    //Refrence to the player
    public GameObject player;

    //An int to figure out which file/set of playerprefs are being used
    public int currentFile = 1;

    //Varible to hold the transform of the current checkpoint
    public Transform currentCheckpoint;

    private Transform beginning;

    public bool loading;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

 

    private void Update()
    {  
        if(loading)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            loading = false;
        }
        

        inputSaveTesting();
    }


    public void Save()
    {
        if(currentFile == 1)
        {
            //Gets the player position and stores it
            Vector3 checkpointPosition = currentCheckpoint.position;
            PlayerPrefs.SetFloat("CheckpointPositionX_1", checkpointPosition.x);
            PlayerPrefs.SetFloat("CheckpointPositionY_1", checkpointPosition.y);
            PlayerPrefs.SetFloat("CheckpointPositionZ_1", checkpointPosition.z);

            //Handles recording the collectibles;
            PlayerPrefs.SetInt("SmallShards_1", smallShards);
            PlayerPrefs.SetInt("BigShards_1", bigShards);
            PlayerPrefs.SetInt("Crystals_1", crystals);

            //Handles Saving the scene;
            PlayerPrefs.SetInt("CurrentScene_1", SceneManager.GetActiveScene().buildIndex);

            //Lets Me know if there is data
            PlayerPrefs.SetInt("HasData_1", 1);

            Debug.Log("You Saved");
        }

        if (currentFile == 2)
        {
            //Gets the player position and stores it
            Vector3 checkpointPosition = currentCheckpoint.position;
            PlayerPrefs.SetFloat("CheckpointPositionX_2", checkpointPosition.x);
            PlayerPrefs.SetFloat("CheckpointPositionY_2", checkpointPosition.y);
            PlayerPrefs.SetFloat("CheckpointPositionZ_2", checkpointPosition.z);

            //Handles recording the collectibles;
            PlayerPrefs.SetInt("SmallShards_2", smallShards);
            PlayerPrefs.SetInt("BigShards_2", bigShards);
            PlayerPrefs.SetInt("Crystals_2", crystals);

            //Handles Saving the scene;
            PlayerPrefs.SetInt("CurrentScene_2", SceneManager.GetActiveScene().buildIndex);

            //Lets Me know if there is data
            PlayerPrefs.SetInt("HasData_2", 1);

            Debug.Log("You Saved");
        }

        if (currentFile == 3)
        {
            //Gets the player position and stores it
            Vector3 checkpointPosition = currentCheckpoint.position;
            PlayerPrefs.SetFloat("CheckpointPositionX_3", checkpointPosition.x);
            PlayerPrefs.SetFloat("CheckpointPositionY_3", checkpointPosition.y);
            PlayerPrefs.SetFloat("CheckpointPositionZ_3", checkpointPosition.z);

            //Handles recording the collectibles;
            PlayerPrefs.SetInt("SmallShards_3", smallShards);
            PlayerPrefs.SetInt("BigShards_3", bigShards);
            PlayerPrefs.SetInt("Crystals_3", crystals);

            //Handles Saving the scene;
            PlayerPrefs.SetInt("CurrentScene_3", SceneManager.GetActiveScene().buildIndex);

            //Lets Me know if there is data
            PlayerPrefs.SetInt("HasData_3", 1);

            Debug.Log("You Saved");
        }
    }

    public void Load()
    {
        if (currentFile == 1)
        {

            if (PlayerPrefs.HasKey("CharacterPositionX_1"))
            {
                float checkpointPositionX = PlayerPrefs.GetFloat("CheckpointPositionX_1");
                float checkpointPositionY = PlayerPrefs.GetFloat("CheckpointPositionY_1");
                float checkpointPositionZ = PlayerPrefs.GetFloat("CheckpointPositionZ_1");
                player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, checkpointPositionZ);

                smallShards = PlayerPrefs.GetInt("SmallShards_1");
                bigShards = PlayerPrefs.GetInt("BigShards_1");
                crystals = PlayerPrefs.GetInt("Crystals_1");

                Debug.Log("You Loaded");
            }
        }

        if (currentFile == 2)
        {

            if (PlayerPrefs.HasKey("CharacterPositionX_2"))
            {
                float checkpointPositionX = PlayerPrefs.GetFloat("CheckpointPositionX_2");
                float checkpointPositionY = PlayerPrefs.GetFloat("CheckpointPositionY_2");
                float checkpointPositionZ = PlayerPrefs.GetFloat("CheckpointPositionZ_2");
                player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, checkpointPositionZ);

                smallShards = PlayerPrefs.GetInt("SmallShards_2");
                bigShards = PlayerPrefs.GetInt("BigShards_2");
                crystals = PlayerPrefs.GetInt("Crystals_2");

                Debug.Log("You Loaded");
            }
        }

        if (currentFile == 3)
        {

            if (PlayerPrefs.HasKey("CharacterPositionX_3"))
            {
                float checkpointPositionX = PlayerPrefs.GetFloat("CheckpointPositionX_3");
                float checkpointPositionY = PlayerPrefs.GetFloat("CheckpointPositionY_3");
                float checkpointPositionZ = PlayerPrefs.GetFloat("CheckpointPositionZ_3");
                player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, checkpointPositionZ);

                smallShards = PlayerPrefs.GetInt("SmallShards_3");
                bigShards = PlayerPrefs.GetInt("BigShards_3");
                crystals = PlayerPrefs.GetInt("Crystals_3");

                Debug.Log("You Loaded");
            }
        }

    }

    public void LoadSavedScene()
    {
        if(currentFile == 1)
        {
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene_1");
            SceneManager.LoadSceneAsync(savedSceneIndex);

        }
        else if (currentFile == 2)
        {
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene_2");
            SceneManager.LoadSceneAsync(savedSceneIndex);

        }
        else if (currentFile == 2)
        {
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene_2");
            SceneManager.LoadSceneAsync(savedSceneIndex);

        }

    }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if(!loadingFromContinue)
            {
                beginning = GameObject.FindGameObjectWithTag("Beginning").transform;
                currentCheckpoint = beginning;
                Save();
                Debug.Log("Continue Is Not Working");
            }
            else if(loadingFromContinue)
            {
                Load();
                loadingFromContinue = false;
                StartCoroutine(Delay(1f));
                Debug.Log("Continue Is Working");

            }

        }
    }


    void inputSaveTesting()
    {
        if (Input.GetKey(KeyCode.Alpha0))
        {
            Load();
        }

        if (Input.GetKey(KeyCode.Alpha9))
        {
            currentCheckpoint = player.transform;
            Save();
        }

        if (Input.GetKey(KeyCode.Alpha8))
        {
            ClearSavedFiles();
        }
    }


    public void ClearSavedFiles()
    {
        if(currentFile == 1)
        {
            //Clears All the Data in the playerPrefs 
          
            PlayerPrefs.SetFloat("CheckpointPositionX_1", 0);
            PlayerPrefs.SetFloat("CheckpointPositionY_1", 0);
            PlayerPrefs.SetFloat("CheckpointPositionZ_1", 0);

            PlayerPrefs.SetInt("SmallShards_1", 0);
            PlayerPrefs.SetInt("BigShards_1", 0);
            PlayerPrefs.SetInt("Crystals_1", 0);

            PlayerPrefs.SetInt("CurrentScene_1", 0);

            PlayerPrefs.SetInt("HasData_1", 0);
            Debug.Log("Cleared Data");
        }

        if (currentFile == 2)
        {
            //Clears All the Data in the playerPrefs 
            Vector3 playerPostion = player.gameObject.transform.position;
            PlayerPrefs.SetFloat("CheckpointPositionX_1", 0);
            PlayerPrefs.SetFloat("CheckpointPositionY_1", 0);
            PlayerPrefs.SetFloat("CheckpointPositionZ_1", 0);

            PlayerPrefs.SetInt("SmallShards_1", 0);
            PlayerPrefs.SetInt("BigShards_1", 0);
            PlayerPrefs.SetInt("Crystals_1", 0);

            PlayerPrefs.SetInt("CurrentScene_1", 0);

            PlayerPrefs.SetInt("HasData_1", 0);

        }

        if (currentFile == 3)
        {
            //Clears All the Data in the playerPrefs 
            Vector3 playerPostion = player.gameObject.transform.position;
            PlayerPrefs.SetFloat("CheckpointPositionX_1", 0);
            PlayerPrefs.SetFloat("CheckpointPositionY_1", 0);
            PlayerPrefs.SetFloat("CheckpointPositionZ_1", 0);

            PlayerPrefs.SetInt("SmallShards_1", 0);
            PlayerPrefs.SetInt("BigShards_1", 0);
            PlayerPrefs.SetInt("Crystals_1", 0);

            PlayerPrefs.SetInt("CurrentScene_1", 0);

            PlayerPrefs.SetInt("HasData_1", 0);

        }
    }


    public void increaseSmallShards(int amountOfShards)
    {
        smallShards += amountOfShards;
    }

    public void increaseBigShards(int amountOfShards)
    {
        bigShards += amountOfShards;
    }
    public void increaseCrystals(int amountOfCrystals)
    {
        smallShards += amountOfCrystals;
    }

    public void updateCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Save();
    }

    IEnumerator Delay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
