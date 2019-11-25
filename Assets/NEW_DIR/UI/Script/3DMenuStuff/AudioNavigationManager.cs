using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject[] sliderButtons = new GameObject[3];

    float maxValueForSlider;
    float currentValueForSlider;
    float sliderPercentDecimal;
    public Transform sliderMin;
    public Transform sliderMax;
    float audioValue;
    public float rateOfSlider;


    void Awake()
    {
        InitilizeButtons();
    }

    private void Start()
    {

    }

    private void OnEnable()
    {

    }

    void InitilizeButtons()
    {


        MenuItems[3].AddComponent<Button_AudioBack>();
        buttons[3] = MenuItems[3].GetComponent<Button_AudioBack>();






    }

    void Update()
    {




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
            if (selected == 0)
            {
                if (horizontalInput > 0.1)
                {

                    IncreaseSlider(sliderButtons[0]);
                    CalculateAudioValue(sliderButtons[0]);
                }
                else if (horizontalInput < -0.1)
                {
                    DecreaseSlider(sliderButtons[0]);
                    CalculateAudioValue(sliderButtons[0]);

                }
            }
            else if(selected == 1)
            {
                if (horizontalInput > 0.1)
                {

                    IncreaseSlider(sliderButtons[1]);
                    CalculateAudioValue(sliderButtons[1]);
                }
                else if (horizontalInput < -0.1)
                {
                    DecreaseSlider(sliderButtons[1]);
                    CalculateAudioValue(sliderButtons[1]);

                }
            }
            else if (selected == 2)
            {
                if (horizontalInput > 0.1)
                {

                    IncreaseSlider(sliderButtons[2]);
                    CalculateAudioValue(sliderButtons[2]);
                }
                else if (horizontalInput < -0.1)
                {
                    DecreaseSlider(sliderButtons[2]);
                    CalculateAudioValue(sliderButtons[2]);

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
        if (sliderButton.transform.localPosition.x <= sliderMax.localPosition.x)
        {
            sliderButton.transform.localPosition += new Vector3(horizontalInput * rateOfSlider, 0, 0) * Time.deltaTime;
        }
    }

    void DecreaseSlider(GameObject sliderButton)
    {
        if (sliderButton.transform.localPosition.x >= sliderMin.localPosition.x)
        {
            sliderButton.transform.localPosition += new Vector3(horizontalInput * rateOfSlider, 0, 0) * Time.deltaTime;
        }
    }

    void CalculateAudioValue(GameObject sliderButton)
    {
    

        maxValueForSlider = Mathf.Abs(sliderMin.localPosition.x) + Mathf.Abs(sliderMax.localPosition.x);

        Debug.Log(maxValueForSlider + " Max Value For Slider");
        currentValueForSlider = (sliderButton.transform.localPosition.x - (sliderMin.localPosition.x));
        Debug.Log(currentValueForSlider + " Current Value Of Slider");

        sliderPercentDecimal = currentValueForSlider / maxValueForSlider;

        audioValue = Mathf.Ceil(sliderPercentDecimal * -100);

        Debug.Log(audioValue + " Calculated Audio Level");


        if (audioValue < 0)
        {
            audioValue = 0;
        }
        else if (audioValue > 100)
        {
            audioValue = 100;
        }

      


    }


}


