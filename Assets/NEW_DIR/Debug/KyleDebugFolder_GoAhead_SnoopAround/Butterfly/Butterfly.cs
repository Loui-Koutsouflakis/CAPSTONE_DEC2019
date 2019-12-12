using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    MeshRenderer rend;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        rend.material = new Material(mat);
        rend.material = mat;
        mat.SetFloat("_DisplacementAmount", 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
