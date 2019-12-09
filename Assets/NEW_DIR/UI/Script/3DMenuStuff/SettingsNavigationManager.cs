// Written By Benjamin Young November 14/2019.  Last Updated December 9/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsNavigationManager : UIManager
{

    public GameObject[] MenuItems = new GameObject[4];
    MenuButtons[] buttons = new MenuButtons[4];
    [SerializeField]
    int selected = 0;
    public Material whenSelected;
    public Material whenNotSelected;
    [Range(0f, 1f)]
    public float bufferTime;
    [Range(0f, 2f)]
    public float animationDelayTime;
    float verticalInput;
    bool recieveInput = true;
    public UIManager ui;
    bool canInteractWithButtons;
    public MainMenuAudioManager audioManager;




    void Awake()
    {
        InitilizeButtons();
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        InitilizeButtons();
    }

    void InitilizeButtons()
    {


        MenuItems[0].AddComponent<Button_Video>();
        buttons[0] = MenuItems[0].GetComponent<Button_Video>();
        MenuItems[1].AddComponent<Button_Audio>();
        buttons[1] = MenuItems[1].GetComponent<Button_Audio>();
        MenuItems[2].AddComponent<Button_Controls>();
        buttons[2] = MenuItems[2].GetComponent<Button_Controls>();
        if(Time.timeScale < 1)
        {
            MenuItems[3].AddComponent<Button_PauseMenuSettingsBack>();
            buttons[3] = MenuItems[3].GetComponent<Button_PauseMenuSettingsBack>();
        }
        else
        {
            MenuItems[3].AddComponent<Button_SettingsBack>();
            buttons[3] = MenuItems[3].GetComponent<Button_SettingsBack>();

        }





    }

    void Update()
    {

        

        if(Time.timeScale < 1)
        {
             verticalInput = Input.GetAxisRaw("VerticalJoy") + Input.GetAxisRaw("Vertical");
        }
        else
        {
             verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        }
        if (canInteractWithButtons)
        {
            if(Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape))
            {
                selected = 3;
                buttons[3].Execute(UIManager.singleton);
            }

            for (int i = 0; i < MenuItems.Length; i++)
            {
                if (selected == i)
                {
                    MenuItems[i].gameObject.GetComponent<Renderer>().material = whenSelected;
                    if (Input.GetButtonDown("AButton"))
                    {
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Select");
                        }
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

            if (verticalInput > 0.9f && selected > 0 && recieveInput == true && canInteractWithButtons)
            {
               

                StartCoroutine("InputBufferSubract");
                verticalInput = 0;
            }
            else if (verticalInput < -0.9f && selected < MenuItems.Length - 1 && recieveInput == true && canInteractWithButtons)
            {
                

                StartCoroutine("InputBufferAdd");
                verticalInput = 0;
            }
        }
    }

    private IEnumerator InputBufferSubract()
    {
        recieveInput = false;
        selected--;
        if (GameObject.FindGameObjectWithTag("AudioManager"))
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
            audioManager.PlaySound("Navigate");
        }
        yield return new WaitForSeconds(bufferTime * Time.timeScale);
        recieveInput = true;
    }

    private IEnumerator InputBufferAdd()
    {
        recieveInput = false;
        selected++;
        if (GameObject.FindGameObjectWithTag("AudioManager"))
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
            audioManager.PlaySound("Navigate");
        }
        yield return new WaitForSeconds(bufferTime * Time.timeScale);
        recieveInput = true;
    }

    private IEnumerator AnimationDelay(float delay)
    {

        canInteractWithButtons = false;
        yield return new WaitForSeconds(delay * Time.timeScale);
        canInteractWithButtons = true;

    }

    public void SetCanInteractWithButtons(bool newBool)
    {
        canInteractWithButtons = newBool;
    }

    public void SetSelected(int newSelected)
    {
        selected = newSelected;
    }

}
