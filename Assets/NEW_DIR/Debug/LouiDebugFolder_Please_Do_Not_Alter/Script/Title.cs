using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private Animator cinematicAnim;

    private bool canStart;
    private float cinematicLength = 13f;


    private void Start()
    {
        StartCoroutine(OpeningSequence());
    }

    private void Update()
    {
        if(canStart && (Input.GetButtonDown("Start") || Input.GetButtonDown("AButton") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            StartCoroutine(OpenMenu());
        }
    }

    public IEnumerator OpeningSequence()
    {
        yield return new WaitForSeconds(cinematicLength);
        canStart = true;
    }

    public IEnumerator OpenMenu()
    {
        canStart = false;
        cinematicAnim.SetTrigger("MenuIn");
        yield return new WaitForSeconds(0.15f);
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
