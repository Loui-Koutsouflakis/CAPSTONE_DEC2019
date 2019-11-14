using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera/Camera Path", 1)]
public class CameraPaths : MonoBehaviour
{

    public bool active = false;
    public CPC_CameraPath path;
    public CPC_CameraPath[] paths;
    public float time = 10;
    // Start is called before the first frame update
    void Start()
    {
        paths = FindObjectsOfType<CPC_CameraPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            foreach (var item in paths)
            {
                item.gameObject.SetActive(false);
            }
            path.gameObject.SetActive(true);
            StartCoroutine(goTime());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        active = true;
    }

    IEnumerator goTime()
    {
        active = false;
        yield return new WaitForEndOfFrame();
        path.PlayPath(time);
    }
}
