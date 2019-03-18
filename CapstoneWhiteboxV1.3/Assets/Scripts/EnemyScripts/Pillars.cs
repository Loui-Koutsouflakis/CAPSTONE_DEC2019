using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillars : MonoBehaviour
{
    public GameObject PillarEnemy;
    public GameObject PillarGraphic;
    public Vector3 PillarShrinkSize;
    public int PillarRingMatch;

    void Start()
    {

    }

    void Update()
    {
        if(PillarGraphic.transform.localScale.y <= 0)
        {
            PillarEnemy.GetComponent<CycloneAttack>().c_Rings[PillarRingMatch].SetActive(false);
            gameObject.SetActive(false);
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Player" && PillarGraphic.transform.localScale.y > 0)
        {
            PillarGraphic.transform.localScale -= PillarShrinkSize;
        }
    }
}
