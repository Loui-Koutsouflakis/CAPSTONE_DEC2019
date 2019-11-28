using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public GameObject walls;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = walls.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 15)
        {
            anim.SetTrigger("WallsUp");
        }
    }
}
