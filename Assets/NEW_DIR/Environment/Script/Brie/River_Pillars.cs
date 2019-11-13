using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River_Pillars : MonoBehaviour
{
    public GameObject pillars;
    public GameObject ice;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = pillars.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            anim.SetTrigger("RiverPillars");
            ice.SetActive(false);
        }
    }
}
