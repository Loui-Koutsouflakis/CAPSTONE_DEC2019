// Written By Benjamin Young October 16/2019.  Last Updated December 9/2019
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
    public Animator GameOverMenuAnimator;
    public Animator buttonLayoutMenuAnimator;
    public Animator keyboardLayoutMenuAnimator;

    //Control the different buttons
    [Header("Menu Navigation")]
    public NewMainMenu MainMenuButtonController;
    public SettingsNavigationManager settingsButtonController;
    public VideoNavigationManager videoButtonController;
    public AudioNavigationManager audioButtonController;
    public ControlsNavigationManager controlsButtonController;
    public PauseMenuNavigationManager pauseButtonController;
    public GameOverMenuNavigationManager gameOverButtonController;


    //The Game Objects that have the scrolls
    [Header("Scroll Game Objects")]
    public GameObject settingsScrollObject;
    public GameObject mainMenuScrollObject;
    public GameObject videoScrollObject;
    public GameObject audioScrollObject;
    public GameObject controlsScrollObject;
    public GameObject pauseMenuScrollObject;
    public GameObject gameOverScrollObject;

    private TransitionManager transManager;
    


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
        if (GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            if (PlayerPrefs.GetInt("HasData_1") == 1)
            {
                mainMenuAnimator.SetTrigger("Close");
                StartCoroutine(AnimationDelayContinue(animDelay));
            }
            else
            {


            }
        }


    }

    public void Setting()
    {


        StartCoroutine(AnimationDelaySettings(animDelay));
        mainMenuAnimator.SetTrigger("Close");



    }

    public void QuitGame()
    {
        mainMenuAnimator.SetTrigger("Close");


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
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        StartCoroutine(AnimationDelayControlsBack(animDelay));
        saveGameManager.LoadSettings();
        controlsMenuAnimator.SetTrigger("ControlsClose");
        //buttonLayoutMenuAnimator.SetTrigger("LayoutClosed");
        //keyboardLayoutMenuAnimator.SetTrigger("LayoutClosed");
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

    }

    public void ControlsStandardButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.SetCameraSmoothing(0.202f);
        saveGameManager.SaveSettings();

    }

    public void ControlsVeteranButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.SetCameraSmoothing(0.609f);
        saveGameManager.SaveSettings();

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


    }

    public void VideoMediumButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        QualitySettings.SetQualityLevel(3, true);
        saveGameManager.SetQualitySettings(3);
        saveGameManager.SaveSettings();


    }

    public void VideoLowButton()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        QualitySettings.SetQualityLevel(1, true);
        saveGameManager.SetQualitySettings(1);
        saveGameManager.SaveSettings();


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

    public void PauseSettingsButton()
    {
        pauseMenuAnimator.SetTrigger("PauseClose");
        StartCoroutine(AnimationDelayPauseMenuSettings(animDelay));
    }

    public void PauseSettingsBackButton()
    {
        settingsMenuAnimator.SetTrigger("SettingsClose");
        StartCoroutine(AnimationDelayPauseMenuSettingsBack(animDelay));
    }

    //GameOver Menu Functions
    public void GameOverRetry()
    {
        GameOverMenuAnimator.SetTrigger("GameOverClose");
        StartCoroutine(AnimationDelayGameOverRetry(animDelay));
    }
    public void GameOverMainMenu()
    {
        GameOverMenuAnimator.SetTrigger("GameOverClose");
        StartCoroutine(AnimationDelayGameOverMainMenu(animDelay));
    }




    public static UIManager singleton;

    private void Awake()
    {
        singleton = this;

    }

    private IEnumerator AnimationDelayStartGame(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.loadingFromContinue = false;
        saveGameManager.loadFixer = 0;
        saveGameManager.loading = true;
        LoadLevel.LoadLevel(1);

    }

    private IEnumerator AnimationDelayContinue(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        saveGameManager.loadingFromContinue = true;
        saveGameManager.loadFixer = 1;
        //Debug.Log(saveGameManager.loadingFromContinue + " Loading From Continue");
        //Debug.Log(saveGameManager.loadingFromContinue);
        saveGameManager.loading = true;
        LoadLevel.LoadLevel(PlayerPrefs.GetInt("CurrentScene_1"));

    }

    private IEnumerator AnimationDelaySettings(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsButtonController.SetCanInteractWithButtons(true);
        mainMenuScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayQuit(float delay)
    {

        MainMenuButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        Application.Quit();

    }

    private IEnumerator AnimationDelaySettingsBack(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        mainMenuScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);

        MainMenuButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayVideoBack(float delay)
    {

        videoButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);

        settingsButtonController.SetCanInteractWithButtons(true);
        videoScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayVideo(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        videoScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);
        videoButtonController.SetSelected(3);
        videoButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }


    private IEnumerator AnimationDelayAudio(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        audioScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);
        audioButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayAudioBack(float delay)
    {

        audioButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);

        settingsButtonController.SetCanInteractWithButtons(true);
        audioScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayControls(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        controlsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);
        controlsButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayControlsBack(float delay)
    {

        controlsButtonController.SetCanInteractWithButtons(false);
        controlsButtonController.sliderButton.SetActive(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);

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
            saveGameManager.increaseSmallShards(gameManager.GetPlayer().GetHManager().GetShards());
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

    private IEnumerator AnimationDelayPauseMenuSettings(float delay)
    {

        pauseButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);
        settingsButtonController.SetCanInteractWithButtons(true);
        pauseMenuScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayPauseMenuSettingsBack(float delay)
    {

        settingsButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        pauseMenuScrollObject.SetActive(true);
        yield return new WaitForSeconds(delay * Time.timeScale);
        pauseButtonController.SetCanInteractWithButtons(true);
        settingsScrollObject.SetActive(false);


    }

    private IEnumerator AnimationDelayGameOverRetry(float delay)
    {

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        transManager = GameObject.FindObjectOfType<TransitionManager>();
        //Debug.Log(transManager.name);

        if (GameObject.FindGameObjectWithTag("SaveGameManager") != null)
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        }

        gameOverButtonController.SetCanInteractWithButtons(false);
        transManager.StartCoroutine(transManager.BlinkSequence(2f, 0.5f, 1, 0.9f, true));
        yield return new WaitForSeconds(delay * Time.timeScale);
        gameOverScrollObject.SetActive(false);
        gameManager.ResetTimeAndCamera();
        gameManager.GetPlayer().SetHealth(3);
        gameManager.GetPlayer().GetHManager().HealthFull();
        gameManager.GetPlayer().UnDeath();
        saveGameManager.Load();
        yield return new WaitForSeconds(1.6f * Time.timeScale);
        gameManager.GetPlayer().EnableControls();

        //gameManager.GetPlayer().SetShards(saveGameManager.GetShards());

    }


    private IEnumerator AnimationDelayGameOverMainMenu(float delay)
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        saveGameManager.increaseSmallShards(gameManager.GetPlayer().GetHManager().GetShards());
        gameOverButtonController.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay * Time.timeScale);
        gameManager.GoToMenu();

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
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.StartGame(); }
}

public class Button_Continue : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.Continue(); }
}

public class Button_Settings : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.Setting(); }
}

public class Button_QuitGame : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.QuitGame(); }
}

public class Button_Audio : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.AudioButton(); }
}

public class Button_Controls : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsButton(); }
}

public class Button_Video : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoButton(); }
}

public class Button_SettingsBack : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.SettingsBackButton(); }
}

public class Button_VideoBack : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.VideoBackButton(); }
}

public class Button_AudioBack : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.AudioBackButton(); }
}

public class Button_ControlsBack : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.ControlsBackButton(); }
}

public class Button_PauseMenuResume : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseResumeButton(); }
}

public class Button_PauseMenuQuit : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseQuitButton(); }
}

public class Button_PauseMenuMainMenu : MenuButtons
{
    public override void Name() { }

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

public class Button_PauseMenuSettings : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseSettingsButton(); }
}

public class Button_PauseMenuSettingsBack : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.PauseSettingsBackButton(); }
}

public class Button_GameOverMenuRetry : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.GameOverRetry(); }
}

public class Button_GameOverMenuMainMenu : MenuButtons
{
    public override void Name() { }

    public override void Execute(UIManager uiManager_m) { uiManager_m.GameOverMainMenu(); }
}