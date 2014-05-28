// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.22
// Edited: 2014.05.23

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
    [MenuItem("Shortcuts/Redo %#Z", false, 0)]
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
}