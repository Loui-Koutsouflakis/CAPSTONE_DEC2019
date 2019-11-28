using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    public GameObject wall;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = wall.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider o)
    {
        if (o.gameObject.layer == 14)
        {
            anim.SetTrigger("IceWall");
        }
    }
}
