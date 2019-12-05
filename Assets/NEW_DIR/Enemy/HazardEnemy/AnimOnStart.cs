using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimOnStart : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private string paramName;

    [SerializeField]
    private int startAnimIndex;

    [SerializeField]
    private float startDelay;

    void Start()
    {
        StartCoroutine(StartDelay());
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        anim.SetInteger(paramName, startAnimIndex);
    }
}
