using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMainMenu : UIManager
{
    
    public GameObject[] MenuItems = new GameObject[4];
     MenuButtons[] buttons = new MenuButtons[4];
    [SerializeField]
    int selected = 0;
    public Material whenSelected;
    public Material whenNotSelected;
    [Range(0f,1f)]
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
        StartCoroutine(AnimationDelay(animationDelayTime));
    }

    private void OnEnable()
    {
        StartCoroutine(AnimationDelay(animationDelayTime));
    }

    void InitilizeButtons()
    {


        MenuItems[0].AddComponent<Button_StartGame>();
        buttons[0] = MenuItems[0].GetComponent<Button_StartGame>();
        MenuItems[1].AddComponent<Button_Continue>();
        buttons[1] = MenuItems[1].GetComponent<Button_Continue>(); 
        MenuItems[2].AddComponent<Button_Settings>();
        buttons[2] = MenuItems[2].GetComponent<Button_Settings>();
        MenuItems[3].AddComponent<Button_QuitGame>();
        buttons[3] = MenuItems[3].GetComponent<Button_QuitGame>();


        

      
    }

    void Update()
    {

      



        verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");
        if(canInteractWithButtons)
        {

            for (int i = 0; i < MenuItems.Length; i++)
            {
                if(selected == i)
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
        if(GameObject.FindGameObjectWithTag("AudioManager"))
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
            audioManager.PlaySound("Navigate");
        }
        yield return new WaitForSeconds(bufferTime);
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
        yield return new WaitForSeconds(bufferTime);
        recieveInput = true;
    }

    private IEnumerator AnimationDelay(float delay)
    {

        canInteractWithButtons = false;
        yield return new WaitForSeconds(delay);
        canInteractWithButtons = true;
        
    }

    public void SetCanInteractWithButtons(bool newBool)
    {
        canInteractWithButtons = newBool;
    }


}
