// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.22
// Edited: 2014.05.28

using System.Linq;
using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Actions that can be applied on Prefabs through shortcuts.
/// </summary>
public class KeyboardShortcuts : EditorWindow
{
    #region Editor Shortcuts

    [UsedImplicitly]
    [MenuItem("Shortcuts/Redo #%Z", false, 0)]
    private static void Redo()
    {
        Undo.PerformRedo();
    }

    #endregion

    #region Prefab Shortcuts

    [UsedImplicitly]
    [MenuItem("Shortcuts/Apply Prefab &I", true)]
    [MenuItem("Shortcuts/Break Prefab &O", true)]
    [MenuItem("Shortcuts/Revert Prefab &P", true)]
    private static bool IsPrefab()
    {
        return Selection.gameObjects.All(obj => PrefabUtility.GetPrefabType(obj) == PrefabType.PrefabInstance);
    }


    [UsedImplicitly]
    [MenuItem("Shortcuts/Apply Prefab &I", false, 50)]
    private static void ApplyPrefab()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            PrefabUtility.ReplacePrefab(obj, PrefabUtility.GetPrefabParent(obj), ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    [UsedImplicitly]
    [MenuItem("Shortcuts/Break Prefab &O", false, 51)]
    private static void BreakPrefab()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            PrefabUtility.DisconnectPrefabInstance(obj);
        }
    }


    [UsedImplicitly]
    [MenuItem("Shortcuts/Revert Prefab &P", false, 52)]
    private static void RevertPrefab()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            PrefabUtility.RevertPrefabInstance(obj);
        }
    }

    #endregion

    #region GameObject Shortcuts

    [UsedImplicitly]
    [MenuItem("Shortcuts/Create Empty Child &%N", false, 100)]
    private static void CreateEmptyGameObjectChild()
    {
        GameObject created = new GameObject("_EmptyGameObject");
        if (Selection.activeGameObject)
        {
            created.tag = Selection.activeGameObject.tag;
            created.layer = Selection.activeGameObject.layer;
        }
        created.transform.parent = Selection.activeTransform;
        created.transform.ResetLocal();

        Selection.activeGameObject = created;
    }

    [UsedImplicitly]
    [MenuItem("Shortcuts/Create Empty Parent &%M", true, 101)]
    private static bool ValidateCreateEmptyGameObjectParent()
    {
        return Selection.activeTransform;
    }


    [UsedImplicitly]
    [MenuItem("Shortcuts/Create Empty Parent &%M", false, 101)]
    private static void CreateEmptyGameObjectParent()
    {
        GameObject created = new GameObject("_EmptyGameObject");
        created.tag = Selection.activeGameObject.tag;
        created.layer = Selection.activeGameObject.layer;

        Selection.activeTransform.parent = created.transform;
        created.transform.position = Selection.activeTransform.position;
        created.transform.rotation = Selection.activeTransform.rotation;
        Selection.activeTransform.ResetLocal(false);

        Selection.activeGameObject = created;
    }

    #endregion
}