using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    private PlayerClass player;
    private HudManager hud;
    private HandleSfx sounds;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerClass>();
        hud = FindObjectOfType<HudManager>();
        sounds = GetComponent<HandleSfx>();
    }

    //void Update()
    //{
    //    
    //}

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 14 && player.GetHealth() < 3)
        {
            sounds.PlayOneShotByName("PickUp");
            hud.HealthUp();
            player.SetHealth(1);

            transform.parent.gameObject.SetActive(false);
        }
    }
}
