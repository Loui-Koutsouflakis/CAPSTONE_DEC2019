using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{

    private HandleSfx footSteps;
    // Start is called before the first frame update
    void Start()
    {
        footSteps = GetComponent<HandleSfx>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFootSteps()
    {
        footSteps.PlayRandomClip(0, 1);
    }

    public void PlayShuffle()
    {
        footSteps.PlayRandomClip(2, 4);
    }
}
