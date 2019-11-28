using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonManger : MonoBehaviour
{
    public Toggle inverted;
    SaveGameManager saveGameManager;

    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            if(saveGameManager.getCameraInverted() == 1)
            {
                inverted.isOn = true;
            }
            else
            {
                inverted.isOn = false;
            }
        }
    }

    public void toggleInverted()
    {
        if(inverted.isOn)
        {
            Debug.Log("Set to false");

            inverted.isOn = false;
            saveGameManager.setInverted();
            Debug.Log(saveGameManager.getCameraInverted());
        }
        else
        {
            Debug.Log("Set to true");
            inverted.isOn = true; 
            saveGameManager.setInverted();
            Debug.Log(saveGameManager.getCameraInverted());


        }
    }
}
