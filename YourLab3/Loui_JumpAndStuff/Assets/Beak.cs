using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beak : MonoBehaviour
{
    //Move playerScript;

    //Vector3 beakOut = new Vector3(0, 0, 1f);
    //Vector3 beakIn;
    //Vector3 initScale;
    //Vector3 scaleDifference;

    //int health = 3;
    //int beak = 5;
    //float erpolate = 0.22f;
    //bool peckin;

    //private void Start()
    //{
    //    peckin = false;
    //    beakIn = transform.localPosition;
    //    initScale = transform.localScale;
    //    scaleDifference = initScale * 1.4f;
    //    playerScript = GameObject.Find("Player").GetComponent<Move>();
    //}

    //private void Update()
    //{
    //    if(Input.GetButtonDown("BeakJ"))
    //    {
    //        peckin = true;

    //        if(!playerScript.grounded)
    //        {
    //            playerScript.Dive();
    //        }
    //    }

    //    if(Input.GetButtonUp("BeakJ"))
    //    {
    //        peckin = false;
    //    }

    //    if(peckin)
    //    {
    //        transform.localPosition = Vector3.Lerp(transform.localPosition, beakOut, erpolate);
    //        transform.localScale = Vector3.Lerp(transform.localScale, scaleDifference, erpolate);
    //    }

    //    if(!peckin)
    //    {
    //        transform.localPosition = Vector3.Lerp(transform.localPosition, beakIn, erpolate);
    //        transform.localScale = Vector3.Lerp(transform.localScale, initScale, erpolate);
    //    }
    //}
}
