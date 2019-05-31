using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenScript : MonoBehaviour
{
    public Image Logo;


    IEnumerator Start()
    {
        Logo.canvasRenderer.SetAlpha(0.0f);

        FadeIn();
        yield return new WaitForSeconds(3.5f);
        FadeOut();
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    void FadeIn()
    {
        Logo.CrossFadeAlpha(1.0f, 2.5f, false);
    }

    void FadeOut()
    {
        Logo.CrossFadeAlpha(0.0f, 2.5f, false);
    }
}
