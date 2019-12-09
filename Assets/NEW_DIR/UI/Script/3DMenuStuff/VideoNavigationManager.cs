// Written By Benjamin Young November 14/2019.  Last Updated December 9/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoNavigationManager : UIManager
{

    public GameObject[] MenuItems = new GameObject[4];
    MenuButtons[] buttons = new MenuButtons[4];
    [SerializeField]
    int selected = 0;
    public Material whenSelected;
    public Material whenNotSelected;
    public Material currentSelectedQuality;
    [Range(0f, 1f)]
    public float bufferTime;
    [Range(0f, 2f)]
    public float animationDelayTime;
    float verticalInput;
    bool recieveInput = true;
    public UIManager ui;
    bool canInteractWithButtons;
    public MainMenuAudioManager audioManager;
    int currentQualitySetting;
    SaveGameManager saveGameManager;
    int qualityButton;




    void Awake()
    {
        InitilizeButtons();
    }

    private void Start()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
    }

    private void OnEnable()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
    }

    void InitilizeButtons()
    {

        MenuItems[0].AddComponent<Button_VideoHigh>();
        buttons[0] = MenuItems[0].GetComponent<Button_VideoHigh>();
        MenuItems[1].AddComponent<Button_VideoMedium>();
        buttons[1] = MenuItems[1].GetComponent<Button_VideoMedium>();
        MenuItems[2].AddComponent<Button_VideoLow>();
        buttons[2] = MenuItems[2].GetComponent<Button_VideoLow>();
        MenuItems[3].AddComponent<Button_VideoBack>();
        buttons[3] = MenuItems[3].GetComponent<Button_VideoBack>();
       





    }

    void Update()
    {

        currentQualitySetting = saveGameManager.GetQualitySetting();





        if (Time.timeScale < 1)
        {
            verticalInput = Input.GetAxisRaw("VerticalJoy") + Input.GetAxisRaw("Vertical");
        }
        else
        {
            verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        }
        if (canInteractWithButtons)
        {
            if (Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape))
            {
                selected = 3;
                buttons[3].Execute(UIManager.singleton);
            }
            if(currentQualitySetting == 5)
            {
                MenuItems[0].gameObject.GetComponent<Renderer>().material = currentSelectedQuality;
                qualityButton = 0;
            }
            else if(currentQualitySetting == 3)
            {
                MenuItems[1].gameObject.GetComponent<Renderer>().material = currentSelectedQuality;
                qualityButton = 1;
            }
            else if (currentQualitySetting == 1)
            {
                MenuItems[2].gameObject.GetComponent<Renderer>().material = currentSelectedQuality;
                qualityButton = 2;
            }
            else
            {
                //Debug.Log(currentQualitySetting); 
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
                else if(selected != i && i != qualityButton)
                {
                    MenuItems[i].gameObject.GetComponent<Renderer>().material = whenNotSelected;
                    //buttons[i].selected = false;
                }

            }

            if (verticalInput > 0.9f && selected > 0 && recieveInput == true && canInteractWithButtons)
            {
                StartCoroutine("InputBufferSubract");
            }
            else if (verticalInput < -0.9f && selected < MenuItems.Length - 1 && recieveInput == true && canInteractWithButtons)
            {
                StartCoroutine("InputBufferAdd");
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
