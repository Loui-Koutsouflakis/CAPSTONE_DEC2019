using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;       //Public variable to store a reference to the player game object


    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        offset = transform.position - player.transform.position;
    }
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
