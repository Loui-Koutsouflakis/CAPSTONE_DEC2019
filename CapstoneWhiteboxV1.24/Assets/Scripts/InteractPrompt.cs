using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    //TODO: Currently seems to load text backwards and low. Next Update will fix.

    public GameObject promptPrefab;
    public GameObject followingObject;
    GameObject instancedPrompt;

    public float rayDistance = 5;


    bool instance = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        gameObject.transform.LookAt(followingObject.transform);

        //Could be optimized by enabling/disabling an already existing text instead of creating/destroying one.

        // Bit shift the index of the layer (9) to get a bit mask (Change to whatever layer the player is set to)
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            if(instance == false)
            {
                CreatePrompt();
                instance = true;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 2, Color.white);
            Debug.Log("Did not Hit");
            instance = false;
            Destroy(instancedPrompt);
        }


    }
    //Create an Object on 
    void CreatePrompt()
    {
        instancedPrompt = Instantiate(promptPrefab, gameObject.transform.position, gameObject.transform.rotation);
    }
}
