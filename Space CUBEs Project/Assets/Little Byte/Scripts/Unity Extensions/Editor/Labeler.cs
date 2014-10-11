// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.11
// Edited: 2014.05.23

using System;
using System.Collections.Generic;
using System.Linq;
using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Sets labels for Objects.
/// </summary>
public class Labeler : EditorWindow
{
    #region Private Fields

    private List<string> labels;
    private string newLabel = "";
    private Vector2 scrollPosition;
    private Vector2 labelScroll;

    #endregion

    #region Static Fields

    private static GameObject[] objects;

    #endregion

    #region Const Fields

    private const string LabelPath = "Labels";
    private static readonly char[] LabelSep = {';'};

    #endregion

    #region EditorWindow Overrides

    [UsedImplicitly]
    [MenuItem("Tools/Manage Labels %L", false, 150)]
    [MenuItem("Assets/Manage Labels %L")]
    private static void Init()
    {
        var window = GetWindow<Labeler>("Labeler");

        objects = Selection.gameObjects.Where(go => PrefabUtility.GetPrefabType(go) == PrefabType.Prefab).ToArray();
        window.labels = EditorPrefs.GetString(LabelPath).Split(LabelSep, StringSplitOptions.RemoveEmptyEntries).ToList();
    }


    [UsedImplicitly]
    private void OnSelectionChange()
    {
        objects = Selection.gameObjects.Where(go => PrefabUtility.GetPrefabType(go) == PrefabType.Prefab).ToArray();
    }


    [UsedImplicitly]
    private void OnGUI()
    {
        // info
        GUILayout.Label("Objects: " + objects.Length, EditorStyles.boldLabel);

        // objects
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        {
            foreach (GameObject obj in objects)
            {
                GUILayout.BeginHorizontal();
                {
                    // name
                    GUILayout.Label(obj.name, GUILayout.Width(150f));

                    // labels
                    foreach (string label in AssetDatabase.GetLabels(obj))
                    {
                        if (GUILayout.Button(label, EditorStyles.miniButtonMid, GUILayout.Width(80f)))
                        {
                            // create
                            if (Event.current.button == 2)
                            {
                                CreateLabel(label);
                            }
                                // remove
                            else if (Event.current.button == 1)
                            {
                                RemoveLabel(obj, label);
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
        GUILayout.Space(10f);

        // labels
        int i = GUILayout.SelectionGrid(-1, labels.ToArray(), Screen.width / 100);
        if (i != -1)
        {
            if (Event.current.button == 0)
            {
                AddLabel(labels[i]);
                return;
            }
            else if (Event.current.button == 1)
            {
                RemoveLabelAll(labels[i]);
                return;
            }
            else if (Event.current.button == 2)
            {
                DeleteLabel(labels[i]);
            }
        }

        GUI.SetNextControlName("New Label");
        newLabel = GUILayout.TextField(newLabel);
        if (GUILayout.Button("Add"))
        {
            CreateLabel(newLabel);
            return;
        }

        GUILayout.Space(10f);

        // return
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "New Label")
        {
            CreateLabel(newLabel);
        }
    }


    [UsedImplicitly]
    private void OnDestroy()
    {
        Save();
    }

    #endregion

    #region Private Methods

    private void AddLabel(string label)
    {
        foreach (GameObject go in objects)
        {
            List<string> objectLabels = AssetDatabase.GetLabels(go).ToList();
            if (objectLabels.Contains(label))
            {
                continue;
            }
            objectLabels.Add(label);
            AssetDatabase.SetLabels(go, objectLabels.ToArray());
        }

        Repaint();
    }


    private void RemoveLabelAll(string label)
    {
        foreach (GameObject go in objects)
        {
            RemoveLabel(go, label);
        }
    }


    private void RemoveLabel(GameObject go, string label)
    {
        List<string> objectLabels = AssetDatabase.GetLabels(go).ToList();
        if (!objectLabels.Contains(label))
        {
            return;
        }
        objectLabels.Remove(label);
        AssetDatabase.SetLabels(go, objectLabels.ToArray());

        Repaint();
    }


    private void CreateLabel(string label)
    {
        if (string.IsNullOrEmpty(label)) return;
        if (labels.Contains(label)) return;

        labels.Add(label);
        if (newLabel == label)
        {
            newLabel = "";
        }
        Save();

        Repaint();
    }


    private void DeleteLabel(string label)
    {
        labels.Remove(label);
        Save();

        Repaint();
    }


    private void Save()
    {
        //EditorPrefs.SetString(LabelPath, labels.Join(LabelSep));
    }

    #endregion
}