using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.8f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    public AudioSource source;
    

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        
        source.pitch = pitch;
        source.Play();
    }
}

public class MainMenuAudioManager : MonoBehaviour
{
    public static MainMenuAudioManager instance;

    [SerializeField]
    Sound[] sounds;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance != null)
        {
            Debug.Log("More than one AudioManager in this scene.");
        }
        else
        {
            instance = this;
        }
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }

        //Couldn't find the sound
        Debug.Log("AudioManager: Sound not found in list," + _name);
    }

    //Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }   


}
