using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    public GameObject frontSpawn;
    public List<GameObject> fireSkills = new List<GameObject>();

	void Update ()
    {
        UseSkill();
	}

    void UseSkill()
    {
        if ( Input.GetKeyUp(KeyCode.Alpha1))
        {
            Instantiate(fireSkills[0], frontSpawn.transform.position, frontSpawn.transform.rotation);
        }
    }
}
