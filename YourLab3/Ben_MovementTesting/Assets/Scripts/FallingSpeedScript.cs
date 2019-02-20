using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpeedScript : MonoBehaviour
{
    public float lowJumpMultiplier;
    public float fallSpeedMultiplier;

    Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_rigidbody.velocity.y < 0)
        {
            m_rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
        }
        else if(m_rigidbody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            m_rigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
