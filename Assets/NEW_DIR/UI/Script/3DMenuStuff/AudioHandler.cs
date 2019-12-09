// Written By Benjamin Young November 25/2019.  Last Updated December 9/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{

    public AudioSource[] allMenuAudioSources;
    public AudioSource[] allAudioSources;
    AudioSource[] sources;
    private SaveGameManager saveGameManager;
    private AudioSource musicSource;
    private AudioSource dialogueSource;


    public AudioMixer inGameMixer;
    public AudioMixerGroup SFXMixer;
    

    // Start is called before the first frame update
    void Start()
    {

      

        if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
 
            
        }

        inGameMixer.SetFloat("MasterVolume", saveGameManager.GetMasterVolume());
        inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume());
        inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume());

        
        //if(GameObject.FindGameObjectWithTag("MusicSource"))
        //{
        //    musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
        //    musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();

        //}
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
        inGameMixer.SetFloat("MasterVolume", saveGameManager.GetMasterVolume());
        inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume());
        inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume());

        GetAllAudioSources();

        for (int i = 0; i < allMenuAudioSources.Length; i++)
        {
            if (allAudioSources[i].outputAudioMixerGroup == null)
            {
                allMenuAudioSources[i].outputAudioMixerGroup = SFXMixer;

            }
        }
        //Debug.Log("On Level Load Works");
        //GetAllAudioSources();
        //for (int i = 0; i < allAudioSources.Length; i++)
        //{
        //    allAudioSources[i].volume = saveGameManager.GetMasterVolume();
        //}


        //if (GameObject.FindGameObjectWithTag("MusicSource"))
        //{
        //    musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
        //    musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();
        //}
        //else
        //{
        //    musicSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        //    musicSource.volume = saveGameManager.GetMusicVolume() * saveGameManager.GetMasterVolume();
        //}
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
