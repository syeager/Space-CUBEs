// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.01
// Edited: 2014.10.10

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Annotations;
using LittleByte;
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
    private static FolderNode currentParent;
    private static string currentPath;

    #endregion

    #region GUI Fields

    private static bool autoRefresh = true;
    private static SaveDataEditor window;
    private int openFile;
    private Vector2 valueScrollPosition;
    private Vector2 fileScroll;

    private const float Margin = 5f;
    private const float FilesWidth = 0.5f;
    private const float DataWidth = 1f - FilesWidth;
    private const float Indent = 18f;

    #endregion

    #region Type Fields

    private static readonly Type[] EditorTypes =
    {
        typeof(bool),
        typeof(int),
        typeof(float),
        typeof(string),
        typeof(Color),
        typeof(Vector2),
        typeof(Vector3),
    };

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
        //GUI.skin = skin;

        InfoBarGUI();

        GUILayout.Space(Margin);

        GUILayout.BeginHorizontal();
        {
            FilesGUI();
            GUILayout.FlexibleSpace();
            if (currentData != null)
            {
                EditorGUI.indentLevel = 0;
                DataGUI();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
    }

    #endregion

    #region GUI Methods

    private void InfoBarGUI()
    {
        Color cachedColor = GUI.color;
        GUI.color = Color.gray;
        EditorGUILayout.BeginHorizontal("toolbar");
        {
            GUI.color = cachedColor;
            EditorGUIUtility.labelWidth = 80;
            if (GUILayout.Button("Auto Refresh " + (autoRefresh ? "On" : "Off"), EditorStyles.toolbarButton, GUILayout.MaxWidth(100f)))
            {
                autoRefresh = !autoRefresh;
            }
            EditorGUIUtility.labelWidth = 0;

            GUI.enabled = !autoRefresh;
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.MaxWidth(80f)))
            {
                Reload(files[openFile]);
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.MaxWidth(80f)))
            {
                Save();
            }

            if (GUILayout.Button("Backup", EditorStyles.toolbarButton, GUILayout.MaxWidth(80f)))
            {
                SaveData.Backup();
            }

            if (GUILayout.Button("Restore", EditorStyles.toolbarButton, GUILayout.MaxWidth(80f)))
            {
                SaveData.RestoreGameState(SaveData.GetLatestBackup());
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    private void FilesGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * FilesWidth - Margin));
        {
            GUILayout.BeginVertical("box");
            {
                fileScroll = EditorGUILayout.BeginScrollView(fileScroll);
                {
                    DrawNode(rootNode);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndVertical();
    }


    private void DataGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width * DataWidth - Margin));
        {
            GUILayout.BeginHorizontal("box");
            {
                valueScrollPosition = EditorGUILayout.BeginScrollView(valueScrollPosition);
                {
                    currentData = DrawData(currentFile, currentData);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndVertical();
    }


    private static void DrawNode(FolderNode node)
    {
        // no children
        if (node.children.Count == 0)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space((EditorGUI.indentLevel - 1) * Indent);
                Color cachedColor = GUI.color;
                GUI.color = currentFile == node.value && currentPath == node.Path() ? Color.gray : cachedColor;
                if (GUILayout.Button(node.value))
                {
                    currentPath = node.Path();
                    currentFile = node.value;
                    currentParent = node.parent;
                    currentData = SaveData.LoadFromPath<object>(currentPath + currentFile);
                }
                GUI.color = cachedColor;
            }
            GUILayout.EndHorizontal();
            return;
        }

        if (node != rootNode)
        {
            EditorGUILayout.LabelField(node.value, currentParent == node ? EditorStyles.boldLabel : EditorStyles.label);
            EditorGUI.indentLevel++;
        }
        foreach (FolderNode child in node.children)
        {
            DrawNode(child);
        }
        EditorGUI.indentLevel--;
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
            string[] paths = file.Split(SaveData.Slash[0]);
            AddFile(paths, 0, rootNode);
        }
    }


    private static void Reload(string filePath)
    {
        currentData = SaveData.LoadFromPath<object>(filePath);
        if (window != null)
        {
            window.Repaint();
        }
    }


    private void Save()
    {
        SaveData.FileSavedEvent -= OnFileSaved;
        SaveData.Save(currentFile, currentData, currentPath);
        SaveData.FileSavedEvent += OnFileSaved;
    }


    public static bool IsEditor(object obj)
    {
        return EditorTypes.Contains(obj.GetType());
    }


    private static object DrawData(string key, object data)
    {
        if (IsEditor(currentData))
        {
            return DrawUnity(key, data);
        }

        return DrawObject(key, data);
    }


    private static object DrawUnity(string key, object data)
    {
        Type type = data.GetType();

        if (type == typeof(bool))
        {
            return EditorGUILayout.Toggle(key, (bool)data);
        }
        if (type == typeof(float))
        {
            return EditorGUILayout.FloatField(key, (float)data);
        }
        if (type == typeof(int))
        {
            return EditorGUILayout.IntField(key, (int)data);
        }
        if (type == typeof(string))
        {
            return EditorGUILayout.TextField(key, (string)data);
        }
        if (type == typeof(Color))
        {
            return EditorGUILayout.ColorField(key, (Color)data);
        }
        if (type == typeof(Vector2))
        {
            return EditorGUILayout.Vector2Field(key, (Vector2)data);
        }
        if (type == typeof(Vector3))
        {
            return EditorGUILayout.Vector3Field(key, (Vector3)data);
        }

        return Debugger.LogException(new TypeNotSupportedException(data, "Need drawer for this type."));
    }


    private static object DrawObject(string key, object data)
    {
        EditorGUILayout.LabelField(key);
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
                if (IsEditor(list[i]))
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
                if (IsEditor(info.GetValue(data)))
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

    private static void OnFileSaved(object sender, FileSavedArgs args)
    {
        if (!autoRefresh) return;

        Reload(args.Path);
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
                        path.Append(SaveData.Slash);
                    }
                }

                return path.ToString();
            }

            parents.Add(node.value);
            node = node.parent;
        }
    }
}