using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum GameState { play, pause, end };
    public GameState currentState = GameState.play;
    public static GameManager Instance;
    public GameObject pauseUI;
    public GameObject player;
    public GameObject endUI;
    public GameObject startUI;
    public bool endGame;

    public static GameManager GetInstance()
    {
        return Instance;
    }

    void Start() {

        DontDestroyOnLoad(gameObject);

        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.play:
                Play();
                break;
            case GameState.pause:
                Pause();
                break;
            case GameState.end:
                End();
                break;
          
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == GameState.play)
            {
                currentState = GameState.pause;
            }
            else if (currentState == GameState.pause)
            {
                pauseUI.SetActive(false);
                currentState = GameState.play;
            }

        }

        if (Instance.endGame)
        {
            if (currentState == GameState.play)
            {
                currentState = GameState.end;
            }
        }
    }

    public void Play()
    {
        endUI.SetActive(false);
        Instance.enabled = true;
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    void Pause()
    {
        Instance.enabled = false;
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    void End()
    {
        Instance.enabled = false;
        endGame = true;
        endUI.SetActive(true);
        Time.timeScale = 0;
    }

  
    public void StartGame()
    {
        startUI.SetActive(false);
    }
    public void BackToMain()
    {
        startUI.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
