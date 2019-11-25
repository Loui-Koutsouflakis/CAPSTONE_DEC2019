using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{

    public AudioSource[] allAudioSources;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
            SceneManager.sceneLoaded += OnSceneLoaded;
         
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioSource[] AudioSourcesInScene = FindObjectsOfType<AudioSource>();
        allAudioSources = new AudioSource[AudioSourcesInScene.Length];
        for (int i = 0; i < AudioSourcesInScene.Length; i++)
        {
            allAudioSources[i] = AudioSourcesInScene[i].GetComponent<AudioSource>();
        }
    }
}
