using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSfx : MonoBehaviour
{
    public AudioSource sfxSource;
    public SfxClip[] clips;

    public void PlayFromSource()
    {
        sfxSource.Stop();
        sfxSource.Play();
    }

    public void PlayRandomClip()
    {
        sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length - 1)].clip);
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
