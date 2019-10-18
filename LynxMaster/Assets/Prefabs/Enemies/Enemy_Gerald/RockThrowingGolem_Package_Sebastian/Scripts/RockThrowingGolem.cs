using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created By Sebastian Borkowski: 10/10/2018.
// Last Updated: 10/10/2019.
public class RockThrowingGolem : MonoBehaviour //, IKillable
{
    //public Transform MiddlePoint;

    public Transform RockSpawnPosition;
    public GameObject RockProjectile;
    public float RockSpeed;

    GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void PickUpRock()
    {
        bullet = Instantiate(RockProjectile, RockSpawnPosition.position, RockSpawnPosition.rotation);
    }

    public void Yeet()
    {
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * RockSpeed;
    }

    //public IEnumerator CheckHit()
    //{
    //    Debug.Log("CheckHit()");
    //    yield return new WaitForSeconds(1f);
    //}

    //public IEnumerator TakeDamage()
    //{
    //    Debug.Log("TakeDamage()");
    //    yield return new WaitForSeconds(1f);
    //}
    //public IEnumerator Die()
    //{
    //    Debug.Log("Die()");
    //    yield return new WaitForSeconds(1f);
    //}
}

