// Steve Yeager
// 12.19.2013

using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

/// <summary>
/// 
/// </summary>
[ExecuteInEditMode]
public class ColorVertices : MonoBehaviour
{
    #region Public Fields

    public Color[] colors;

    #endregion


    #region MonoBehaviour Overrides
    
    [Conditional("UNITY_EDITOR")]
    private void Update()
    {
        Bake();
    }

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

        Mesh mesh;
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.GetPrefabType(gameObject) == UnityEditor.PrefabType.Prefab)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }
        else
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }
#else
        mesh = GetComponent<MeshFilter>().mesh;
#endif

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


    public void SetandBake(int index, Color color)
    {
#if UNITY_EDITOR
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
        so.FindProperty("colors").GetArrayElementAtIndex(index).colorValue = color;
        so.ApplyModifiedProperties();
#else
        colors[index] = color;
#endif

        Bake();
    }

    #endregion
}