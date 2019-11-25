using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMusic : MonoBehaviour
{
    public AudioSource levelMusic;
    public AudioSource enemyMusic;
    public AudioSource deathMusic;

    private readonly float musicDelay = 2f;
    private readonly float deathMusicDelay = 1f;

    private void Start()
    {
        StartCoroutine(StartMusicDelay());
    }

    public void PlayLevelMusic()
    {
        deathMusic.Stop();
        levelMusic.Stop();
        levelMusic.Play();
    }

    public IEnumerator StartMusicDelay()
    {
        yield return new WaitForSeconds(musicDelay);

        levelMusic.Play();
    }

    public IEnumerator CueDeathMusic()
    {
        levelMusic.Stop();
        yield return new WaitForSeconds(deathMusicDelay);
        deathMusic.Play();
    }
}
