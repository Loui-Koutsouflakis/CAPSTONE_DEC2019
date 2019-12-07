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

    /// <summary>
    /// Coroutine used for fade transitions of any length. Good for respawns, retries, and cutscene cues.
    /// </summary>
    /// <param name="outTime"> Duration of fade out. </param>
    /// <param name="darkTime"> Duration of full darkness. </param>
    /// <param name="inTime"> Duration of fade in. </param>
    /// <param name="deltaFadeFactor"> Scale of the fade speed (default fade out = 1f, default fade in = 0.8f) </param>
    /// <returns></returns>
    public IEnumerator BlinkSequence(float outTime, float darkTime, float inTime, float deltaFadeFactor)
    {
        deltaFadeIn.a *= deltaFadeFactor;
        deltaFadeOut.a *= deltaFadeFactor;

        fadeOut = true;
        yield return new WaitForSeconds(outTime);
        fadeOut = false;
        yield return new WaitForSeconds(darkTime);
        fadeIn = true;
        yield return new WaitForSeconds(inTime);
        fadeIn = fadeIn = false;

        deltaFadeIn.a /= deltaFadeFactor;
        deltaFadeOut.a /= deltaFadeFactor;
    }

    public void FadeIn()
    {
        fadeIn = true;
    }

    public void FadeOut()
    {
        fadeOut = true;
    }
}
