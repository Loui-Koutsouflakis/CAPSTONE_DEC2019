using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SettingsManager : MonoBehaviour
{
    public Button[] settingsMenuButtons;
    
    public int selected = 0;

   
    public float navigationDelay;

    float timeSincePaused;

    float verticalInput;

    bool recieveInput = true;

    void UpdateSelected(int number)
    {
        if (number > 0)
        {
            if (selected < settingsMenuButtons.Length)
                selected++;
            else
                selected = 0;
        }

        else
        {
            if (selected > 0)
                selected--;
            else
                selected = 0;
        }
    }

    private void OnEnable()
    {
        recieveInput = true;
        selected = 0;
        settingsMenuButtons[selected].Select(); 
        Debug.Log(settingsMenuButtons.Length);
    }

    // Update is called once per frame
    void Update()
    {

            settingsMenuButtons[selected].Select();
            verticalInput = 0;
            verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");



            if (verticalInput < -0.9f && selected < settingsMenuButtons.Length - 1 && recieveInput)
            {
                
                
                StartCoroutine(InputBufferAdd());
            }
            else if (verticalInput > 0.9f && selected > 0 && recieveInput)
            {
               
                
                StartCoroutine(InputBufferSubtract());

            }

            if (Input.GetButtonDown("AButton"))
            {
                settingsMenuButtons[selected].onClick.Invoke();
            }
        
            

       
    }



   
    

   


    private IEnumerator InputBufferAdd()
    {
        recieveInput = false;
        UpdateSelected(1);
        
        yield return new WaitForSeconds(navigationDelay * Time.timeScale);
        recieveInput = true;
    }

    private IEnumerator InputBufferSubtract()
    {
        recieveInput = false;
        UpdateSelected(-1);
       
        yield return new WaitForSeconds(navigationDelay * Time.timeScale);
        recieveInput = true;
    }

    public void SetSelected(int num)
    {
        selected = num;
    }




}


