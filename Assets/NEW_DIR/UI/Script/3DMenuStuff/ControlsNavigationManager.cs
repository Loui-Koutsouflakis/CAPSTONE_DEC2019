// Written By Benjamin Young October 31/2019.  Last Updated December 9/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsNavigationManager : UIManager
{

    public GameObject[] MenuItems = new GameObject[6];
    MenuButtons[] buttons = new MenuButtons[6];
    [SerializeField]
    int selected = 0;
    public Material whenSelected;
    public Material whenNotSelected;
    public Material currentSelectedSmoothing;
    [Range(0f, 1f)]
    public float bufferTime;
    [Range(0f, 2f)]
    public float animationDelayTime;
    float verticalInput;
    float horizontalInput;
    bool recieveInput = true;
    public UIManager ui;
    bool canInteractWithButtons;
    public MainMenuAudioManager audioManager;

    public GameObject invertButton1;
    public GameObject invertButton2;
    SaveGameManager saveGameManager;
    int cameraSmoothingButton;
    float currentCameraSmoothingValue;
    public Transform sliderMin;
    public Transform sliderMax;
    public GameObject sliderButton;
    public float rateOfSlider;

    float maxValueForSlider;
    float currentValueForSlider;
    float sliderPercentDecimal;
    float sensitivity;


    private GameManager gameManager;

    
    void Awake()
    {
        InitilizeButtons();
    }

    private void Start()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        

        if (saveGameManager.GetSliderPosition() < sliderMin.localPosition.x - 0.2 || saveGameManager.GetSliderPosition() > sliderMax.localPosition.x + 0.2)
        {
            sliderButton.transform.localPosition = new Vector3(saveGameManager.GetSliderPosition(), sliderButton.transform.localPosition.y, sliderButton.transform.localPosition.z);
        }
        else
        {
            sliderButton.transform.localPosition = new Vector3(0, sliderButton.transform.localPosition.y, sliderButton.transform.localPosition.z);
            saveGameManager.SetSliderPosition(sliderButton.transform.localPosition.x);
            saveGameManager.SaveSliderPosition();
        }
    }

    private void OnEnable()
    {
        
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        StartCoroutine(SliderButtonEnable(0.9f));


        if (saveGameManager.GetSliderPosition() < sliderMin.localPosition.x - 0.2 || saveGameManager.GetSliderPosition() > sliderMax.localPosition.x + 0.2)
        {
            sliderButton.transform.localPosition = new Vector3(saveGameManager.GetSliderPosition(), sliderButton.transform.localPosition.y, sliderButton.transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetSliderPosition(sliderButton.transform.localPosition.x);
            saveGameManager.SaveSliderPosition();
        }


    }

    

    void InitilizeButtons()
    {


        MenuItems[0].AddComponent<Button_ControlsInvert>();
        buttons[0] = MenuItems[0].GetComponent<Button_ControlsInvert>();
        MenuItems[2].AddComponent<Button_ControlsClassic>();
        buttons[2] = MenuItems[2].GetComponent<Button_ControlsClassic>();
        MenuItems[3].AddComponent<Button_ControlsStandard>();
        buttons[3] = MenuItems[3].GetComponent<Button_ControlsStandard>();
        MenuItems[4].AddComponent<Button_ControlsVeteran>();
        buttons[4] = MenuItems[4].GetComponent<Button_ControlsVeteran>();
        MenuItems[5].AddComponent<Button_ControlsBack>();
        buttons[5] = MenuItems[5].GetComponent<Button_ControlsBack>();





    }

    void Update()
    {
        //Debug.Log(saveGameManager.getCameraSensitivity());

        currentCameraSmoothingValue = saveGameManager.GetCameraSmoothing();


        if(saveGameManager.getCameraInverted() == 0)
        {
            invertButton1.SetActive(false);
            invertButton2.SetActive(false);
        }
        else
        {
            invertButton1.SetActive(true);
            invertButton2.SetActive(true);
        }



        if (Time.timeScale < 1)
        {
            verticalInput = Input.GetAxisRaw("VerticalJoy") + Input.GetAxisRaw("Vertical");
        }
        else
        {
            verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        }

        if (Time.timeScale < 1)
        {
            horizontalInput = Input.GetAxisRaw("HorizontalJoy") + Input.GetAxisRaw("Horizontal");
        }
        else
        {
            horizontalInput = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");

        }
        
        if (canInteractWithButtons)
        {
            if (Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape))
            {
                selected = 5;
                buttons[5].Execute(UIManager.singleton);
            }

            if(currentCameraSmoothingValue < 0.202)
            {
                cameraSmoothingButton = 2;
                MenuItems[cameraSmoothingButton].gameObject.GetComponent<Renderer>().material = currentSelectedSmoothing;
            }
            else if(currentCameraSmoothingValue < 0.609)
            {
                cameraSmoothingButton = 3;
                MenuItems[cameraSmoothingButton].gameObject.GetComponent<Renderer>().material = currentSelectedSmoothing;
            }
            else if (currentCameraSmoothingValue < 1)
            {
                cameraSmoothingButton = 4;
                MenuItems[cameraSmoothingButton].gameObject.GetComponent<Renderer>().material = currentSelectedSmoothing;
            }

            


            for (int i = 0; i < MenuItems.Length; i++)
            {

                if(selected == 2)
                {
                    if (horizontalInput > 0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        StartCoroutine("InputBufferAdd");
                    }
                    else if(verticalInput > 0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        selected = 1;
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Navigate");
                        }
                        canInteractWithButtons = false;
                        StartCoroutine(TraversalDelay(0.3f));
                    }
                    else if(verticalInput < -0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        selected = 5;
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Navigate");
                        }
                    }
                    
                }

                if (selected == 4)
                {
                    if (horizontalInput < -0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        StartCoroutine("InputBufferSubract");
                    }
                    else if (verticalInput > 0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        selected = 1;
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Navigate");
                        }
                        canInteractWithButtons = false;
                        StartCoroutine(TraversalDelay(0.3f));
                    }
                    else if (verticalInput < -0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        selected = 5;
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Navigate");
                        }
                    }

                }

                if (selected == 3)
                {
                    if (horizontalInput < -0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        StartCoroutine("InputBufferSubract");
                    }
                    else if (horizontalInput > 0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        StartCoroutine("InputBufferAdd");
                    }
                    else if (verticalInput > 0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        selected = 1;
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Navigate");
                        }
                        canInteractWithButtons = false;
                        StartCoroutine(TraversalDelay(0.3f));
                    }
                    else if (verticalInput < -0.9f && recieveInput == true && canInteractWithButtons)
                    {
                        selected = 5;
                        if (GameObject.FindGameObjectWithTag("AudioManager"))
                        {
                            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                            audioManager.PlaySound("Navigate");
                        }
                    }

                }

                if (selected == 1)
                {
                    if(horizontalInput > 0.1)
                    {
                        
                        IncreaseSlider();
                        CalculateSensitivityValue();
                    }
                    else if(horizontalInput < -0.1)
                    {
                        DecreaseSlider();
                        CalculateSensitivityValue();

                    }
                }

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
                else if(selected != i && i != cameraSmoothingButton)
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

    private IEnumerator SliderButtonEnable(float delay)
    {

        
        yield return new WaitForSeconds(delay * Time.timeScale);
        sliderButton.SetActive(true);

    }

    public void SetCanInteractWithButtons(bool newBool)
    {
        canInteractWithButtons = newBool;
    }

    public void SetSelected(int newSelected)
    {
        selected = newSelected;
    }

 
    void IncreaseSlider()
    {
        if(sliderButton.transform.position.x <= sliderMax.position.x)
        {
            sliderButton.transform.position += new Vector3(horizontalInput * rateOfSlider , 0,0 ) * Time.deltaTime;
        }
    }

    void DecreaseSlider()
    {
        if (sliderButton.transform.position.x >= sliderMin.position.x)
        {
            sliderButton.transform.position += new Vector3(horizontalInput * rateOfSlider, 0, 0) * Time.deltaTime;
        }
    }

    void CalculateSensitivityValue()
    {
        Debug.Log(saveGameManager.getCameraSensitivity() + " Saved Sensitivity");

        maxValueForSlider = Mathf.Abs(sliderMin.localPosition.x) + Mathf.Abs(sliderMax.localPosition.x);
        
        Debug.Log(maxValueForSlider + " Max Value For Slider");
        currentValueForSlider = (sliderButton.transform.localPosition.x - (sliderMin.localPosition.x));
        Debug.Log(currentValueForSlider + " Current Value Of Slider");

        sliderPercentDecimal = currentValueForSlider / maxValueForSlider;

        sensitivity = Mathf.Ceil(sliderPercentDecimal * -5);
       
        Debug.Log(sensitivity + " Calculated Sensiivity");


        if (sensitivity < 1)
        {
            sensitivity = 1;
        }
        else if(sensitivity > 5)
        {
            sensitivity = 5;
        }

        saveGameManager.setCameraSensitivity(sensitivity);
        saveGameManager.SaveSettings();
        
        saveGameManager.SetSliderPosition(sliderButton.transform.localPosition.x);
        saveGameManager.SaveSliderPosition();
        saveGameManager.LoadSettings();
       

    }


    private IEnumerator TraversalDelay(float delay)
    {

        
        yield return new WaitForSeconds(delay * Time.timeScale);
        canInteractWithButtons = true;

    }

}
