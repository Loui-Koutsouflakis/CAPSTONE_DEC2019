using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCheckPoint : MonoBehaviour
{

    private SaveGameManager manager;
    private PlayerClass player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 14)
        {
            manager = FindObjectOfType<SaveGameManager>().GetComponent<SaveGameManager>();        
            //manager.increaseSmallShards(player.GetShards());
            manager.updateCheckpoint(other.gameObject.transform);
            Debug.Log("Trigger Works");
        }
    }
}
