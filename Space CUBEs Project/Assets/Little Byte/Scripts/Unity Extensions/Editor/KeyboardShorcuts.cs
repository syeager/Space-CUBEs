// Little Byte Games
// Author: Steve Yeager
// Created: 2014.05.22
// Edited: 2014.09.01

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Annotations;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    #region Application Shortcuts

    [UsedImplicitly]
    [MenuItem("Shortcuts/Open Data Folder #%D", false, 1)]
    private static void OpenDataPath()
    {
        string path = Application.persistentDataPath.Replace(@"/", @"\") + @"\";
        Debug.Log(path);
        Process.Start("explorer.exe", path);
    }


    [UsedImplicitly]
    [MenuItem("Shortcuts/Create Folder &F", false, 2)]
    private static void CreateFolder()
    {
        ProjectWindowUtil.CreateFolder();
    }


    [UsedImplicitly]
    [MenuItem("Shortcuts/Create C# Script &C", false, 3)]
    private static void CreateScript()
    {
        var window = GetWindow<ScriptName>(true);
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 250, 50);
        window.minSize = new Vector2(250, 50);
        window.maxSize = new Vector2(250, 50);
    }


    public static void CreateScript(string scriptName)
    {
        // get current path
        Type projectBrowser = Type.GetType("UnityEditor.ProjectBrowser, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        object last = projectBrowser.GetField("s_LastInteractedProjectBrowser", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        string path = projectBrowser.GetMethod("GetActiveFolderPath", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(last, null).ToString();
        path = path.Substring(6);

        // set class name
        Type componentMenu = Type.GetType("UnityEditor.AddComponentWindow, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        ScriptableObject addComponentWindow = CreateInstance(componentMenu);
        componentMenu.GetField("m_ClassName", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(addComponentWindow, scriptName);

        // create new script
        Type newScriptElement = Type.GetType("UnityEditor.AddComponentWindow+NewScriptElement, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        object created = Activator.CreateInstance(newScriptElement);

        // set values
        newScriptElement.GetField("m_Directory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(created, path);
        newScriptElement.GetMethod("CreateScript", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(created, null);

        // select and open
        Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets" + path + "/" + scriptName + ".cs", typeof(MonoScript));
        AssetDatabase.OpenAsset(Selection.activeObject);

        // cleanup
        DestroyImmediate(addComponentWindow);
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

        created.transform.parent = Selection.activeTransform.parent;
        Selection.activeTransform.parent = created.transform;
        created.transform.position = Selection.activeTransform.position;
        created.transform.rotation = Selection.activeTransform.rotation;
        Selection.activeTransform.ResetLocal(false);

        Selection.activeGameObject = created;
    }

    #endregion
}

public class ScriptName : EditorWindow
{
    private string scriptName = "";

    [UsedImplicitly]
    private void OnGUI()
    {
        GUI.SetNextControlName("name");
        scriptName = EditorGUILayout.TextField(scriptName);
        if (GUI.GetNameOfFocusedControl() != "name")
        {
            GUI.FocusControl("name");
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create"))
        {
            Create();
        }

        if (Event.current.isKey)
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                Create();
            }
            else if (Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
        }
    }


    private void Create()
    {
        KeyboardShortcuts.CreateScript(scriptName);
        Close();
    }
}