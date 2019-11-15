using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera/Camera Path", 1)]
public class CameraPaths : MonoBehaviour
{
    PlayerClass player;
    public bool active = false;
    public CPC_CameraPath path;
    public CPC_CameraPath[] paths;
    public float time = 10;
    public float stayTime;

    [Header("If the object is collection based")]
    [Tooltip("Crystal threshold")]
    public float c_Threshold;

    [Header("Animations?")]
    [Tooltip("If your path requires an animation")]
    public Animation anims;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerClass>();
        paths = FindObjectsOfType<CPC_CameraPath>();
        if (anims == null)
            Debug.Log("No big deal");
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

        if (Vector3.Distance(player.gameObject.transform.position, transform.position) < 1 && this.gameObject.tag == "Obelisk")
            if (Vector3.Distance(player.gameObject.transform.position, transform.position) < .2f)
            {
                if (player.GetShards() >= c_Threshold)
                {
                    active = true;
                    if (anims != null)
                        anims.Play();
                }
            }
    }

    IEnumerator goTime()
    {
        active = false;
        yield return new WaitForEndOfFrame();
        path.PlayPath(time,stayTime);
    }
}
