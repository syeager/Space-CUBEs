// Steve Yeager
// 11.26.2013

using System;
using UnityEngine;

public class CUBE : MonoBehaviour
{
    #region Public Fields

    public int ID;
    private Vector3 pivot;
    public Vector3[] pieces = new Vector3[0];
    public Color[] colors;

    public enum CUBETypes
    {
        Armor = 0,
        Weapon = 1,
        Cockpit = 2,
        Engine = 3,
        Wing = 4,
    }
    public CUBETypes CUBEType;

    public float health;
    public float shield;
    public float speed;

    #endregion

    #region Static Fields

    public static int IDs { get; private set; }

    #endregion


    #region Public Methods

    [Obsolete("need to pass Color[] colors from CG.Build")]
    public void ColorVertices()
    {
        // new mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // material
        Material[] materials = new Material[renderer.materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = GameResources.Main.VertexColor_Mat;
        }
        renderer.materials = materials;

        // create color array
        Color[] vertColors = new Color[mesh.vertexCount];
        for (int i = 0; i < colors.Length; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j++)
            {
                vertColors[tris[j]] = colors[i];
            }
        }

        // save new colors
        mesh.colors = vertColors;
    }

    #endregion
}