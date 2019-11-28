using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{

    public AudioSource[] allMenuAudioSources;
    public AudioSource[] allAudioSources;
    AudioSource[] sources;
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

        for (int i = 0; i < allMenuAudioSources.Length; i++)
        {
            allMenuAudioSources[i].volume = saveGameManager.GetMasterVolume();
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
        Debug.Log("On Level Load Works");
        GetAllAudioSources();

        if (GameObject.FindGameObjectWithTag("MusicSource"))
        {
            musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
            musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();
        }
        else
        {
            musicSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
            musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();
        }
    }

    public void GetAllAudioSources()
    {
        allMenuAudioSources = GetComponents<AudioSource>();
        sources = FindObjectsOfType<AudioSource>();
        allAudioSources = new AudioSource[sources.Length];
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            allAudioSources[i] = sources[i].GetComponent<AudioSource>();
        }

    }

    public AudioSource GetMusicSouce()
    {
        return musicSource;
    }
}
