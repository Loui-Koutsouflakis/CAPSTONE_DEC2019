using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating_Rotation : MonoBehaviour
{
    public GameObject floater;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = floater.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            anim.SetTrigger("Floater");
        }
    }
}
