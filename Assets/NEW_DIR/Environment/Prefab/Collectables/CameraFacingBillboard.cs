using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    [SerializeField]
    Camera m_Camera;

    private void Awake()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
    }
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}