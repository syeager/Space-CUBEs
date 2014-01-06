// Steve Yeager
// 12.19.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 
/// </summary>
public class ColorVertices : MonoBehaviour
{
    #region Public Fields

    public Color[] colors;

    #endregion


    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newColors"></param>
    /// <param name="delete"></param>
    public void Bake(Color[] newColors = null, bool delete = false)
    {
        if (newColors == null)
        {
            newColors = colors;
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Color[] vertColors = new Color[mesh.vertexCount];

        for (int i = 0; i < newColors.Length; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j++)
            {
                vertColors[tris[j]] = newColors[i];
            }
        }

        mesh.colors = vertColors;

        if (delete)
        {
            Destroy(this);
        }
    }

    #endregion
}