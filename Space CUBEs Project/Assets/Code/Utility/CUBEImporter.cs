// Steve Yeager
// 2.19.2014

using Annotations;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CUBEImporter : MonoBehaviour
{
    #region Public Fields

    public Material VertexColor_Mat;

    #endregion

    #region Private Fields

    private string meshPath;
    private string prefabPath;

    #endregion


    #region MonoBehaviour Overrides

#if UNITY_EDITOR
    [UsedImplicitly]
    private void Start()
    {
        GameObject model = UnityEditor.AssetDatabase.LoadAssetAtPath(meshPath, typeof(GameObject)) as GameObject;
        GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

        // combine meshes
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>(true);
        List<CombineInstance> combine = new List<CombineInstance>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            Transform child = meshFilters[i].transform;
            CombineInstance combineInstance = new CombineInstance
            {
                mesh = meshFilters[i].sharedMesh,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale)
            };
            combine.Add(combineInstance);
        }

        // save mesh as asset
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine.ToArray(), false);
        mesh.Optimize();
        //mesh.uv = null; // -1KB

        // prefab
        string path = meshPath.Substring(0, meshPath.Length - 10) + "_Mesh.asset";
        UnityEditor.AssetDatabase.CreateAsset(mesh, path);
        prefab.GetComponent<MeshFilter>().sharedMesh = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));

        // materials
        int materialCount = meshFilters.Length;
        Material[] alphaMats = new Material[materialCount];
        for (int i = 0; i < materialCount; i++)
        {
            alphaMats[i] = VertexColor_Mat;
        }
        prefab.renderer.sharedMaterials = alphaMats;
        prefab.AddComponent<ColorVertices>().colors = new int[materialCount];

        // delete model
        UnityEditor.AssetDatabase.DeleteAsset(meshPath);       

        Debug.Log("Imported " + name);

        // delete self
        Resources.UnloadUnusedAssets();
        DestroyImmediate(gameObject);
    }
#endif


    #endregion

    #region Public Methods

    public void Create(string meshPath, string prefabPath)
    {
        this.meshPath = meshPath;
        this.prefabPath = prefabPath;
    }

    #endregion
}