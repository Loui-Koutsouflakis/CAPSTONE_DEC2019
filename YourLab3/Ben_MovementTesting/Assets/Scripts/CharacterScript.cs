using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    [Range(1,15)]
    public float jumpStrength;

    Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>(); 
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            m_rigidbody.velocity = Vector3.up * jumpStrength;
        }
    }
}
