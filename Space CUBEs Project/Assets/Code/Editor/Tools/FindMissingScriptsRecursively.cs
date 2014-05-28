// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.01
// Edited: 2014.05.27

using UnityEditor;
using UnityEngine;

/// <summary>
/// Find all missing scripts on a selection of GameObjects.
/// </summary>
public class FindMissingScriptsRecursively : EditorWindow
{
    #region Static Fields

    private static int gameObjectCount;
    private static int componentsCount;
    private static int missingCount;

    #endregion

    #region EditorWindow Overrides

    [MenuItem("Tools/FindMissingScriptsRecursively", false, 151)]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScriptsRecursively));
    }


    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected();
        }
    }

    #endregion

    #region Public Methods

    public static void FindInAll()
    {
        GameObject[] go = FindObjectsOfType<GameObject>();
        gameObjectCount = 0;
        componentsCount = 0;
        missingCount = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }

        Debugger.Print("Searched {0} GameObjects, {1} components, found {2} missing", gameObjectCount, componentsCount, missingCount);
    }


    public static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        gameObjectCount = 0;
        componentsCount = 0;
        missingCount = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }

        Debugger.Print("Searched {0} GameObjects, {1} components, found {2} missing", gameObjectCount, componentsCount, missingCount);
    }


    public static void FindInGO(GameObject g)
    {
        gameObjectCount++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            componentsCount++;
            if (components[i] == null)
            {
                missingCount++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }

                Debugger.Break(s + " has an empty script attached in position: " + i, g);
            }
        }

        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            FindInGO(childT.gameObject);
        }
    }

    #endregion
}