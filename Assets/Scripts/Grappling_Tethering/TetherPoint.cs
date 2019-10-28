using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TetherPoint : MonoBehaviour, Interact
{

    public Material tetherMat;


    public GrappleComponent grappleComp;

    private void Start()
    {
        PlayerClass player = FindObjectOfType<PlayerClass>();
        grappleComp = player.GetGrappleComponent();
        tetherMat.color = Color.gray;


    }


    public Transform GetTetherLocation()
    {
        return transform;
    }

    public void DontInteractWithMe()
    {
        //if (grappleComp != null)
        //{
        //    if(grappleComp.tetherPoint == this.transform)
        //    {
        //    grappleComp.setTetherPoint(null);
        //    grappleComp.isStaring = false;
        //    }
        //    tetherMat.color = Color.gray;
        //}

    }

    public void InteractWithMe()
    {

        //if (grappleComp != null)
        //{

        //    tetherMat.color = Color.red;
        //    grappleComp.setTetherPoint(transform);
        //    grappleComp.isStaring = true; 



        //}

    }


}