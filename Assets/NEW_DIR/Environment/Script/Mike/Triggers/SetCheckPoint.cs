using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCheckPoint : MonoBehaviour
{

    private SaveGameManager manager;
    private PlayerClass player;

    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 14 && this.enabled)
        {
            if (FindObjectOfType<SaveGameManager>())
            {
                manager = FindObjectOfType<SaveGameManager>().GetComponent<SaveGameManager>();
                //manager.increaseSmallShards(player.GetShards());
                manager.updateCheckpoint(other.gameObject.transform);
            }
            anim.Play();
            //Debug.Log("Trigger Works" + gameObject.name);
            this.enabled = false;
        }
    }
}
