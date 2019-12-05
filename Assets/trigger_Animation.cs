using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger_Animation : MonoBehaviour
{
    public CameraPaths camTrigger;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if(!anim)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(camTrigger.active)
        {
            anim.SetTrigger("Go");
        }
    }
}
