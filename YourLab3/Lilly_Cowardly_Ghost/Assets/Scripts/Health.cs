using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;

	// Update is called once per frame
	void Update ()
    {
        if(health <= 0)
        {
            gameObject.SetActive(false);
        }	
	}

    public void TakeDMG(int dmg)
    {
        health = health - dmg;
    }
}
