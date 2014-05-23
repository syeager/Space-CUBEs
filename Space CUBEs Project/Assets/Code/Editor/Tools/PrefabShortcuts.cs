// Steve Yeager
// 5.22.2014

using System.Linq;
using Annotations;
using UnityEditor;

/// <summary>
/// Actions that can be applied on Prefabs through shortcuts.
/// </summary>
public class PrefabShortcuts : EditorWindow
{
    #region EditorWindow Overrides

    [UsedImplicitly]
    [MenuItem("Tools/Apply Prefab #I", true)]
    [MenuItem("Tools/Break Prefab #O", true)]
    [MenuItem("Tools/Revert Prefab #P", true)]
    private static bool IsPrefab()
    {
        return Selection.gameObjects.All(obj => PrefabUtility.GetPrefabType(obj) == PrefabType.PrefabInstance);
    }


    [UsedImplicitly]
    [MenuItem("Tools/Apply Prefab #I", false, 0)]
    private static void ApplyPrefab()
    {
        foreach (var obj in Selection.gameObjects)
        {
            PrefabUtility.ReplacePrefab(obj, PrefabUtility.GetPrefabParent(obj), ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    [UsedImplicitly]
    [MenuItem("Tools/Break Prefab #O", false, 1)]
    private static void BreakPrefab()
    {
        foreach (var obj in Selection.gameObjects)
        {
            PrefabUtility.DisconnectPrefabInstance(obj);
        }
    }


    [UsedImplicitly]
    [MenuItem("Tools/Revert Prefab #P", false, 2)]
    private static void RevertPrefab()
    {
        foreach (var obj in Selection.gameObjects)
        {
            PrefabUtility.RevertPrefabInstance(obj);
        }
    }
    
    #endregion
}