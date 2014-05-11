// Steve Yeager
// 1.30.2014

using Annotations;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
public class CUBEPostprocessor : AssetPostprocessor
{
    #region Const Fields

    private const string MeshPath = "Assets/Ship/CUBEs/Meshes/";
    private const string VertexColorPath = "Assets/Ship/CUBEs/Materials/VertexColor_Mat.mat";
    private const string PrefabPath = "Assets/Ship/CUBEs/Prefabs/";

    #endregion


    #region AssetPostprocessor Overrides

    [UsedImplicitly]
    private void OnPreprocessModel()
    {
        if (!assetPath.Contains(MeshPath)) return;
        
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


    [UsedImplicitly]
    private void OnPostprocessModel(GameObject gameObject)
    {
        if (!assetPath.Contains(MeshPath)) return;

        // create prefab
        string prefabName = gameObject.name.Replace('_', ' ').Remove(gameObject.name.Length - 6);
        GameObject go = new GameObject(prefabName);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.renderer.castShadows = false;
        go.renderer.receiveShadows = false;

        string prefabPath = PrefabPath + prefabName + ".prefab";
        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, go);
        prefab.AddComponent<CUBE>();

        var cube = go.AddComponent<CUBEImporter>();
        cube.Create(assetPath, prefabPath);

        CUBEUpdater.Update();
    }


    [UsedImplicitly]
    private Material OnAssignMaterialModel(Material material, Renderer renderer)
    {
        return (Material)AssetDatabase.LoadAssetAtPath(VertexColorPath, typeof(Material));
    }

    #endregion
}