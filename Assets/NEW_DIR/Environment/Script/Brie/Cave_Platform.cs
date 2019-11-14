using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave_Platform : MonoBehaviour
{
    public GameObject cavePlats;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = cavePlats.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.layer == 14)
        {
            anim.SetTrigger("CavePlat");
        }
    }
}
