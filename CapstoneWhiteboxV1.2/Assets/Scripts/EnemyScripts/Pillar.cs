using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public Vector3 PillarShrinkAmount;
    public GameObject PillarGraphics;
    public GameObject PillarCycloneEnemy;
    public int PillarAttachedRing;

    private CycloneAttack PillarRingAccess;

    void Start()
    {
        PillarRingAccess = PillarCycloneEnemy.GetComponent<CycloneAttack>();
    }

    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Player" && PillarGraphics.transform.localScale.y > 0)
        {
            PillarGraphics.transform.localScale -= PillarShrinkAmount;
        }

        if(PillarGraphics.transform.localScale.y <= 0 && PillarRingAccess.c_Rings[PillarAttachedRing] != null &&
            PillarRingAccess.c_Rings[PillarAttachedRing] == true)
        {
            PillarRingAccess.c_Rings[PillarAttachedRing].SetActive(false);
        }
    }

}
