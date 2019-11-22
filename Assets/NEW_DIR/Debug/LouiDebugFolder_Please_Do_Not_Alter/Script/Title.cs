using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private Animator cinematicAnim;
    [SerializeField]
    private Image faderImage;
    [SerializeField]
    private TransitionManager transitionManager;

    private bool canStart;
    private bool opening;
    private float cinematicLength = 8.8f;


    private void Start()
    {
        StartCoroutine(OpeningSequence());
    }

    private void Update()
    {
        if(Input.GetButtonDown("Start") || Input.GetButtonDown("AButton") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (canStart)
            {
                StartCoroutine(OpenMenu());
            }
            else if(opening)
            {
                StartCoroutine(SkipOpening());
            }
        }
    }

    public IEnumerator OpeningSequence()
    {
        opening = true;
        yield return new WaitForSeconds(cinematicLength);
        opening = false;
        canStart = true;
    }

    public IEnumerator SkipOpening()
    {
        opening = false;
        canStart = false;
        transitionManager.StopAllCoroutines();
        transitionManager.fadeIn = false;
        transitionManager.fadeOut = false;
        faderImage.color = Color.black;
        StartCoroutine(OpenMenu());
        yield return new WaitForSeconds(0.3f);
        faderImage.color = Color.clear;
    }

    public IEnumerator OpenMenu()
    {
        canStart = false;
        cinematicAnim.SetTrigger("MenuIn");
        yield return new WaitForSeconds(0.15f);
        mainMenu.SetActive(true);
    }
}
