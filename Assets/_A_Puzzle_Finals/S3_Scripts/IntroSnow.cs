//Created by Dylan LeClair 12/06/2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSnow : MonoBehaviour
{
    //Cache wait time
    private readonly WaitForSeconds timer = new WaitForSeconds(13.0f);

    private void Start()
    {
        //Start timer
        StartCoroutine(TurnOffSnow());
    }

    private IEnumerator TurnOffSnow()
    {
        //Wait for openeing cutscene to finish, then disable gameobject
        yield return timer;

        gameObject.SetActive(false);
    }
}
