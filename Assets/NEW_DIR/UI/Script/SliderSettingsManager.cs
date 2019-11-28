using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettingsManager : MonoBehaviour
{
    public Slider slider;
    public SettingsManager settingManager;
    public bool canInteractWithSliders;
    [SerializeField]
    float horizontalInput;
    public float sliderSensitivity;
    private SaveGameManager saveGameManager;
    float timeSinceSlider;
    
    void Start()
    {
        canInteractWithSliders = false;
        if (GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            slider.value = saveGameManager.getCameraSensitivity();
        }
    }

    void Update()
    {
        if (canInteractWithSliders)
        {
            timeSinceSlider += Time.deltaTime;
            horizontalInput = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");



            if (horizontalInput > 0.1f)
            {
                slider.value += (horizontalInput * sliderSensitivity ) * Time.deltaTime;
            }
            else if (horizontalInput < -0.1f)
            {
                slider.value += (horizontalInput * sliderSensitivity) * Time.deltaTime;

            }

            if(Input.GetButtonDown("AButton") && timeSinceSlider > 0.3)
            {
                if(GameObject.FindGameObjectWithTag("SaveGameManager"))
                {
                    saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
                    saveGameManager.setCameraSensitivity(slider.value);
                    saveGameManager.SaveSettings();
                }
                canInteractWithSliders = false;
                settingManager.enabled = true;
                settingManager.selected = 2;
                

            }

        }
        else
        {
            timeSinceSlider = 0;
        }


    }

    public void SetCanInteractWithSliders(bool _bool)
    {
        canInteractWithSliders = _bool;
        Debug.Log(canInteractWithSliders);
    }
     

}