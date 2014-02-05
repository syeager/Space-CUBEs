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
    #region AssetPostprocessor Overrides

    private void OnPostprocessModel(GameObject gameObject)
    {
        //Debug.Log(gameObject.GetComponent<ModelIm);
    }

    #endregion

    #region Private Methods

    private void Strip(ModelImporter importer)
    {
        
    }

    #endregion
}