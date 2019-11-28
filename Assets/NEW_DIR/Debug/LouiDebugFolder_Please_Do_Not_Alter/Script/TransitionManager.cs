using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField]
    private Image fader;
    public bool fadeIn;
    public bool fadeOut;
    private float fadeInTime = 1.5f;
    private float fadeOutTime = 2.5f;
    private Color deltaFadeIn = new Color(0f, 0f, 0f, 1f);
    private Color deltaFadeOut = new Color(0f, 0f, 0f, 0.8f);

    void Start()
    {
        StartCoroutine(FadeInSequence());
    }
    
    void Update()
    {
        if(fadeIn)
        {
            fader.color -= deltaFadeIn * Time.deltaTime;
        }   
        else if(fadeOut)
        {
            fader.color += deltaFadeOut * Time.deltaTime;
        }
    }

    public IEnumerator FadeInSequence()
    {
        fadeIn = true;

        yield return new WaitForSeconds(fadeInTime);

        fadeIn = false;
    }

    public IEnumerator SceneTransition(int sceneIndexOffset)
    {
        fadeOut = true;

        yield return new WaitForSeconds(fadeOutTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + sceneIndexOffset);
    }

    public void FadeOut()
    {
        fadeOut = true;
    }
}
