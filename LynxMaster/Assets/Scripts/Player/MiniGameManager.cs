using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : PlayerInputHandler
{
    [System.Serializable]
    public enum Game { None, Tap, Press };
    [SerializeField]
    Game whichGame;
    [Header("Player World UI")]
    GameObject player;
    public float score;

    public bool playing = false;
    bool exclaimer = false;
    //Attack Tap MiniGame
    [Header("Tap to Attack Game")]
    public Slider a_TapSlider;
    public Text mt_Exclaim;
    public Image a_Fill;
    float attackMax = 3;
    float attackMin = 0;

    [Header("Press to Attack Game")]
    public Slider p_Slider;
    public Text p_Exclaim;
    public Image p_Button;
    Vector3 pb_Size;
    float ps_Max = 20;
    float ps_Min = 0;
    float ps_TargetValue;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pb_Size = p_Button.transform.localScale;
        whichGame = Game.None;
        a_TapSlider.maxValue = attackMax;
        a_TapSlider.minValue = attackMin;
        p_Slider.maxValue = ps_Max;
        p_Slider.minValue = ps_Min;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().spiderWebs >= 3 && !playing)
        {
            playing = true;
            exclaimer = true;
            StartCoroutine(GameStart(5, mt_Exclaim, a_TapSlider, Game.Tap));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !playing)
        {
            playing = true;
            exclaimer = true;
            StartCoroutine(GameStart(5, p_Exclaim, p_Slider, Game.Press));
        }
        switch (whichGame)
        {
            case Game.None:
                score = 0;
                break;
            case Game.Tap:
                AttackTap();
                break;
            case Game.Press:
                AttackPress();
                break;
            default:
                break;
        }
    }

    void AttackTap()
    {
        if (exclaimer)
        {
            mt_Exclaim.text = "Tap Jump!!";
            TextColorChanger(mt_Exclaim);
            if (a_TapSlider.value > 0)
            {
                a_TapSlider.value -= 0.2f * Time.deltaTime;
            }
            if (Input.GetButtonDown("AButton"))
            {
                a_TapSlider.value += 0.2f;
                score = a_TapSlider.value;
            }
            if (a_TapSlider.value < 1)
            {
                a_Fill.color = Color.red;
            }
            if (a_TapSlider.value >= 1 && a_TapSlider.value < 1.8f)
            {
                a_Fill.color = Color.yellow;
            }
            if (a_TapSlider.value >= 1.8f && a_TapSlider.value < 2.5f)
            {
                a_Fill.color = Color.green;
            }
            if (a_TapSlider.value >= 2.5f)
            {
                a_Fill.color = Color.blue;
            }
        }
    }

    void AttackPress()
    {
        if (exclaimer)
        {
            p_Exclaim.text = "Tap B When the button Turns Green!";
            TextColorChanger(p_Exclaim);
            p_Slider.value = Mathf.Lerp(0, 20, Mathf.PingPong(Time.time, 1));
            if (p_Slider.value >= 8f && p_Slider.value <= 12f)
            {
                p_Button.color = Color.green;
                p_Button.transform.localScale = Vector3.Lerp(p_Button.transform.localScale, pb_Size * 3.5f, 10 * Time.deltaTime);
                if (Input.GetButtonDown("AButton"))
                {
                    score += 1;
                }
            }
            else if (p_Slider.value < 8 || p_Slider.value > 12)
            {
                p_Button.color = Color.red;
                p_Button.transform.localScale = Vector3.Lerp(p_Button.transform.localScale, pb_Size, 4 * Time.deltaTime);
                if (Input.GetButtonDown("AButton"))
                {
                    score -= 1;
                }
            }
        }
    }

    void TextColorChanger(Text t)
    {
        t.color = Color.Lerp(Color.yellow, Color.green, Mathf.PingPong(Time.time, 1f));
    }

    IEnumerator GameStart(float t, Text g, Slider s, Game j)
    {
        player.GetComponentInChildren<PlayerController>().paused = true;
        s.gameObject.SetActive(true);
        whichGame = j;
        yield return new WaitForSeconds(t);
        playing = false;
        StartCoroutine(GameEnd(g, score, j, s));
    }

    IEnumerator GameEnd(Text t, float s, Game g, Slider l)
    {
        switch (g)
        {
            case Game.Tap:
                exclaimer = false;
                if (s < 1)
                {
                    t.text = "Oops!";
                    t.color = Color.red;
                }
                if (s >= 1 && s < 1.8f)
                {
                    t.text = "Good!";
                    player.GetComponent<PlayerController>().spiderWebs = 0;
                    t.color = Color.yellow;
                }
                if (s >= 1.8f && s < 2.5f)
                {
                    t.text = "Great!";
                    t.color = Color.green;
                    player.GetComponent<PlayerController>().spiderWebs = 0;

                }
                if (s >= 2.5f && s <= 3)
                {
                    t.text = "perfect!";
                    t.color = Color.blue;
                    player.GetComponent<PlayerController>().spiderWebs = 0;
                }
                yield return new WaitForSeconds(1.5f);
                l.value = 0;
                l.gameObject.SetActive(false);
                whichGame = Game.None;
                player.GetComponentInChildren<PlayerController>().paused = false;
                break;

            case Game.Press:
                exclaimer = false;
                if (s <= 1)
                {
                    t.text = "Oops!";
                    t.color = Color.red;
                }
                if (s > 1 && s < 3)
                {
                    t.text = "Good!";
                    t.color = Color.yellow;
                }
                if (s == 3)
                {
                    t.text = "Great!";
                    t.color = Color.green;
                }
                if (s == 4)
                {
                    t.text = "Perfect!";
                    t.color = Color.blue;
                }
                yield return new WaitForSeconds(1.5f);
                l.value = 0;
                l.gameObject.SetActive(false);
                whichGame = Game.None;
                player.GetComponentInChildren<PlayerController>().paused = false;
                break;
            default:
                break;
        }
    }
}
