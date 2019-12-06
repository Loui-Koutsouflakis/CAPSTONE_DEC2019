using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracks : MonoBehaviour
{
    public Shader drawShader;
    //public Texture testTexture;
    //public Shader DrawTracks;
    private RenderTexture splatmap;
    private Material drawMaterial;
    private Material myMaterial;
    public GameObject terrian;
    public Transform[] feetPositions;
    RaycastHit groundHit;
    public float RayLength;
    public LayerMask layermask;

    //public RenderTexture BrushImage;

    [Range(0, 1)]
    public float BrushSize;
    [Range(0, 10)]
    public float BrushOpacity;

    // Start is called before the first frame update
    void Start()
    {
            drawMaterial = new Material(drawShader);
            myMaterial = terrian.GetComponent<MeshRenderer>().material;
            myMaterial.SetTexture("_Splat", splatmap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat));
            //DrawTracks.
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < feetPositions.Length; i++)
        {
            if (Physics.Raycast(feetPositions[i].position, Vector3.down, out groundHit, RayLength, layermask))
            {
                drawMaterial.SetVector("_Coordinate", new Vector4(groundHit.textureCoord.x, groundHit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_BrushOpacity", BrushOpacity);
                drawMaterial.SetFloat("_BrushSize", BrushSize);
                RenderTexture temp = RenderTexture.GetTemporary(splatmap.width, splatmap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatmap, temp);
                Graphics.Blit(temp, splatmap, drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
