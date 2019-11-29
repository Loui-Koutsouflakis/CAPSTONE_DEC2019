using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{

    public AudioSource[] allAudioSources;
    private SaveGameManager saveGameManager;
    private AudioSource musicSource;
    private AudioSource dialogueSource;

    // Start is called before the first frame update
    void Start()
    {

        if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
 
            
        }
        GetAllAudioSources();

        for (int i = 0; i < allAudioSources.Length; i++)
        {
            allAudioSources[i].volume = saveGameManager.GetMasterVolume();
        }
        if(GameObject.FindGameObjectWithTag("MusicSource"))
        {
            musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
            musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();
          
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            
            if(saveGameManager.loading == true)
            {

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
         
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetAllAudioSources();

        if (GameObject.FindGameObjectWithTag("MusicSource"))
        {
            musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
            musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();
        }
    }

    public void GetAllAudioSources()
    {
        allAudioSources = GetComponents<AudioSource>();//FindObjectsOfType<AudioSource>();
        //allAudioSources = new AudioSource[AudioSourcesInScene.Length];
        //for (int i = 0; i < AudioSourcesInScene.Length; i++)
        //{
        //    allAudioSources[i] = AudioSourcesInScene[i].GetComponent<AudioSource>();
        //}

    }

    public AudioSource GetMusicSouce()
    {
        return musicSource;
    }
}
