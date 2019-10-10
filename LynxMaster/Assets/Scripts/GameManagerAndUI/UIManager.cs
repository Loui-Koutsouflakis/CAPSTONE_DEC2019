using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public LevelLoader LoadLevel;
    public void StartGame()
    {
        LoadLevel.LoadLevel(1);
    }

    public void Setting()
    {
        Debug.Log("GameStarted");
    }

    public void QuitGame()
    {
        Debug.Log("GameStarted");
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
