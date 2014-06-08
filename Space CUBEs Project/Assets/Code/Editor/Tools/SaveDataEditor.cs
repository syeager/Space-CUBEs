// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.19
// Edited: 2014.06.04

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
        typeof(float),
        typeof(string),
        typeof(Vector3),
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
    private void Update()
    {
        if (allData == null || allData.Length == 0)
        {
            ReloadAll();
        }
    }


    [UsedImplicitly]
    private void OnGUI()
    {
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
            //allData[i] = SaveData.LoadFileData(files[i]);
        }
    }


    private static void Reload(string file)
    {
        //allData[Array.IndexOf(files, file)] = SaveData.LoadFileData(file);
    }


    private void Save()
    {
        SaveData.FileSavedEvent -= OnFileSaved;

        string[] keys = allData[openFile].Keys.ToArray();
        foreach (string key in keys)
        {
            SaveData.Save(key, allData[openFile][key], files[openFile]);
        }

        SaveData.FileSavedEvent += OnFileSaved;
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
                allData[openFile][key] = DrawData(key, allData[openFile][key]);
            }
        }
        EditorGUILayout.EndScrollView();
    }


    private static object DrawData(string key, object data)
    {
        if (EditorTypes.Contains(data.GetType()))
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
        if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>))) // TODO: not IList. is generic
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
                if (EditorTypes.Contains(list[i].GetType()))
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