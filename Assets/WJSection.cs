using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WJSection : MonoBehaviour
{
    PlayerCamera Camera;

    public PlayerCamera.WallCamChoice wallCamChoice;
    public float distance;
    public float height;
    public float followLerp;
    // Start is called before the first frame update
    void Start()
    {
        Camera = FindObjectOfType<PlayerCamera>();
    }

    private void OnTriggerStay(Collider c)
    {
        if (c.gameObject.layer == 14 && !Camera.cinema_Playing)
        {
            Camera.SwitchToCinema(PlayerCamera.CameraType.SideScroll);
            Camera.wallCamChoice = wallCamChoice;
            Camera.Height2D = height;
            Camera.DistIn2D = distance;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 14) Camera.SwitchToCinema(PlayerCamera.CameraType.Orbit);
    }

}
