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

    private const string MODELPATH = "Assets/Ship/CUBEs/Models/";
    private const string MATERIALPATH = MODELPATH + "Materials";
    private const string VERTEXCOLORPATH = "Assets/Ship/CUBEs/Materials/VertexColor_Mat.mat";
    private const string PREFABPATH = "Assets/Ship/CUBEs/Prefabs/";

    #endregion


    #region AssetPostprocessor Overrides

    private void OnPreprocessModel()
    {
        if (!assetPath.Contains(MODELPATH)) return;
        
        ModelImporter importer = assetImporter as ModelImporter;

        importer.globalScale = 1f;
        importer.meshCompression = ModelImporterMeshCompression.High;
        importer.isReadable = true;
        importer.optimizeMesh = true;
        importer.importBlendShapes = false;
        importer.addCollider = false;
        importer.swapUVChannels = false;

        importer.normalImportMode = ModelImporterTangentSpaceMode.None;
        importer.splitTangentsAcrossSeams = false;

        importer.importMaterials = false;

        importer.animationType = ModelImporterAnimationType.None;
    }


    private void OnPostprocessModel(GameObject gameObject)
    {
        if (!assetPath.Contains(MODELPATH)) return;

        // create prefab
        GameObject go = GameObject.Instantiate(gameObject) as GameObject;
        string prefabPath = PREFABPATH + gameObject.name.Replace('_', ' ').Remove(gameObject.name.Length - 6) + ".prefab";
        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, go);

        var renderer = prefab.GetComponent<MeshRenderer>();
        renderer.castShadows = false;
        renderer.receiveShadows = false;

        prefab.AddComponent<CUBE>();
        // add weapon
        CUBEUpdater.Update(); // update one CUBE

        var cube = go.AddComponent<CUBEImporter>();
        cube.Create(assetPath, prefabPath);
    }


    private Material OnAssignMaterialModel(Material material, Renderer renderer)
    {
        return (Material)AssetDatabase.LoadAssetAtPath(VERTEXCOLORPATH, typeof(Material));
    }

    #endregion
}