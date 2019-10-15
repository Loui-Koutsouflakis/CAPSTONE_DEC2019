using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMainMenu : UIManager
{
    public Animation scrollOpen;
    public GameObject[] MenuItems = new GameObject[3];
     MenuButtons[] buttons = new MenuButtons[3];
    int selected = 0;
    public Material whenSelected;
    public Material whenNotSelected;
    [Range(0f,1f)]
    public float bufferTime;
    float verticalInput;
    bool recieveInput = true;
    public UIManager ui;



    void Awake()
    {
        InitilizeButtons();
    }

    void InitilizeButtons()
    {


        MenuItems[0].AddComponent<Button_StartGame>();
        buttons[0] = MenuItems[0].GetComponent<Button_StartGame>();
        Debug.Log(buttons[0]);
        MenuItems[1].AddComponent<Button_Settings>();
        buttons[1] = MenuItems[1].GetComponent<Button_Settings>();
        MenuItems[2].AddComponent<Button_QuitGame>();
        buttons[2] = MenuItems[2].GetComponent<Button_QuitGame>();


        

      
    }

    void Update()
    {

      



        verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
        if(scrollOpen.isPlaying == false)
        {

            for (int i = 0; i < MenuItems.Length; i++)
            {
                if(selected == i)
                {
                    MenuItems[i].gameObject.GetComponent<Renderer>().material = whenSelected;
                    if (Input.GetButton("ABUtton"))
                    {
                        buttons[i].Execute(UIManager.singleton);
                    }

                    //buttons[i].Name();
                }
                else
                {
                    MenuItems[i].gameObject.GetComponent<Renderer>().material = whenNotSelected;
                    //buttons[i].selected = false;
                }
            
            }

            if(verticalInput > 0.9f && selected > 0 && recieveInput == true)
            {
                StartCoroutine("InputBufferSubract");
            }
            else if(verticalInput < -0.9f && selected < MenuItems.Length -1 && recieveInput == true)
            {
                StartCoroutine("InputBufferAdd");
            }
        }
    }

    private IEnumerator InputBufferSubract()
    {
        recieveInput = false;
        selected--;
        yield return new WaitForSeconds(bufferTime);
        recieveInput = true;
    }

    private IEnumerator InputBufferAdd()
    {
        recieveInput = false;
        selected++;
        yield return new WaitForSeconds(bufferTime);
        recieveInput = true;
    }
}
