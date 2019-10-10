using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMainMenu : MonoBehaviour
{
    public GameObject[] MenuItems;
    int selected = 0;
    public Material whenSelected;
    public Material whenNotSelected;
    [Range(0f,1f)]
    public float bufferTime;
    float verticalInput;
    bool recieveInput = true;
    void Start()
    {
        
    }

    void Update()
    {
        verticalInput = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        for (int i = 0; i < MenuItems.Length; i++)
        {
            if(selected == i)
            {
                MenuItems[i].gameObject.GetComponent<Renderer>().material = whenSelected;
            }
            else
            {
                MenuItems[i].gameObject.GetComponent<Renderer>().material = whenNotSelected;
            }
            
        }

        if(verticalInput > 0.9f && selected > 0 && recieveInput == true)
        {
            StartCoroutine("InputBufferSubract");
        }
        else if(verticalInput < -0.9f && selected < MenuItems.Length -1 && recieveInput == true)
        {
            StartCoroutine("InputBufferAdd");
        }
    }

    private IEnumerator InputBufferSubract()
    {
        recieveInput = false;
        selected--;
        yield return new WaitForSeconds(bufferTime);
        recieveInput = true;
    }

    private IEnumerator InputBufferAdd()
    {
        recieveInput = false;
        selected++;
        yield return new WaitForSeconds(bufferTime);
        recieveInput = true;
    }
}
