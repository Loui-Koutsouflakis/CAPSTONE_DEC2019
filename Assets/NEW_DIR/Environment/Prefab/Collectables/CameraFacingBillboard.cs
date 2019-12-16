using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    [SerializeField]
    Camera m_Camera;
    [SerializeField]
    bool isPlant = false;
    Renderer rend;
    Material mat;
    private void Awake()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }

        if (isPlant) { rend = GetComponent<Renderer>();}
    }
    private void Start()
    {
        if (isPlant)
        {
            rend.material.renderQueue = 2256;
          //  rend.material.mainTextureScale = new Vector3(Random.Range(.3f, 1), Random.Range(.3f, 1), Random.Range(.3f, 1));
        }
    }
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        if (isPlant)
        {
            Quaternion m_cam = m_Camera.transform.rotation;
            m_cam.z = 0;
            m_cam.x = 0;
            transform.LookAt(transform.position + m_cam * Vector3.forward,
                m_Camera.transform.rotation * -Vector3.up);
        }
        else
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }
    }
}