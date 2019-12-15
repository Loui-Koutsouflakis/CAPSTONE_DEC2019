using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderlingAnimController : MonoBehaviour
{
    public AudioClip footFall;

    public Spiderlings spiderlings;
    public GameObject audioSourceObjects;

    private List<AudioSource> audioSource = new List<AudioSource>();
    private new AudioSource audio;
    private Animator anim;

    //public void Step()
    //{
    //    spiderlings.Step();
    //}

    public void TrapPlayer(bool isTrapped)
    {
        Debug.Log(anim + " in trapPlayer");
        GetComponent<Animator>().SetBool("TrapPlayer", isTrapped);
    }

    public void StepSound()
    {

        audio.pitch = Random.Range(1f, 2f);
        audio.Play();
    }

    public void Death()
    {
        GetComponent<Animator>().SetTrigger("Death");
    }

    public void Deactivate()
    {
        StartCoroutine(spiderlings.DeathDeactivate());
    }
    // Start is called before the first frame update
    void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = footFall;
        foreach (Transform child in audioSourceObjects.transform)
        {
            audioSource.Add(child.GetComponent<AudioSource>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null) anim = GetComponent<Animator>();

    }
}
