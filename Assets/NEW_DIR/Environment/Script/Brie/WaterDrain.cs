using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrain : MonoBehaviour
{
    public GameObject water;
    public GameObject caveDoor;
    public GameObject spikes;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = caveDoor.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            water.SetActive(false);
            spikes.SetActive(false);
            anim.SetTrigger("CaveDoor");
        }
    }
}
