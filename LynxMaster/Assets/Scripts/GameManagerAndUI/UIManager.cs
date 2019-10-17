using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private SaveGameManager saveGameManager;
    public LevelLoader LoadLevel;
    public Animation scrollOpen;
    public void StartGame()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
        saveGameManager.loading = true;
        LoadLevel.LoadLevel(1);
    }

    public void Continue()
    {
        if(GameObject.FindGameObjectWithTag("SaveGameManager"))
        {
            saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>();
            if(PlayerPrefs.GetInt("HasData_1") == 1)
            {
                saveGameManager.loading = true;
                saveGameManager.loadingFromContinue = true;
                LoadLevel.LoadLevel(PlayerPrefs.GetInt("CurrentScene_1"));
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
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();

    }


    public static UIManager singleton;

    private void Awake()
    {
        singleton = this; 
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
