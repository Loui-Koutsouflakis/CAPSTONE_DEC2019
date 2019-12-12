// Written By Benjamin Young October 16/2019.  Last Updated December 9/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    private static SaveGameManager _instance;

    //The variables that will hold the collectible info
    private int smallShards;
    private int bigShards;
    private int crystals;

    //Bool to decide which load scene function to call
    public bool loadingFromContinue;

    //Refrence to the player
    private PlayerClass player;

    //An int to figure out which file/set of playerprefs are being used
    public int currentFile = 1;

    //Varible to hold the transform of the current checkpoint
    public Transform currentCheckpoint;

    //Varible to hold the beginning transform so that the current checkpoint can be set to it when the player starts a new game;
    private Transform beginning;

    //bool to prevent OnSceneLoaded function from being called multiple times
    public bool loading;


    //Shard Saving Varibles
    public bool shardCollected;
    public List<string> collectedShards = new List<string>();

    //Monolith Saving Varibles
    public List<string> triggeredMonoliths = new List<string>();

    //Settings Varibles
    [SerializeField]
    int cameraInverted = 0;
    float cameraSensitivity;
    private PlayerCamera mainCamera;
    int qualitySetting;
    float cameraSmoothing;
    float sliderButtonPostionX;

    float masterSoundVolume;
    float MusicSoundVolume;
    float dialogueSoundVolume;
    AudioHandler audioHandler;
    float masterVolumePercent;

    float musicSliderPositionX;
    float masterSliderPositionX;
    float dialogueSliderPositionX;

    bool startingUp = true;


    private void Awake()
    {
        
        

        if (startingUp == true)
        {
            LoadCollectedShards();
            //LoadTriggeredMonoliths();
            startingUp = false;
        }
        //for (int i = 0; i < PlayerPrefs.GetInt("NumberOfCollectedShards"); i++)
        //{
        //    Debug.Log(PlayerPrefs.GetString("CollectedShardsIDs Main Menu" + i));
        //}

        //Basic Singleton set up to prevent muliples of this script
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        //this script will need to run throughout the game
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.GetInt("CameraInverted") != 0 && PlayerPrefs.GetInt("CameraInverted") != 1)
        {
            cameraInverted = 0;
        }
        else
        {
            cameraInverted = PlayerPrefs.GetInt("CameraInverted");
        }

        if (PlayerPrefs.GetFloat("CameraSmoothing") > -0.1)
        {
            cameraSmoothing = PlayerPrefs.GetFloat("CameraSmoothing");
        }
        else
        {
            cameraSmoothing = 0.202f;
        }

        if (PlayerPrefs.GetFloat("CameraSensitivity") >= 1 && PlayerPrefs.GetFloat("CameraSensitivity") <= 10)
        {
            cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity");

        }
        else
        {
            cameraSensitivity = 1;
        }

        if (PlayerPrefs.GetInt("QualitySetting") > 0)
        {
            qualitySetting = PlayerPrefs.GetInt("QualitySetting");
        }
        else
        {
            qualitySetting = 5;
        }


        //if(PlayerPrefs.GetFloat("MasterSoundVolume") >= 0 && PlayerPrefs.GetFloat("MasterSoundVolume") <= 1)
        //{
        //    masterSoundVolume = PlayerPrefs.GetFloat("MasterSoundVolume");
        //}
        //else
        //{
        //    masterSoundVolume = 0.5f;
        //}


        if (PlayerPrefs.HasKey("MasterSoundVolume"))
        {
            masterSoundVolume = PlayerPrefs.GetFloat("MasterSoundVolume");
        }
        else
        {
            masterSoundVolume = 0f;
        }

        if (PlayerPrefs.HasKey("MusicSoundVolume"))
        {
            MusicSoundVolume = PlayerPrefs.GetFloat("MusicSoundVolume");
        }
        else
        {
            MusicSoundVolume = 0f;
        }

        if (PlayerPrefs.HasKey("DialogueSoundVolume"))
        {
            dialogueSoundVolume = PlayerPrefs.GetFloat("DialogueSoundVolume");
        }
        else
        {
            dialogueSoundVolume = 0f;
        }

        if (PlayerPrefs.GetFloat("MasterVolumePercent") >= 0 && PlayerPrefs.GetFloat("MasterVolumePercent") <= 1)
        {
            masterVolumePercent = PlayerPrefs.GetFloat("MasterVolumePercent");
        }
        else
        {
            masterVolumePercent = 1f;
        }

        SaveSettings();
        LoadSettings();
    }



    private void Update()
    {

        if (loading)
        {
            //Calls OnSceneLoaded when the scene switches 
            SceneManager.sceneLoaded += OnSceneLoaded;
            loading = false;
        }


        inputSaveTesting();
    }

    //Saves data to playerPrefs
    public void Save()
    {
        //Checks which file it should save to
        if (currentFile == 1)
        {
            //Gets the player position and stores it
            if (player)
            {

                  
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

                //Debug.Log("You Saved");
            }
        }

        //Checks which file it should save to
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

            //Debug.Log("You Saved");
        }

        //Checks which file it should save to
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

            //Debug.Log("You Saved");
        }
    }


    //Loads data from playerPrefs
    public void Load()
    {
        //Debug.Log("You Enterd Load Function");
        //Checks which file it should load from
        if (currentFile == 1)
        {
            //Debug.Log("You Enterd File");
            //Checks if it has a key before it trys to load anything
            if (PlayerPrefs.HasKey("CheckpointPositionX_1"))
            {
                float checkpointPositionX = PlayerPrefs.GetFloat("CheckpointPositionX_1");
                float checkpointPositionY = PlayerPrefs.GetFloat("CheckpointPositionY_1");
                float checkpointPositionZ = PlayerPrefs.GetFloat("CheckpointPositionZ_1");
                player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, checkpointPositionZ);

                smallShards = PlayerPrefs.GetInt("SmallShards_1");
                bigShards = PlayerPrefs.GetInt("BigShards_1");
                crystals = PlayerPrefs.GetInt("Crystals_1");

               // Debug.Log("You Loaded");
            }
        }

        //Checks which file it should load from
        if (currentFile == 2)
        {
            //Checks if it has a key before it trys to load anything
            if (PlayerPrefs.HasKey("CheckpointPositionX_2"))
            {
                float checkpointPositionX = PlayerPrefs.GetFloat("CheckpointPositionX_2");
                float checkpointPositionY = PlayerPrefs.GetFloat("CheckpointPositionY_2");
                float checkpointPositionZ = PlayerPrefs.GetFloat("CheckpointPositionZ_2");
                player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, checkpointPositionZ);

                smallShards = PlayerPrefs.GetInt("SmallShards_2");
                bigShards = PlayerPrefs.GetInt("BigShards_2");
                crystals = PlayerPrefs.GetInt("Crystals_2");

                //Debug.Log("You Loaded");
            }
        }

        //Checks which file it should load from
        if (currentFile == 3)
        {
            //Checks if it has a key before it trys to load anything
            if (PlayerPrefs.HasKey("CheckpointPositionX_3"))
            {
                float checkpointPositionX = PlayerPrefs.GetFloat("CheckpointPositionX_3");
                float checkpointPositionY = PlayerPrefs.GetFloat("CheckpointPositionY_3");
                float checkpointPositionZ = PlayerPrefs.GetFloat("CheckpointPositionZ_3");
                player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, checkpointPositionZ);

                smallShards = PlayerPrefs.GetInt("SmallShards_3");
                bigShards = PlayerPrefs.GetInt("BigShards_3");
                crystals = PlayerPrefs.GetInt("Crystals_3");

               // Debug.Log("You Loaded");
            }
        }

    }


    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySetting", qualitySetting);
        PlayerPrefs.SetInt("CameraInverted", cameraInverted);
        PlayerPrefs.SetFloat("CameraSensitivity", cameraSensitivity);
        PlayerPrefs.SetFloat("CameraSmoothing", cameraSmoothing);
        PlayerPrefs.SetFloat("MasterSoundVolume", masterSoundVolume);
        PlayerPrefs.SetFloat("MusicSoundVolume", MusicSoundVolume);
        PlayerPrefs.SetFloat("DialogueSoundVolume", dialogueSoundVolume);
        PlayerPrefs.SetFloat("MasterVolumePercent", masterVolumePercent);


    }

    public void SaveSliderPosition()
    {
        PlayerPrefs.SetFloat("SliderButtonPositionX", sliderButtonPostionX);
    }

    public void SaveAudioSliderPositions()
    {
        PlayerPrefs.SetFloat("MusicSliderPositionX", musicSliderPositionX);
        PlayerPrefs.SetFloat("MasterSliderPositionX", masterSliderPositionX);
        PlayerPrefs.SetFloat("DialogueSliderPositionX", dialogueSliderPositionX);
    }



    public void LoadSettings()
    {
        if (mainCamera)
        {
            if (PlayerPrefs.GetInt("CameraInverted") == 1)
            {
                mainCamera.invY = true;
            }
            else if (PlayerPrefs.GetInt("CameraInverted") == 0)
            {
                mainCamera.invY = false;
            }
            else
            {
                Debug.LogError("Camera Inverted Int Not set to either 1 or 0");
            }
            mainCamera.sensitivity = PlayerPrefs.GetFloat("CameraSensitivity");

            mainCamera.rotationsmoothTime = PlayerPrefs.GetFloat("CameraSmoothing");

        }

        //if (audioHandler)
        //{
        //    for (int i = 0; i < audioHandler.allAudioSources.Length; i++)
        //    {
        //        audioHandler.allAudioSources[i].volume = PlayerPrefs.GetFloat("MasterSoundVolume");
        //    }
        //    audioHandler.GetMusicSouce().volume = PlayerPrefs.GetFloat("MusicSoundVolume");
        //}


        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualitySetting"), true);

    }

    //Loads the scene from the saved scene in playerPrefs
    public void LoadSavedScene()
    {
        //Checks which file the scene should be loaded from
        if (currentFile == 1)
        {
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene_1");
            SceneManager.LoadSceneAsync(savedSceneIndex);

        }

        //Checks which file the scene should be loaded from
        else if (currentFile == 2)
        {
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene_2");
            SceneManager.LoadSceneAsync(savedSceneIndex);

        }

        //Checks which file the scene should be loaded from
        else if (currentFile == 3)
        {
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene_2");
            SceneManager.LoadSceneAsync(savedSceneIndex);

        }

    }


    //A unique function that is called whenever a scene loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Finds if the player is in the scene
        if (GameObject.FindGameObjectWithTag("Player") && GameObject.FindGameObjectWithTag("MainCamera") && GameObject.FindGameObjectWithTag("AudioManager"))
        {

            //Sets the found player to the player variable
            player = GameObject.FindObjectOfType<PlayerClass>().GetComponent<PlayerClass>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerCamera>();
            audioHandler = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioHandler>();


            //Checks whether or not it is loading from a file or not;
            if (!loadingFromContinue && SceneManager.GetActiveScene().buildIndex == 1)
            {
                ClearCollectedShards();
                ResetListOfShards();
                //ClearTriggeredMonoliths();
                //ResetListOfMonoliths();
                //Sets the beginning variable to the beginning object in the scene
                beginning = player.transform;

                //Sets current checkpoint to the beginning
                currentCheckpoint = beginning;

                HudManager.ResetShards();
                HudManager.ResetMoons();

                //Save the game
                Save();

                LoadSettings();
                //Debug.Log("Gets Called ");

            }
            else if (loadingFromContinue)
            {
                //Load the data
                Load();

                //Sets the condition to load from continue to false;

                LoadSettings();

                loadingFromContinue = false;
            }
            else
            {
                beginning = player.transform;
                currentCheckpoint = beginning;
                Save();
                LoadSettings();
                Debug.Log("Else Worked");
            }
            Debug.Log(loadingFromContinue + " Loading From Continue");
        }
    }

    //Function to handle input testing
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
            PlayerPrefs.DeleteKey("MasterSoundVolume");
            PlayerPrefs.DeleteKey("MusicSoundVolume");
            PlayerPrefs.DeleteKey("DialogueSoundVolume");

        }
    }

    //Function to delete data from the playerPrefs based on which file is selected
    public void ClearSavedFiles()
    {
        //Checks which file it should delete the data from
        if (currentFile == 1)
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
            //Debug.Log("Cleared Data");
        }

        //Checks which file it should delete the data from
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

        //Checks which file it should delete the data from
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

    //Function that saves the IDs of the shards into playerPrefs
    public void SaveCollectedShardsID()
    {


        PlayerPrefs.SetString("CollectedShardsIDs" + PlayerPrefs.GetInt("NumberOfCollectedShards"), collectedShards[collectedShards.Count - 1]);

        PlayerPrefs.SetInt("NumberOfCollectedShards", collectedShards.Count);
        


    }

    //Function that loads the IDs of the shards into a list
    public void LoadCollectedShards()
    {
       
        for (int i = 0; i < PlayerPrefs.GetInt("NumberOfCollectedShards"); i++)
        {
            
            AddToListOfCollectedShards(PlayerPrefs.GetString("CollectedShardsIDs" + i));
        }
    }

    public void SaveTriggeredMonoliths()
    {
        PlayerPrefs.SetString("TriggeredMonoliths" + PlayerPrefs.GetInt("NumberOfTriggeredMonoliths"), triggeredMonoliths[triggeredMonoliths.Count - 1]);

        PlayerPrefs.SetInt("NumberOfTriggeredMonoliths", triggeredMonoliths.Count);
    }

    public void LoadTriggeredMonoliths()
    {

        for (int i = 0; i < PlayerPrefs.GetInt("NumberOfTriggeredMonoliths"); i++)
        {

            AddToListOfTriggeredMonoliths(PlayerPrefs.GetString("TriggeredMonoliths" + i));
        }
    }

    //Public function to increase small shards
    public void increaseSmallShards(int amountOfShards)
    {
        smallShards = amountOfShards;
    }

    //Public function to increase big shards
    public void increaseBigShards(int amountOfShards)
    {
        bigShards = amountOfShards;
    }

    //Public function to increase crystals
    public void increaseCrystals(int amountOfCrystals)
    {
        smallShards = amountOfCrystals;
    }

    public int GetShards()
    {
        return PlayerPrefs.GetInt("SmallShards_1");
    }

    //Public function to update current checkpoint
    public void updateCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Save();
    }

    public void setInverted()
    {
        if (PlayerPrefs.GetInt("CameraInverted") == 0)
        {
            cameraInverted = 1;
            SaveSettings();
        }
        else if (PlayerPrefs.GetInt("CameraInverted") == 1)
        {
            cameraInverted = 0;
            SaveSettings();
        }
    }

    public void setCameraSensitivity(float _float)
    {
        cameraSensitivity = _float;
    }

    public float getCameraSensitivity()
    {
        return PlayerPrefs.GetFloat("CameraSensitivity");
    }

    public float getCameraInverted()
    {
        return PlayerPrefs.GetInt("CameraInverted");
    }

    public int GetQualitySetting()
    {
        return qualitySetting;
    }

    public void SetQualitySettings(int newQualitySetting)
    {
        qualitySetting = newQualitySetting;
    }

    public float GetCameraSmoothing()
    {
        return cameraSmoothing;
    }

    public void SetCameraSmoothing(float newSmoothingValue)
    {
        cameraSmoothing = newSmoothingValue;
    }

    public float GetSliderPosition()
    {
        return PlayerPrefs.GetFloat("SliderButtonPositionX"); ;
    }

    public void SetSliderPosition(float newPositionX)
    {
        sliderButtonPostionX = newPositionX;

    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterSoundVolume");
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicSoundVolume");
    }

    public float GetDialogueVolume()
    {
        return PlayerPrefs.GetFloat("DialogueSoundVolume");
    }

    public void SetMasterVolume(float newFloat)
    {
        masterSoundVolume = newFloat;
        SaveSettings();
    }
    public void SetMusicVolume(float newFloat)
    {
        MusicSoundVolume = newFloat;
        SaveSettings();
    }

    public void SetDialogueVolume(float newFloat)
    {
        dialogueSoundVolume = newFloat;
        SaveSettings();
    }

    public float GetMasterVolumePercent()
    {
        return PlayerPrefs.GetFloat("MasterVolumePercent");
    }

    public void SetMasterPercent(float newFloat)
    {
        masterVolumePercent = newFloat;
        SaveSettings();
    }


    public void SetMusicSliderPosition(float newPosition)
    {
        musicSliderPositionX = newPosition;
        SaveAudioSliderPositions();
    }

    public void SetMasterSliderPosition(float newPosition)
    {
        masterSliderPositionX = newPosition;
        SaveAudioSliderPositions();
    }

    public void SetDialogueSliderPosition(float newPosition)
    {
        dialogueSliderPositionX = newPosition;
        SaveAudioSliderPositions();
    }

    public float GetMusicSliderPosition()
    {
        return PlayerPrefs.GetFloat("MusicSliderPositionX");
    }

    public float GetMasterSliderPosition()
    {
        return PlayerPrefs.GetFloat("MasterSliderPositionX");
    }

    public float GetDialogueSliderPosition()
    {
        return PlayerPrefs.GetFloat("DialogueSliderPositionX");
    }

    public List<string> ReturnListOfCollectedShards()
    {
        return collectedShards;
    }

    public void AddToListOfCollectedShards(string IDOfCollectedShard)
    {
        collectedShards.Add(IDOfCollectedShard);
    }

    public void ResetListOfShards()
    {

        collectedShards.Clear();
    }

    public void ClearCollectedShards()
    {

        for (int i = 0; i < PlayerPrefs.GetInt("NumberOfCollectedShards"); i++)
        {
            PlayerPrefs.DeleteKey("CollectedShardsIDs" + i);


        }
        PlayerPrefs.DeleteKey("NumberOfCollectedShards");
    }

    public void ClearTriggeredMonoliths()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("NumberOfTriggeredMonoliths"); i++)
        {
            PlayerPrefs.DeleteKey("TriggeredMonoliths" + i);


        }
        PlayerPrefs.DeleteKey("NumberOfTriggeredMonoliths");
    }

    public void AddToListOfTriggeredMonoliths(string NameOfMonolith)
    {
        triggeredMonoliths.Add(NameOfMonolith);
    }

    public void ResetListOfMonoliths()
    {

        triggeredMonoliths.Clear();
    }

}
