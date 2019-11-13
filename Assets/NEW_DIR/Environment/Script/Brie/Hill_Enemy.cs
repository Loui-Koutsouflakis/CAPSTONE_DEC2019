using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hill_Enemy : MonoBehaviour
{
    public GameObject toFloating;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            toFloating.SetActive(true);
        }
    }
}
