using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatCodeHandler : MonoBehaviour
{
    private string[] levelOneCheatCode;
    private string[] levelTwoCheatCode;
    private string[] levelThreeCheatCode;
    private string[] levelFourCheatCode;
    private string[] godModeCheatCode;

    private int indexOne;
    private int indexTwo;
    private int indexThree;
    private int indexFour;
    private int indexFive;


    private bool godMode = false;

    private MainMenuAudioManager audioManager;
    private PlayerClass player;

    // Start is called before the first frame update
    void Start()
    {
        indexOne = 0;
        indexTwo = 0;
        indexThree = 0;
        indexFour = 0;

        levelOneCheatCode = new string[] { "l", "e", "v", "e", "l", "1" };
        levelTwoCheatCode = new string[] { "l", "e", "v", "e", "l", "2" };
        levelThreeCheatCode = new string[] { "l", "e", "v", "e", "l", "3" };
        levelFourCheatCode = new string[] { "l", "e", "v", "e", "l", "4" };
        godModeCheatCode = new string[] { "g", "o", "d", "m", "o", "d","e" };


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            
            if (Input.GetKeyDown(levelOneCheatCode[indexOne]))
            {

                indexOne++;
            }
           
            else
            {
                indexOne = 0;
            }


            if (Input.GetKeyDown(levelTwoCheatCode[indexTwo]))
            {

                indexTwo++;
            }

            else
            {
                indexTwo = 0;
            }

            if (Input.GetKeyDown(levelThreeCheatCode[indexThree]))
            {

                indexThree++;
            }

            else
            {
                indexThree = 0;
            }

            if (Input.GetKeyDown(levelFourCheatCode[indexFour]))
            {

                indexFour++;
            }

            else
            {
                indexFour = 0;
            }

            if (Input.GetKeyDown(godModeCheatCode[indexFive]))
            {

                indexFive++;
            }

            else
            {
                indexFive = 0;
            }

        }

        if (indexOne == levelOneCheatCode.Length)
        {
            if(GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
            }
            SceneManager.LoadSceneAsync(1);
            indexOne = 0;
        }

        if (indexTwo == levelTwoCheatCode.Length)
        {
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
            }
            SceneManager.LoadSceneAsync(2);
            indexTwo = 0;
        }

        if (indexThree == levelThreeCheatCode.Length)
        {
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
            }
            SceneManager.LoadSceneAsync(3);
            indexThree = 0;
        }

        if (indexFour == levelFourCheatCode.Length)
        {
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
            }
            SceneManager.LoadSceneAsync(4);
            indexFour = 0;
        }

        if (indexFour == levelFourCheatCode.Length)
        {
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
            }
            SceneManager.LoadSceneAsync(4);
            indexFour = 0;
        }

        if (indexFive == godModeCheatCode.Length)
        {
            if (GameObject.FindGameObjectWithTag("AudioManager"))
            {
                audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<MainMenuAudioManager>();
                audioManager.PlaySound("UnPause");
            }

            if(godMode)
            {
                godMode = false;
            }
            else
            {
                godMode = true;
            }

            indexFive = 0;
        }

        if(GameObject.FindObjectOfType<PlayerClass>())
        {
            player = GameObject.FindObjectOfType<PlayerClass>();
            if(!godMode)
            {
                player.SetDamagable(true);
                
            }
            else
            {
                player.SetDamagable(false);
                player.SetHealth(3);
                player.GetHManager().HealthFull();
            }
            Debug.Log(player.GetDamagable());
        }
    }
}
