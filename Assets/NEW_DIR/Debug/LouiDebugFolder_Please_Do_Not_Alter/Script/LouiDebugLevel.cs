using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LouiDebugLevel : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene(2);
            }

            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                SceneManager.LoadScene(3);
            }
        }
    }
}
