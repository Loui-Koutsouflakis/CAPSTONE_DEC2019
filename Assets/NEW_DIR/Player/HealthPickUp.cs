using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    private PlayerClass player;
    private HudManager hud;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerClass>();
        hud = FindObjectOfType<HudManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 14)
        {
            //sfx.PlayOneShotByName("Collect");
            hud.HealthUp();
            player.SetHealth(1);

            transform.parent.gameObject.SetActive(false);                   

        }
    }
}
