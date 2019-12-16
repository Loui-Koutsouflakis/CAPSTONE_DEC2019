//Made By Benjamin Young.  Purpose: Game Manager For Capstone Project. Last updated 21/11/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool GameIsPaused;

    //bool IsDead = false;

    public GameObject pauseMenuUI;

    public GameObject gameOverMenuUI;

    bool isMuted = false;

    public int PlayerHealth;

    PlayerClass player;

    private MainMenuAudioManager audioManager;

    public Animator pauseAnimController;

    private PauseMenuNavigationManager pauseNavigationManager;

    public GameObject pauseCamera;


    public Animator gameOverAnimController;

    private GameOverMenuNavigationManager gameOverNavigationManager;

    public GameObject GameOverCamera;


    // public Button[] pauseMenuButtons;

    // int selected = 0;

    //public float delay;
    //public float navigationDelay;

    // float timeSincePaused;

    // float verticalInput;

    // bool recieveInput;



    //public List<Image> Hearts = new List<Image>();

    //public AudioSource Music;

    //public GameObject UIElements;

    //public Slider Volume;

    //private float previousVolume;

    //Loading Screen Variables
    // public GameObject m_LoadScreen;
    // public Slider progressBar;
    // public Text progressPercent;

    void Awake()
    {
        //DontDestroyOnLoad(this);
        //MakeSingleton();
        GameIsPaused = false;

    }

    private void Start()
    {
        player = FindObjectOfType<PlayerClass>();
    }

    //void UpdateSelected(int number)
    //{
    //    if(number > 0)
    //    { 
    //    if (selected < pauseMenuButtons.Length)
    //        selected++;
    //    else
    //        selected = 0;
    //    }

    //    else
    //    { 
    //         if (selected > 0)
    //        selected--;
    //    else
    //        selected = 0;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {

        //ChangeQuality();

        //LoseHealth();

        //HealthBar();

        //MuteSound();

        //Sets the sound volume to the slider within the pause menu
        //if(!isMuted)
        //{
        //    Music.volume = Volume.value;
        //}

        //Quits the game
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Quit();
        //}

        //Unpauses the game
        //if(GameIsPaused)
        //{
        //    pauseMenuButtons[selected].Select();
        //    verticalInput = 0;
        //    verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical") * 1000;



        //    if (verticalInput < -0.9f && selected < pauseMenuButtons.Length - 1 && recieveInput)
        //    {
        //        Debug.Log("Error");
        //        UpdateSelected(1);

        //        StartCoroutine(InputBufferAdd());
        //    }
        //    else if (verticalInput > 0.9f && selected > 0 && recieveInput)
        //    {
        //        Debug.Log("Error");
        //        UpdateSelected(-1);

        //        StartCoroutine(InputBufferSubtract());

        //    }

        //    if (Input.GetButtonDown("AButton"))
        //    {
        //        pauseMenuButtons[selected].onClick.Invoke();
        //    }
        //}
        if (GameObject.FindObjectOfType<PauseMenuNavigationManager>())
        {
            pauseNavigationManager = GameObject.FindObjectOfType<PauseMenuNavigationManager>().GetComponent<PauseMenuNavigationManager>();


        }

        if (Input.GetKeyDown(KeyCode.Y) && pauseMenuUI.activeInHierarchy == false && GameIsPaused == false) 
        {
            Death();
        }


        if (Input.GetKeyDown(KeyCode.P) && GameIsPaused && pauseNavigationManager.GetCanInteractWithButtons() == true || Input.GetButtonDown("Start") && GameIsPaused && pauseNavigationManager.GetCanInteractWithButtons() == true || Input.GetKeyDown(KeyCode.Escape) && GameIsPaused && pauseNavigationManager.GetCanInteractWithButtons() == true)
        {
            
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
                
            }

            StartCoroutine(PauseMenuAnimDelay());
            if(GameObject.FindObjectOfType<PauseMenuNavigationManager>())
            {
                pauseNavigationManager = GameObject.FindObjectOfType<PauseMenuNavigationManager>().GetComponent<PauseMenuNavigationManager>();
                pauseNavigationManager.SetSelected(0);
            }
        }
        //Pauses the Game
        else if (Input.GetKeyDown(KeyCode.P) && !GameIsPaused && gameOverMenuUI.activeInHierarchy == false || Input.GetButtonDown("Start") && !GameIsPaused && gameOverMenuUI.activeInHierarchy == false)
        {
            pauseCamera.SetActive(true);
            
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("Pause");
            }
            Pause();
        }
    }

    
    //Makes sure that there is only ever one gameManager being used in a scence;
    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //Placeholder for quitting the application 
    public void Quit()
    {
        Time.timeScale = 1;
        //Debug.Log("Game is Quitting...");
        Application.Quit();

    }


    //simply changes the quality based on unitys standerds of Low, Medium and High quality
    //public void ChangeQuality()
    //{
    //    switch (Input.inputString)
    //    {
    //        case "1":
    //            QualitySettings.SetQualityLevel(0, true);
    //            Debug.Log("Quality set to Low");
    //            break;


    //        case "2":
    //            QualitySettings.SetQualityLevel(2, true);
    //            Debug.Log("Quality set to Medium");
    //            break;

    //        case "3":
    //            QualitySettings.SetQualityLevel(3, true);
    //            Debug.Log("Quality set to High");
    //            break;


    //    }
    //}

    //Controls the muting of sound in the scene
    //public void MuteSound()
    //{
    //    if(Input.GetKeyDown(KeyCode.M) && !isMuted)
    //    {
    //        isMuted = true;
    //        previousVolume = Music.volume;
    //        Music.volume = 0;
    //    }
    //    else if(Input.GetKeyDown(KeyCode.M) && isMuted)
    //    {

    //        Music.volume = previousVolume;
    //        isMuted = false;
    //    }
    //}
    //Enables the pause menu and sets timescale to 0
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        //UIElements.SetActive(false);
        player.DisableControls();
        Time.timeScale = 0.0001f;
        GameIsPaused = true;
        //timeSincePaused = 0;
        //recieveInput = true;
        //selected = 0;

    }
    //Resumes timescale and dissables the Pause menu
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        //UIElements.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.EnableControls();

    }

    //Placeholder way for me to test health loss
    //public void LoseHealth()
    //{
    //    if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        PlayerHealth--;
    //    }
    //}

    //Controlls the number of hearts on screen based on player health
    //public void HealthBar()
    //{

    //    if (PlayerHealth == 2)
    //    {
    //        Hearts[2].enabled = false;
    //    }

    //    else if (PlayerHealth == 1)
    //    {
    //        Hearts[1].enabled = false;
    //    }

    //    else if (PlayerHealth <= 0)
    //    {
    //        Hearts[0].enabled = false;
    //        Death();
    //        PlayerHealth = 3;
    //        IsDead = true;
    //    }
    //    //Restarts the game when the player dies
    //    else if (PlayerHealth == 3 && IsDead)
    //    {
    //        for (int i = 0; i < Hearts.Count ; i++)
    //        {
    //            Hearts[i].enabled = true;
    //        }
    //        IsDead = false;
    //    }
    //}

    //Placeholder for death 
    public void Death()
    {
        GameOverCamera.SetActive(true);
        gameOverMenuUI.SetActive(true);
        gameOverNavigationManager = GameObject.FindObjectOfType<GameOverMenuNavigationManager>().GetComponent<GameOverMenuNavigationManager>();
        gameOverNavigationManager.SetCanInteractWithButtons(true);
        player.DisableControls();
        Time.timeScale = 0.0001f;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        player.EnableControls();
        SceneManager.LoadScene(0);
    }

    public void ResetTimeAndCamera()
    {
        Time.timeScale = 1;
        GameOverCamera.SetActive(false);
    }

    //private IEnumerator InputDelay(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    player.GetComponent<PlayerController>().paused = false;
    //}

    //private IEnumerator InputBufferAdd()
    //{
    //    recieveInput = false;
    //    //selected++;
    //    yield return new WaitForSeconds(navigationDelay * Time.timeScale);
    //    recieveInput = true;
    //}

    //private IEnumerator InputBufferSubtract()
    //{
    //    recieveInput = false;
    //   // selected--;
    //    yield return new WaitForSeconds(navigationDelay * Time.timeScale);
    //    recieveInput = true;
    //}



    private IEnumerator PauseMenuAnimDelay()
    {
        pauseAnimController.SetTrigger("PauseClose");
        yield return new WaitForSeconds(1.4f * Time.timeScale);
        pauseCamera.SetActive(false);
        Resume();
    }

    public PlayerClass GetPlayer()
    {
        return player;
    }
}
