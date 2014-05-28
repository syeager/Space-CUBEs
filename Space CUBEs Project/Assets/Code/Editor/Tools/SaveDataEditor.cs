// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.19
// Edited: 2014.05.23

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Annotations;
using LittleByte.Data;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Uses Reflection to display editable GUI fields for all SaveData.
/// </summary>
public class SaveDataEditor : EditorWindow
{
    #region File Fields

    private static string[] files;
    private static Dictionary<string, object>[] allData;

    // TODO: move to another class
    private static readonly Type[] EditorTypes =
    {
        typeof(int),
        typeof(string),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
    };

    #endregion

    #region GUI Fields

    private static bool autoRefresh = true;
    private static SaveDataEditor window;
    private int openFile;
    private Vector2 valueScrollPosition;

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
    private void OnGUI()
    {
        if (allData == null || allData.Length == 0)
        {
            GUILayout.Label("Loading...");
            return;
        }

        GUILayout.BeginHorizontal();
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

        Files();
        Values();

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save"))
        {
            Save();
        }
    }

    #endregion

    #region Private Methods

    private static void ReloadAll()
    {
        SaveData.FileSavedEvent -= OnFileSaved;
        SaveData.FileSavedEvent += OnFileSaved;

        files = SaveData.GetFiles();
        allData = new Dictionary<string, object>[files.Length];

        for (int i = 0; i < allData.Length; i++)
        {
            Debugger.Print("Loading " + files[i] + "...");
            allData[i] = SaveData.LoadFileData(files[i]);
        }
    }


    private static void Reload(string file)
    {
        allData[Array.IndexOf(files, file)] = SaveData.LoadFileData(file);
    }


    private void Files()
    {
        openFile = GUILayout.SelectionGrid(openFile, files, 10);
    }


    private void Values()
    {
        valueScrollPosition = EditorGUILayout.BeginScrollView(valueScrollPosition);
        {
            string[] keys = allData[openFile].Keys.ToArray();
            foreach (string key in keys)
            {
                object newData = DrawData(key, allData[openFile][key]);
                if (newData != allData[openFile][key])
                {
                    allData[openFile][key] = newData;
                    // TODO: mark dirty
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }


    private void Save()
    {
        string[] keys = allData[openFile].Keys.ToArray();
        foreach (string key in keys)
        {
            SaveData.Save(key, allData[openFile][key], files[openFile]);
        }
    }


    private object DrawData(string key, object data)
    {
        //Debugger.Print("Key: {0}, Type: {1}", key, data.GetType().Name);
        if (EditorTypes.Contains(data.GetType()))
        {
            return DrawUnity(key, data);
        }

        return DrawObject(key, data);
    }


    private object DrawUnity(string key, object data)
    {
        Type type = data.GetType();

        if (type == typeof(int))
        {
            return EditorGUILayout.IntField(key, (int)data);
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


    private object DrawObject(string key, object data)
    {
        Type type = data.GetType();

        EditorGUILayout.Foldout(true, key);
        EditorGUI.indentLevel++;

        // list
        if (data is IEnumerable)
        {
            var list = data as IEnumerable;
            int i = 0;
            foreach (object child in list)
            {
                DrawObject(i.ToString(), child);
                i++;
            }
        }
        else
        {
            FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Static);
            foreach (FieldInfo info in fieldInfo)
            {
                if (EditorTypes.Contains(info.FieldType))
                {
                    info.SetValue(data, DrawUnity(info.Name, info.GetValue(data)));
                }
                else
                {
                    DrawObject(info.Name, info.GetValue(data));
                }
            }

            //PropertyInfo[] propertyInfo = type.GetProperties();
            //foreach (var info in propertyInfo)
            //{
            //    if (EditorTypes.Contains(info.PropertyType))
            //    {
            //        info.SetValue(data, DrawUnity(info.Name, info.GetValue(data, null)), null);
            //    }
            //    else
            //    {
            //        return DrawObject(info.Name, info.GetValue(data, null));
            //    }
            //}
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
        window.Repaint();
    }

    #endregion
}


[Serializable]
public class SObject : ScriptableObject
{
    [SerializeField] public object obj;
}