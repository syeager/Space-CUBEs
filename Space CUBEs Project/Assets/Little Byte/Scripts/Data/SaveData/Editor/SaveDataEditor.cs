// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.19
// Edited: 2014.06.09

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Annotations;
using LittleByte.Data;
using UnityClasses;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Uses Reflection to display editable GUI fields for all SaveData.
/// </summary>
public class SaveDataEditor : EditorWindow
{
    #region File Fields

    private static string[] files;

    private static FolderNode rootNode;
    private static object currentData;
    private static string currentFile;
    private static string currentPath;

    #endregion

    #region GUI Fields

    private static bool autoRefresh = true;
    private static SaveDataEditor window;
    private int openFile;
    private Vector2 valueScrollPosition;
    private Vector2 fileScroll;

    #endregion

    #region Editor Overrides

    [UsedImplicitly]
    [MenuItem("Tools/Save Data Editor", false, 100)]
    private static void Init()
    {
        window = GetWindow<SaveDataEditor>("Save Data");
        ReloadAll();
    }


    [UsedImplicitly]
    private void Update()
    {
        if (rootNode == null)
        {
            ReloadAll();
        }
    }


    [UsedImplicitly]
    private void OnGUI()
    {
        GUILayout.BeginHorizontal("box");
        {
            autoRefresh = EditorGUILayout.Toggle("Auto Refresh", autoRefresh);
            GUI.enabled = !autoRefresh;
            // TODO: Refresh all, refresh on switch file. Settings tab along with dynamic toolbar?
            if (GUILayout.Button("Refresh"))
            {
                Reload(files[openFile]);
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
        {
            GUILayout.BeginVertical("box", GUILayout.Width(Screen.width * 0.75f - 10f));
            {
                if (currentData != null)
                {
                    Values();
                }
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical("box", GUILayout.Width(Screen.width * 0.25f));
            {
                Files();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save"))
        {
            Save();
        }
    }

    #endregion

    #region Private Methods

    private static void AddFile(string[] paths, int cursor, FolderNode node)
    {
        while (true)
        {
            // done
            if (cursor == paths.Length)
            {
                return;
            }

            FolderNode nextNode = node.GetNode(paths[cursor]);

            // not found
            if (nextNode == null)
            {
                FolderNode newNode = node.Add(paths[cursor]);
                cursor = cursor + 1;
                node = newNode;
            }
            else
            {
                cursor = cursor + 1;
                node = nextNode;
            }
        }
    }


    private static void ReloadAll()
    {
        SaveData.FileSavedEvent -= OnFileSaved;
        SaveData.FileSavedEvent += OnFileSaved;

        files = SaveData.GetAllFiles();
        rootNode = new FolderNode {value = "root"};

        foreach (string file in files)
        {
            string[] paths = file.Split('\\');
            AddFile(paths, 0, rootNode);
        }
    }


    private static void Reload(string filePath)
    {
        currentData = SaveData.LoadFromPath(filePath);
    }


    private void Save()
    {
        SaveData.FileSavedEvent -= OnFileSaved;

        SaveData.Save(currentFile, currentData, currentPath);

        //string[] keys = allData[openFile].Keys.ToArray();
        //foreach (string key in keys)
        //{
        //    SaveData.Save(key, allData[openFile][key], files[openFile]);
        //}

        SaveData.FileSavedEvent += OnFileSaved;
    }


    private void Files()
    {
        fileScroll = EditorGUILayout.BeginScrollView(fileScroll);
        {
            DrawNode(rootNode);
        }
        EditorGUILayout.EndScrollView();
    }


    private static void DrawNode(FolderNode node)
    {
        // no children
        if (node.children.Count == 0)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space((EditorGUI.indentLevel - 1) * 18f);
                if (GUILayout.Button(node.value))
                {
                    currentPath = node.Path();
                    currentFile = node.value;
                    currentData = SaveData.LoadFromPath(currentPath + currentFile);
                }
            }
            GUILayout.EndHorizontal();
            return;
        }

        if (node != rootNode)
        {
            EditorGUILayout.LabelField(node.value, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
        }
        foreach (FolderNode child in node.children)
        {
            DrawNode(child);
        }
        EditorGUI.indentLevel--;
    }


    private void Values()
    {
        valueScrollPosition = EditorGUILayout.BeginScrollView(valueScrollPosition);
        {
            currentData = DrawData(currentFile, currentData);
        }
        EditorGUILayout.EndScrollView();
    }


    private static object DrawData(string key, object data)
    {
        if (UnityTypes.IsEditor(currentData))
        {
            return DrawUnity(key, data);
        }

        return DrawObject(key, data);
    }


    private static object DrawUnity(string key, object data)
    {
        Type type = data.GetType();

        if (type == typeof(int))
        {
            return EditorGUILayout.IntField(key, (int)data);
        }
        if (type == typeof(float))
        {
            return EditorGUILayout.FloatField(key, (float)data);
        }
        if (type == typeof(string))
        {
            return EditorGUILayout.TextField(key, (string)data);
        }
        if (type == typeof(Vector3))
        {
            return EditorGUILayout.Vector3Field(key, (Vector3)data);
        }

        return Debugger.LogException(new TypeNotSupportedException(data, "Need drawer for this type."));
    }


    private static object DrawObject(string key, object data)
    {
        EditorGUILayout.Foldout(true, key);
        EditorGUI.indentLevel++;

        if (data == null)
        {
            EditorGUILayout.LabelField("Empty");
            EditorGUI.indentLevel--;
            return null;
        }

        Type type = data.GetType();

        // list
        if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>)))
        {
            var list = data as IList;
            Debug.Log(list.Count);
            // empty
            if (list.Count == 0)
            {
                EditorGUILayout.LabelField("Empty");
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is IUnityClass)
                {
                    IUnityClass item = (IUnityClass)list[i];
                    list[i] = DrawUnity(i.ToString(), item.Cast());
                }
                if (UnityTypes.IsEditor(list[i]))
                {
                    list[i] = DrawUnity(i.ToString(), list[i]);
                }
                else
                {
                    DrawObject(i.ToString(), list[i]);
                }
            }
        }
            //else if (data != null && data.GetType() == typeof(KeyValuePair<,>))
            //{
            //    Debug.Log("haldfj;al");
            //}
            //else if (dict != null)
            //{
            //    Debug.Log("here");
            //    foreach (var entry in dict)
            //    {
            //        if (EditorTypes.Contains(entry.Value.GetType()))
            //        {
            //            dict[entry.Key] = DrawUnity(entry.Key.ToString(), entry.Value);
            //        }
            //        else
            //        {
            //            DrawObject(entry.Key.ToString(), entry.Value);
            //        }
            //    }
            //}
        else
        {
            FieldInfo[] fieldInfo = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in fieldInfo)
            {
                if (UnityTypes.IsEditor(info.FieldType))
                {
                    info.SetValue(data, DrawUnity(info.Name, info.GetValue(data)));
                }
                else
                {
                    DrawData(info.Name, info.GetValue(data));
                }
            }
        }

        EditorGUI.indentLevel--;

        return data;
    }

    #endregion

    #region Event Handlers

    private static void OnFileSaved(object sender, ValueArgs args)
    {
        if (!autoRefresh) return;

        Reload(args.value.ToString());
    }

    #endregion
}


public class FolderNode
{
    public FolderNode parent;
    public List<FolderNode> children = new List<FolderNode>();
    public string value;


    public FolderNode Add(string value)
    {
        FolderNode child = new FolderNode {value = value, parent = this};
        children.Add(child);
        return child;
    }


    public FolderNode GetNode(string value)
    {
        return children.FirstOrDefault(child => child.value.Equals(value));
    }


    public string Path()
    {
        FolderNode node = this;
        StringBuilder path = new StringBuilder();
        List<string> parents = new List<string>();

        while (true)
        {
            // root
            if (node.parent == null)
            {
                for (int i = parents.Count - 1; i > 0; i--)
                {
                    path.Append(parents[i]);

                    if (i > 0)
                    {
                        path.Append('\\');
                    }
                }

                return path.ToString();
            }

            parents.Add(node.value);
            node = node.parent;
        }
    }
}