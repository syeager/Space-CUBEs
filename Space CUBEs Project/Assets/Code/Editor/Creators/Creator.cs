// Steve Yeager
// 4.27.2014

using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles creating an instance from a prefab in Edit Mode.
/// </summary>
/// <typeparam name="T">Singleton to create.</typeparam>
public class Creator<T> : Editor where T : MonoBehaviour
{
    #region Const Fields

    private const string Path = "Assets/Prefabs/Common/";
    private const string Postfix = ".prefab";

    #endregion


    #region Static Methods

    /// <summary>
    /// Instantiate a prefab in the scene.
    /// </summary>
    /// <param name="prefabName">Name of the prefab gameObject.</param>
    /// <param name="path">Path, starting with Assets, to the prefab in the project pane.</param>
    /// <param name="breakPrefab">Should the instantiated object be disconnected from its prefab?</param>
    protected static void Create(string prefabName, string path = "", bool breakPrefab = false)
    {
        if (path == "")
        {
            path = Path;
        }

        Object instance = FindObjectOfType(typeof(T));
        if (instance != null)
        {
            Debug.LogWarning("There is already a " + prefabName + " in the scene.", instance);
        }
        else
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path + prefabName + Postfix, typeof(GameObject)) as GameObject;
            GameObject created = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            if (Selection.activeObject != null)
            {
                created.transform.parent = Selection.activeTransform;
                created.layer = Selection.activeGameObject.layer;
            }

            Selection.activeGameObject = created;

            if (breakPrefab)
            {
                PrefabUtility.DisconnectPrefabInstance(created);
            }

            Debug.Log("Created " + prefabName + ".", created);
        }
    }

    #endregion
}