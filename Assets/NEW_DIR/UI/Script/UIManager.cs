using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private SaveGameManager saveGameManager;
    private GameManager gameManager;
    public LevelLoader LoadLevel;

    //Animators For The Different Menus
    [Header("Animators")]
    public Animator mainMenuAnimator;
    public Animator settingsMenuAnimator;
    public Animator videoMenuAnimator;
    public Animator audioMenuAnimator;
    public Animator controlsMenuAnimator;
    public Animator pauseMenuAnimator;

    //Control the different buttons
    [Header("Menu Navigation")]
    public NewMainMenu MainMenuButtonController;
    public SettingsNavigationManager settingsButtonController;
    public VideoNavigationManager videoButtonController;
    public AudioNavigationManager audioButtonController;
    public ControlsNavigationManager controlsButtonController;
    public PauseMenuNavigationManager pauseButtonController;


    //The Game Objects that have the scrolls
    [Header("Scroll Game Objects")]
    public GameObject settingsScrollObject;
    public GameObject mainMenuScrollObject;
    public GameObject videoScrollObject;
    public GameObject audioScrollObject;
    public GameObject controlsScrollObject;
    

    //The Game Objects That Hold The Button Controller Scripts
   
    

    //Animation delay for the scrolls
    [Range(0f, 2f)]
    public float animDelay;
  
    //Functions that the individual button classes will call
    public void StartGame()
    {
        mainMenuAnimator.SetTrigger("Close");
        StartCoroutine(AnimationDelayStartGame(animDelay));
        
    }

    public void Continue()
    {
        if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            if(PlayerPrefs.GetInt("HasData_1") == 1)
            {
                mainMenuAnimator.SetTrigger("Close");
                StartCoroutine(AnimationDelayContinue(animDelay));
            }
            else
            {
                Debug.Log("No Data To Load");
                
            }
        }
        Debug.Log("Continue");

    }

    public void Setting()
    {
        
        Debug.Log("Settings");
        StartCoroutine(AnimationDelaySettings(animDelay));
        mainMenuAnimator.SetTrigger("Close");
       
        
        
    }

    public void QuitGame()
    {
        mainMenuAnimator.SetTrigger("Close");
        
        Debug.Log("Quit");
        StartCoroutine(AnimationDelayQuit(animDelay));

    }

    public void AudioButton()
    {
        StartCoroutine(AnimationDelayAudio(animDelay));
        settingsMenuAnimator.SetTrigger("SettingsClose");
    }

    public void SettingsBackButton()
    {
        StartCoroutine(AnimationDelaySettingsBack(animDelay));
        settingsMenuAnimator.SetTrigger("SettingsClose");
    }

    public void VideoBackButton()
    {
        StartCoroutine(AnimationDelayVideoBack(animDelay));
        videoMenuAnimator.SetTrigger("VideoClose");
    }

    public void AudioBackButton()
    {
        StartCoroutine(AnimationDelayAudioBack(animDelay));
        audioMenuAnimator.SetTrigger("AudioClose");
    }

    public void ControlsButton()
    {
        StartCoroutine(AnimationDelayControls(animDelay));
        
        settingsMenuAnimator.SetTrigger("SettingsClose");

    }

    public void ControlsBackButton()
    {
        StartCoroutine(AnimationDelayControlsBack(animDelay));
        controlsMenuAnimator.SetTrigger("ControlsClose");
    }

    public void ControlsInvertButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.setInverted();
        
    }

    public void ControlsClassicButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.SetCameraSmoothing(0f);
        saveGameManager.SaveSettings();
        Debug.Log(saveGameManager.GetCameraSmoothing());
    }

    public void ControlsStandardButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.SetCameraSmoothing(0.202f);
        saveGameManager.SaveSettings();
        Debug.Log(saveGameManager.GetCameraSmoothing());
    }

    public void ControlsVeteranButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.SetCameraSmoothing(0.609f);
        saveGameManager.SaveSettings();
        Debug.Log(saveGameManager.GetCameraSmoothing());
    }

    public void VideoButton()
    {
        StartCoroutine(AnimationDelayVideo(animDelay));
        settingsMenuAnimator.SetTrigger("SettingsClose");
    }

    public void VideoHighButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        QualitySettings.SetQualityLevel(5, true);
        saveGameManager.SetQualitySettings(5);
        saveGameManager.SaveSettings();
        Debug.Log("Set To High");

    }

    public void VideoMediumButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        QualitySettings.SetQualityLevel(3, true);
        saveGameManager.SetQualitySettings(3);
        saveGameManager.SaveSettings();
        Debug.Log("Set To Medium");

    }

    public void VideoLowButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        QualitySettings.SetQualityLevel(1, true);
        saveGameManager.SetQualitySettings(1);
        saveGameManager.SaveSettings();
        Debug.Log("Set To Low");

    }

    //Pause Menu Functions
    public void PauseResumeButton()
    {
        pauseMenuAnimator.SetTrigger("PauseClose");
        StartCoroutine(AnimationDelayPauseMenuResume(animDelay));
            
    }

    public void PauseQuitButton()
    {
        pauseMenuAnimator.SetTrigger("PauseClose");
        StartCoroutine(AnimationDelayPauseMenuQuit(animDelay));

    }

    public void PauseMainMenuButton()
    {
        pauseMenuAnimator.SetTrigger("PauseClose");
        StartCoroutine(AnimationDelayPauseMenuMainMenu(animDelay));

    }


    //Gets
    public static UIManager singleton;

    private void Awake()
    {
        singleton = this;
        
    }

    private IEnumerator AnimationDelayStartGame(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.loading = true;
        LoadLevel.LoadLevel(1);

    }

    private IEnumerator AnimationDelayContinue(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        saveGameManager.loading = true;
        saveGameManager.loadingFromContinue = true;
        LoadLevel.LoadLevel(PlayerPrefs.GetInt("CurrentScene_1"));

    }

    private IEnumerator AnimationDelaySettings(float delay)
    {
       
        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        settingsButtonController.SetCanInteractWithButtons(true);
        mainMenuScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayQuit(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        Application.Quit();

    }

    private IEnumerator AnimationDelaySettingsBack(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        mainMenuScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
       
        MainMenuButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayVideoBack(float delay)
    {

        videoButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        
        settingsButtonController.SetCanInteractWithButtons(true);
        videoScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayVideo(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        videoScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        videoButtonController.SetSelected(3);
        videoButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }


    private IEnumerator AnimationDelayAudio(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        audioScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        audioButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayAudioBack(float delay)
    {

        audioButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
      
        settingsButtonController.SetCanInteractWithButtons(true);
        audioScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayControls(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        controlsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        controlsButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayControlsBack(float delay)
    {

        controlsButtonController.SetCanInteractWithButtons(false);
        controlsButtonController.sliderButton.SetActive(false);
        yield return new WaitForSeconds(delay);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        
        settingsButtonController.SetCanInteractWithButtons(true);
        controlsScrollObject.SetActive(false);


    }


    private IEnumerator AnimationDelayPauseMenuResume(float delay)
    {

       
        pauseButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        if (GameObject.FindGameObjectWithTag("GameManager"))
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.pauseCamera.SetActive(false);
            gameManager.Resume();
        }


    }

    private IEnumerator AnimationDelayPauseMenuQuit(float delay)
    {

        pauseButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        if (GameObject.FindGameObjectWithTag("GameManager"))
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.Quit();
        }


    }

    private IEnumerator AnimationDelayPauseMenuMainMenu(float delay)
    {

        pauseButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        if (GameObject.FindGameObjectWithTag("GameManager"))
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.GoToMenu();
        }


    }




}

public class MenuButtons : MonoBehaviour
{

    public bool selected;
    public string m_name; 
    

    public virtual void Execute(UIManager uiManager_m) { }

    public virtual void Name() { }

}

public class Button_StartGame : MenuButtons
{
    public override void Name() { Debug.Log("Start"); }

    public override void Execute(UIManager uiManager_m) { uiManager_m.StartGame(); }
}

public class Button_Continue : MenuButtons
{
    public override void Name() { Debug.Log("Continue"); }

    public override void Execute(UIManager uiManager_m) { uiManager_m.Continue(); }
}

public class Button_Settings : MenuButtons
{
   public override void Name() { Debug.Log("Settings"); }

    public override void Execute(UIManager uiManager_m) { uiManager_m.Setting(); }
}

public class Button_QuitGame : MenuButtons
{
    public override void Name() { Debug.Log("Quit"); }

    public override void Execute(UIManager uiManager_m) { uiManager_m.QuitGame(); }
}

public class Button_Audio : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.AudioButton(); }
}

public class Button_Controls : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsButton(); }
}

public class Button_Video : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoButton(); }
}

public class Button_SettingsBack : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.SettingsBackButton(); }
}

public class Button_VideoBack : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoBackButton(); }
}

public class Button_AudioBack : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.AudioBackButton(); }
}

public class Button_ControlsBack : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsBackButton(); }
}

public class Button_PauseMenuResume : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseResumeButton(); }
}

public class Button_PauseMenuQuit : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseQuitButton(); }
}

public class Button_PauseMenuMainMenu : MenuButtons
{
    public override void Name() {  }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseMainMenuButton(); }
}

public class Button_VideoHigh : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoHighButton(); }
}

public class Button_VideoMedium : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoMediumButton(); }
}

public class Button_VideoLow : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoLowButton(); }
}

public class Button_ControlsInvert : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsInvertButton(); }
}

public class Button_ControlsClassic : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsClassicButton(); }
}

public class Button_ControlsStandard : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsStandardButton(); }
}

public class Button_ControlsVeteran : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsVeteranButton(); }
}