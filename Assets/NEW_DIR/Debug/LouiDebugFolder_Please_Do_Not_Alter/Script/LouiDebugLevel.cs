using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LouiDebugLevel : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            SceneManager.LoadScene(1);
        }

        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            SceneManager.LoadScene(2);
        }
    }
}
