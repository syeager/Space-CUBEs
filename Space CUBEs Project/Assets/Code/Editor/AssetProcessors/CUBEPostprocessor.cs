// Steve Yeager
// 1.30.2014

using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class CUBEPostprocessor : AssetPostprocessor
{
    #region Const Fields

    private const string MESHPATH = "Assets/Ship/CUBEs/Meshes/";
    private const string MATERIALPATH = MESHPATH + "Materials";
    private const string VERTEXCOLORPATH = "Assets/Ship/CUBEs/Materials/VertexColor_Mat.mat";
    private const string PREFABPATH = "Assets/Ship/CUBEs/Prefabs/";

    #endregion


    #region AssetPostprocessor Overrides

    private void OnPreprocessModel()
    {
        if (!assetPath.Contains(MESHPATH)) return;
        
        ModelImporter importer = assetImporter as ModelImporter;

        importer.globalScale = 1f;
        importer.meshCompression = ModelImporterMeshCompression.High;
        importer.isReadable = true;
        importer.optimizeMesh = true;
        importer.importBlendShapes = false;
        importer.addCollider = false;
        importer.swapUVChannels = false;

        importer.importMaterials = false;

        importer.animationType = ModelImporterAnimationType.None;
    }


    private void OnPostprocessModel(GameObject gameObject)
    {
        if (!assetPath.Contains(MESHPATH)) return;

        // create prefab
        string prefabName = gameObject.name.Replace('_', ' ').Remove(gameObject.name.Length - 6);
        GameObject go = new GameObject(prefabName);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.renderer.castShadows = false;
        go.renderer.receiveShadows = false;

        string prefabPath = PREFABPATH + prefabName + ".prefab";
        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, go);
        prefab.AddComponent<CUBE>();
        // add weapon

        var cube = go.AddComponent<CUBEImporter>();
        cube.Create(assetPath, prefabPath);

        CUBEUpdater.Update();
    }


    private Material OnAssignMaterialModel(Material material, Renderer renderer)
    {
        return (Material)AssetDatabase.LoadAssetAtPath(VERTEXCOLORPATH, typeof(Material));
    }

    #endregion
}