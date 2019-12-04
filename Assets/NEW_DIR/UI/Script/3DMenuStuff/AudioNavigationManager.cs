using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioNavigationManager : UIManager
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
    float horizontalInput;
    bool recieveInput = true;
    public UIManager ui;
    bool canInteractWithButtons;
    public MainMenuAudioManager audioManager;

    public GameObject[] sliderButtons;

    float maxValueForSlider;
    float currentValueForSlider;
    float sliderPercentDecimal;
    public Transform sliderMin;
    public Transform sliderMax;
    float audioValue;
    public float rateOfSlider;
    
    private AudioSource[] allAudioSources;
    private float masterVolumePercent;
    private AudioHandler audioHandler;

    private SaveGameManager saveGameManager;

    


    void Awake()
    {
        InitilizeButtons();
    }

    private void Start()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        audioHandler = FindObjectOfType<AudioHandler>().GetComponent<AudioHandler>();

        //for (int i = 0; i < sliderButtons.Length; i++)
        //{


        //    if (sliderButtons[i].transform.localPosition.x > sliderMin.transform.localPosition.x || sliderButtons[i].transform.localPosition.x < sliderMax.transform.localPosition.x)
        //    {
        //        sliderButtons[i].transform.localPosition = new Vector3(0, sliderButtons[i].transform.localPosition.y, sliderButtons[i].transform.localPosition.z);
        //    }
        //}

        if (saveGameManager.GetMasterSliderPosition() < sliderMin.localPosition.x || saveGameManager.GetMasterSliderPosition() > sliderMax.localPosition.x)
        {
            sliderButtons[2].transform.localPosition = new Vector3(saveGameManager.GetMasterSliderPosition(), sliderButtons[2].transform.localPosition.y, sliderButtons[2].transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetMasterSliderPosition(sliderButtons[2].transform.localPosition.x);
            saveGameManager.SaveAudioSliderPositions();
        }

        if (saveGameManager.GetMusicSliderPosition() < sliderMin.localPosition.x || saveGameManager.GetMusicSliderPosition() > sliderMax.localPosition.x)
        {
            sliderButtons[0].transform.localPosition = new Vector3(saveGameManager.GetMusicSliderPosition(), sliderButtons[0].transform.localPosition.y, sliderButtons[0].transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetMusicSliderPosition(sliderButtons[0].transform.localPosition.x);
            saveGameManager.SaveAudioSliderPositions();
        }

        if (saveGameManager.GetDialogueSliderPosition() < sliderMin.localPosition.x || saveGameManager.GetDialogueSliderPosition() > sliderMax.localPosition.x)
        {
            sliderButtons[1].transform.localPosition = new Vector3(saveGameManager.GetDialogueSliderPosition(), sliderButtons[1].transform.localPosition.y, sliderButtons[1].transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetDialogueSliderPosition(sliderButtons[1].transform.localPosition.x);
            saveGameManager.SaveAudioSliderPositions();
        }
    }

    private void OnEnable()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        audioHandler = FindObjectOfType<AudioHandler>().GetComponent<AudioHandler>();


        if (saveGameManager.GetMasterSliderPosition() < sliderMin.localPosition.x || saveGameManager.GetMasterSliderPosition() > sliderMax.localPosition.x)
        {
            sliderButtons[2].transform.localPosition = new Vector3(saveGameManager.GetMasterSliderPosition(), sliderButtons[2].transform.localPosition.y, sliderButtons[2].transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetMasterSliderPosition(sliderButtons[2].transform.localPosition.x);
            saveGameManager.SaveAudioSliderPositions();
        }

        if (saveGameManager.GetMusicSliderPosition() < sliderMin.localPosition.x || saveGameManager.GetMusicSliderPosition() > sliderMax.localPosition.x)
        {
            sliderButtons[0].transform.localPosition = new Vector3(saveGameManager.GetMusicSliderPosition(), sliderButtons[0].transform.localPosition.y, sliderButtons[0].transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetMusicSliderPosition(sliderButtons[0].transform.localPosition.x);
            saveGameManager.SaveAudioSliderPositions();
        }

        if (saveGameManager.GetDialogueSliderPosition() < sliderMin.localPosition.x || saveGameManager.GetDialogueSliderPosition() > sliderMax.localPosition.x)
        {
            sliderButtons[1].transform.localPosition = new Vector3(saveGameManager.GetDialogueSliderPosition(), sliderButtons[1].transform.localPosition.y, sliderButtons[1].transform.localPosition.z);
        }
        else
        {
            saveGameManager.SetDialogueSliderPosition(sliderButtons[1].transform.localPosition.x);
            saveGameManager.SaveAudioSliderPositions();
        }



        StartCoroutine(SliderButtonEnable(0.9f, sliderButtons[0]));
        StartCoroutine(SliderButtonEnable(0.9f, sliderButtons[1]));
        StartCoroutine(SliderButtonEnable(0.9f, sliderButtons[2]));
    }

    void InitilizeButtons()
    {


        MenuItems[3].AddComponent<Button_AudioBack>();
        buttons[3] = MenuItems[3].GetComponent<Button_AudioBack>();



        


    }

    void Update()
    {

        Debug.Log(CalculateMasterVolumePercent(sliderButtons[2]));


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

        //saveGameManager.SetMasterPercent(CalculateMasterVolumePercent(sliderButtons[2]));
        //audioHandler.inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume() * CalculateMasterVolumePercent(sliderButtons[2]));
        //audioHandler.inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume() * CalculateMasterVolumePercent(sliderButtons[2]));


        //saveGameManager.SetMusicVolume(CalculateAudioValue(sliderButtons[0]));
        if (canInteractWithButtons)
        {
            if (selected == 0)
            {
                if (horizontalInput > 0.1)
                {

                    saveGameManager.SetMusicVolume(CalculateAudioValue(sliderButtons[0]));
                    audioHandler.inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume());
                    IncreaseSlider(sliderButtons[0]);
                    saveGameManager.SetMusicSliderPosition(sliderButtons[0].transform.localPosition.x);
                    saveGameManager.SaveAudioSliderPositions();
                }
                else if (horizontalInput < -0.1)
                {
                    saveGameManager.SetMusicVolume(CalculateAudioValue(sliderButtons[0]));
                    audioHandler.inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume());
                    DecreaseSlider(sliderButtons[0]);
                    saveGameManager.SetMusicSliderPosition(sliderButtons[0].transform.localPosition.x);
                    saveGameManager.SaveAudioSliderPositions();

                }
            }
            else if(selected == 1)
            {
                if (horizontalInput > 0.1)
                {

                    IncreaseSlider(sliderButtons[1]);
                    
                    saveGameManager.SetDialogueVolume(CalculateAudioValue(sliderButtons[1]));
                    audioHandler.inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume());
                    saveGameManager.SetDialogueSliderPosition(sliderButtons[1].transform.localPosition.x);
                    saveGameManager.SaveAudioSliderPositions();
                }
                else if (horizontalInput < -0.1)
                {
                    DecreaseSlider(sliderButtons[1]);
                    saveGameManager.SetDialogueVolume(CalculateAudioValue(sliderButtons[1]));
                    audioHandler.inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume());
                    saveGameManager.SetDialogueSliderPosition(sliderButtons[1].transform.localPosition.x);
                    saveGameManager.SaveAudioSliderPositions();

                }
            }
            else if (selected == 2)
            {
                if (horizontalInput > 0.1)
                {

                    IncreaseSlider(sliderButtons[2]);
                    saveGameManager.SetMasterVolume(CalculateAudioValue(sliderButtons[2]));
                    saveGameManager.SetMasterSliderPosition(sliderButtons[2].transform.localPosition.x);
                    saveGameManager.SaveAudioSliderPositions();

                    audioHandler.inGameMixer.SetFloat("MasterVolume", saveGameManager.GetMasterVolume());
                    audioHandler.inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume());
                    audioHandler.inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume());

                    //for (int i = 0; i < audioHandler.allMenuAudioSources.Length; i++)
                    //{
                    //    audioHandler.allMenuAudioSources[i].volume = saveGameManager.GetMasterVolume();
                    //    audioHandler.GetMusicSouce().volume = CalculateAudioValue(sliderButtons[0]) * saveGameManager.GetMasterVolume();
                    //}

                    //for (int i = 0; i < audioHandler.allAudioSources.Length; i++)
                    //{
                    //    audioHandler.allAudioSources[i].volume = saveGameManager.GetMasterVolume();
                    //    audioHandler.GetMusicSouce().volume = CalculateAudioValue(sliderButtons[0]) * saveGameManager.GetMasterVolume();

                    //}

                }
                else if (horizontalInput < -0.1)
                {
                    DecreaseSlider(sliderButtons[2]);
                    saveGameManager.SetMasterVolume(CalculateAudioValue(sliderButtons[2]));
                    saveGameManager.SetMasterSliderPosition(sliderButtons[2].transform.localPosition.x);
                    saveGameManager.SaveAudioSliderPositions();

                    audioHandler.inGameMixer.SetFloat("MasterVolume", saveGameManager.GetMasterVolume());
                    audioHandler.inGameMixer.SetFloat("MusicVolume", saveGameManager.GetMusicVolume());
                    audioHandler.inGameMixer.SetFloat("SFXVolume", saveGameManager.GetDialogueVolume());



                    //for (int i = 0; i < audioHandler.allMenuAudioSources.Length; i++)
                    //{
                    //    audioHandler.allMenuAudioSources[i].volume = saveGameManager.GetMasterVolume();
                    //    audioHandler.GetMusicSouce().volume = CalculateAudioValue(sliderButtons[0]) * saveGameManager.GetMasterVolume();

                    //}
                    //for (int i = 0; i < audioHandler.allAudioSources.Length; i++)
                    //{
                    //    audioHandler.allAudioSources[i].volume = saveGameManager.GetMasterVolume();
                    //    audioHandler.GetMusicSouce().volume = CalculateAudioValue(sliderButtons[0]) * saveGameManager.GetMasterVolume();

                    //}

                }
            }



            if (Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Escape))
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

    private IEnumerator SliderButtonEnable(float delay, GameObject sliderButton)
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

    void IncreaseSlider(GameObject sliderButton)
    {
        if (-sliderButton.transform.localPosition.x <= sliderMin.localPosition.x)
        {
            sliderButton.transform.localPosition -= new Vector3(horizontalInput * rateOfSlider, 0, 0) * Time.unscaledDeltaTime;
        }
    }

    void DecreaseSlider(GameObject sliderButton)
    {
        if (-sliderButton.transform.localPosition.x >= sliderMax.localPosition.x)
        {
            sliderButton.transform.localPosition -= new Vector3(horizontalInput * rateOfSlider, 0, 0) * Time.unscaledDeltaTime;
        }
    }

    float CalculateAudioValue(GameObject sliderButton)
    {
    

        maxValueForSlider = Mathf.Abs(sliderMin.localPosition.x) + Mathf.Abs(sliderMax.localPosition.x);

       
        currentValueForSlider = (sliderButton.transform.localPosition.x - (sliderMin.localPosition.x));
        

        sliderPercentDecimal = currentValueForSlider / maxValueForSlider;

        audioValue = ((sliderPercentDecimal * 80 + 80) * -1);


        


        if (audioValue < -80)
        {
            audioValue = -80;
        }
        else if (audioValue > 0)
        {
            audioValue = 0;
        }

        
        return audioValue;

      


    }

    private float CalculateMasterVolumePercent(GameObject sliderButton)
    {
        maxValueForSlider = Mathf.Abs(sliderMin.localPosition.x) + Mathf.Abs(sliderMax.localPosition.x);

        
        currentValueForSlider = (sliderButton.transform.localPosition.x - (sliderMin.localPosition.x));
        

        sliderPercentDecimal =Mathf.Abs( currentValueForSlider / maxValueForSlider);

        if(sliderPercentDecimal > 1)
        {
            sliderPercentDecimal = 1;
        }
        else if(sliderPercentDecimal < 0)
        {
            sliderPercentDecimal = 0;
        }

        saveGameManager.SetMasterPercent(sliderPercentDecimal);

        return sliderPercentDecimal;
    }

    
}


