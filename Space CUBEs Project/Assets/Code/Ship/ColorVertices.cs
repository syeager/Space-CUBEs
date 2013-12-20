using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class ColorVertices : MonoBehaviour
{
    public Color[] colors;
    
    
    private void Start()
    {
        colors = new Color[renderer.sharedMaterials.Length];
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.name = mesh.name.Replace(" Instance", "");
    }


    private void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Color[] vertColors = new Color[mesh.vertexCount];

        for (int i = 0; i < colors.Length; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j++)
            {
                vertColors[tris[j]] = colors[i];
            }
        }

        mesh.colors = vertColors;
    }
}