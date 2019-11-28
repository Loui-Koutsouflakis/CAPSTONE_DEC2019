using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether_Trigger_Test : MonoBehaviour
{
    Animator anim1;
    Animator anim2;

    public GameObject p1;
    public GameObject p2;
    public GameObject iceCube;
    // Start is called before the first frame update
    void Start()
    {
        anim1 = p1.GetComponent<Animator>();
        anim2 = p2.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.layer == 14)
        {
            iceCube.SetActive(false);
            anim1.SetTrigger("Drop");
            anim2.SetTrigger("Drop2");
        }
    }
}
