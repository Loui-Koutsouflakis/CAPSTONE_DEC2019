using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskCollector : MonoBehaviour 
{
    PlayerClass player;
    public CPC_CameraPath path;
    public CPC_CameraPath[] paths;

    public float c_Threshold;
    bool didIt = false;
    bool stopit = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerClass>();
        paths = FindObjectsOfType<CPC_CameraPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopit) {
            if (Vector3.Distance(player.gameObject.transform.position, transform.position) < .2f)
            {
                if (player.GetShards() >= c_Threshold)
                {
                StartCoroutine(PlayAnim());
                }
            }
        }
    }

    IEnumerator PlayAnim()
    {
        yield return new WaitForEndOfFrame();
        //play animation
        stopit = true;
    }
}
