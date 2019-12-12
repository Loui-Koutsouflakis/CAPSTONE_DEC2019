using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderlingAnimController : MonoBehaviour
{
    public Spiderlings spiderlings;
    private Animator anim;

    public void Step()
    {
        spiderlings.Step();
    }

    public void TrapPlayer(bool isTrapped)
    {
        GetComponent<Animator>().SetBool("TrapPlayer", isTrapped);
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
