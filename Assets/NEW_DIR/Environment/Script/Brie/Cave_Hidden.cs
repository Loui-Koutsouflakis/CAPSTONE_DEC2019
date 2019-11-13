using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave_Hidden : MonoBehaviour
{
    public GameObject hiddenWall;
    public GameObject secretPlats;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = hiddenWall.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            anim.SetTrigger("HideWall");
            secretPlats.SetActive(true);
        }
    }
}
