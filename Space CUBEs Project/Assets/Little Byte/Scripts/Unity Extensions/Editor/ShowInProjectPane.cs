// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.02
// Edited: 2014.07.02

using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Find object in Project pane.
/// </summary>
public class ShowInProjectPane : EditorWindow
{
    #region EditorWindow Overrides

    [UsedImplicitly]
    [MenuItem("Assets/Show in Project Pane &P", true)]
    private static bool Validate()
    {
        Object[] selected = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
        if (selected.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [UsedImplicitly]
    [MenuItem("Assets/Show in Project Pane &P")]
    private static void Init()
    {
        Object[] selected = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
        if (selected.Length == 0) return;

        EditorGUIUtility.PingObject(selected[0]);
    }

    #endregion
}