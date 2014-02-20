// Steve Yeager
// 2.19.2014

using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class CUBEImporter : MonoBehaviour
{
    #region Private Fields

    string modelPath;
    string prefabPath;

    #endregion


    #region MonoBehaviour Overrides

#if UNITY_EDITOR
    void Start()
    {
        GameObject model = UnityEditor.AssetDatabase.LoadAssetAtPath(modelPath, typeof(GameObject)) as GameObject;
        GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

        prefab.GetComponent<MeshFilter>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;

        DestroyImmediate(gameObject);
    }
#endif
    

    #endregion

    #region Public Methods

    public void Create(string modelPath, string prefabPath)
    {
        this.modelPath = modelPath;
        this.prefabPath = prefabPath;
    }

    #endregion
}