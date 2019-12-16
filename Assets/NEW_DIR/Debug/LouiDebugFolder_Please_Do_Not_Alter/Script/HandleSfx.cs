using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSfx : MonoBehaviour
{
    public AudioSource sfxSource;
    public SfxClip[] clips;

    public bool localSource = false;

    private void Awake()
    {
        if(!localSource)
        {
            sfxSource = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<AudioSource>();
        }
        else
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }
    public void PlayFromSource()
    {
        sfxSource.Stop();
        sfxSource.Play();
    }

    public void PlayRandomClip(int start, int end)
    {
        sfxSource.PlayOneShot(clips[Random.Range(start, end)].clip);
    }

    public void PlayOneShotByIndex(int index)
    {
        sfxSource.PlayOneShot(clips[index].clip);
    }

    public void PlayOneShotByName(string clipName)
    {
        foreach(SfxClip clip in clips)
        {
            if(clip.name == clipName)
            {
                sfxSource.PlayOneShot(clip.clip);
            }
        }
    }
}

[System.Serializable]
public struct SfxClip
{
    public string name;
    public AudioClip clip;
}
