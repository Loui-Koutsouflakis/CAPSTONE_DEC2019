using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

    public GameObject projectile;

    public Transform spawn;

    public int speed;

    public void ShootProjectile()
    {
        GameObject shootItem = Instantiate(projectile, spawn.transform.position, Quaternion.identity);
        shootItem.transform.rotation = spawn.rotation;
        shootItem.GetComponent<Rigidbody>().AddForce(spawn.transform.forward * speed, ForceMode.Impulse);
    }
}
