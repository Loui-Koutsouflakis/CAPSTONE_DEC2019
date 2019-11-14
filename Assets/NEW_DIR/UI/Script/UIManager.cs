using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private SaveGameManager saveGameManager;
    public LevelLoader LoadLevel;
    public Animator m_animator;
    public NewMainMenu mainMenu;
    public GameObject settingsUI;
    public GameObject scrollObject;
    [Range(0f, 2f)]
    public float animDelay;

    [SerializeField]
    private TransitionManager t_manager;

    public void StartGame()
    {
        m_animator.SetTrigger("Close");
        StartCoroutine(AnimationDelayStartGame(animDelay));
        t_manager.FadeOut();
        //saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        //saveGameManager.loading = true;
        //LoadLevel.LoadLevel(1);
    }

    public void Continue()
    {
        if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            if(PlayerPrefs.GetInt("HasData_1") == 1)
            {
                m_animator.SetTrigger("Close");
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
        m_animator.SetTrigger("Close");
       
        
        
    }

    public void QuitGame()
    {
        //m_animator.SetTrigger("Close");
        
        Debug.Log("Quit");
        //StartCoroutine(AnimationDelayQuit(animDelay));

    }


    public static UIManager singleton;

    private void Awake()
    {
        singleton = this; 
    }

    private IEnumerator AnimationDelayStartGame(float delay)
    {

        mainMenu.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.loading = true;
        LoadLevel.LoadLevel(1);

    }

    private IEnumerator AnimationDelayContinue(float delay)
    {

        mainMenu.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        saveGameManager.loading = true;
        saveGameManager.loadingFromContinue = true;
        LoadLevel.LoadLevel(PlayerPrefs.GetInt("CurrentScene_1"));

    }

    private IEnumerator AnimationDelaySettings(float delay)
    {

        mainMenu.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        settingsUI.SetActive(true);
        scrollObject.SetActive(false);

    }

    private IEnumerator AnimationDelayQuit(float delay)
    {

        mainMenu.SetCanInteractWithButtons(false);
        yield return new WaitForSeconds(delay);
        Application.Quit();

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

