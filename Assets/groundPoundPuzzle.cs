using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundPoundPuzzle : MonoBehaviour, Interact
{
    MeshCollider[] mesh;
    Rigidbody[] body;
    ParticleSystem particles;
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
        }
        StartCoroutine(DisableShit());
    }

    IEnumerator DisableShit()
    {
        yield return new WaitForSeconds(1);
        foreach (var item in mesh)
        {
            item.enabled = false;
        }
        GetComponent<BoxCollider>().enabled = false;
    }
}
