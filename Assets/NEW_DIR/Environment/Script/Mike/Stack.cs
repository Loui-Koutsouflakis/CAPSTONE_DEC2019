//written by Michael Elkin 11-26-2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct Info
{
    // Stacking Offset Height;
    public float offset;

    // Box Collider Info
    public Vector3 boxColliderSize;
    public Vector3 boxColliderCenter;

    // Capsule Collider Info
    public Vector3 capColliderCenter;
    public float capColliderHeight;
    public float capColliderRadius;

    // NavMesh Info    
    public float navMeshAgentHeight;
    public float navMeshAgentRadius;
}



public class Stack : MonoBehaviour
{
    // Access Struct
    [SerializeField]
    Info info;

    // Stack Offset
    [SerializeField, Range(0.0f, 10), Header("Stack Offset Size Input")]
    float offsetInput = 0.0f;

    // Box Collider Size Input
    [SerializeField, Range(0.0f, 10), Header("Box Collider Size Input")]
    float bColSizeX = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float bColSizeY = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float bColSizeZ = 0.0f;

    // Box Collider Center Input
    [SerializeField, Range(0.0f, 10), Header("Box Collider Center Input")]
    float bColCentX = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float bColCentY = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float bColCentZ = 0.0f;

    // Capsule Collider Center Input
    [SerializeField, Range(0.0f, 10), Header("Capsule Collider Center Input")]
    float capColCentX = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float capColCentY = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float capColCentZ = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float capColHeightInput = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float capColRadiusInput = 0.0f;

    // NavMesh Collider Input
    [SerializeField, Range(0.0f, 10)]
    float navAgentHeightInput = 0.0f;
    [SerializeField, Range(0.0f, 10)]
    float navAgentRadiusInput = 0.0f;

    
    //float navMeshAgentTurnSpeed = 0.0f;

    private void Awake()
    {
        info.offset = offsetInput;
        //Debug.Log(offsetInput + gameObject.name);

        info.boxColliderCenter = new Vector3(bColCentX, bColCentY, bColCentZ);
        info.boxColliderSize = new Vector3(bColSizeX, bColSizeY, bColSizeZ);
         
        info.capColliderCenter = new Vector3(capColCentX, capColCentY, capColCentZ);
        info.capColliderHeight = capColHeightInput;
        info.capColliderRadius = capColRadiusInput;

        info.navMeshAgentHeight = navAgentHeightInput;
        info.navMeshAgentRadius = navAgentRadiusInput;
    }

    public Info GetModelInfo()
    {
        return info;
    }

}
