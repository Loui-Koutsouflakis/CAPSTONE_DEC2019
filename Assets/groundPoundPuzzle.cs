using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundPoundPuzzle : MonoBehaviour, Interact
{
    MeshCollider[] mesh;
    Rigidbody[] body;
    ParticleSystem particles;
    public float upWards;
    [SerializeField]
    bool isIce = false;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentsInChildren<MeshCollider>();
        body = GetComponentsInChildren<Rigidbody>();
        particles = GetComponentInChildren<ParticleSystem>();
        foreach (var item in mesh)
        {
            item.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    //    if (AreaChecks())
    //    {
    //        particles.Stop();
    //    }
    //    else if (!AreaChecks())
    //    {
    //        particles.Play();
    //    }
    }
    public void DontInteractWithMe()
    {
        throw new System.NotImplementedException();
    }

    public void InteractWithMe()
    {
        particles.Stop();
        foreach (var item in mesh)
        {
            item.enabled = true;
        }
        foreach (var item in body)
        {
            item.useGravity = true;
            item.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }

        StartCoroutine(DisableShit());
    }

    IEnumerator DisableShit()
    {
        if (isIce)
            yield return new WaitForSeconds(1);
        else
            yield return new WaitForSeconds(0);
        foreach (var item in mesh)
        {
            item.enabled = false;
        }
        GetComponent<BoxCollider>().enabled = false;
    }

    //public bool AreaChecks()
    //{
    //   // Vector3 lineStart = transform.position;
    //   // Vector3 forward = new Vector3(lineStart.x, lineStart.y + upWards, lineStart.z);
    //   //return Physics.BoxCast(transform.position, GetComponent<BoxCollider>().bounds.center / 2, Vector3.up, out up, Quaternion.identity, GetComponent<BoxCollider>().bounds.center.y);
    //  //  return Physics.Linecast(transform.position,forward, out up);
    //}
}
