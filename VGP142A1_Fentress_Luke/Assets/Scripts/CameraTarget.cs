using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public GameObject player;
    private Transform playerLoc; 
   
    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    private void Update()
    {
        findLoc();
        FollowPlayer();

    }


    void FollowPlayer()
    {
        Vector3 targetPos = new Vector3(playerLoc.position.x, 5, playerLoc.position.z + 5);
        Vector3 startPos = transform.position;
        //transform.position = Vector3.Lerp(startPos, targetPos, 1);
        transform.position = targetPos; 
    }

    void findLoc()
    {
        playerLoc = player.transform;
    }
}
