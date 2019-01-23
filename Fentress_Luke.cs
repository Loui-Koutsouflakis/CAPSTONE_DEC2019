using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWoodAbility : Ability 
{
    public GameObject wood;
    public Transform spawnPoint;

    public float initialVelocity = 10;
    //public float maxTime = 5;
    //public float timer;



    

    public override void onUpdate()
    {
        base.onUpdate();


        if (Input.GetButtonDown("Fire1"))
        {
            GameObject spawnedProjectile = Instantiate(wood, spawnPoint.transform.position, Quaternion.identity);
            spawnedProjectile.transform.rotation = transform.rotation;
            spawnedProjectile.GetComponent<Wood>().GiveInitialVelocity();
            //create global timer
              
   
}
    }

    public override void onFinish()
    {
        base.onFinish();
        //this.GetComponent<Collider2D>().enabled = false;
        //give to wood script

    }

}
