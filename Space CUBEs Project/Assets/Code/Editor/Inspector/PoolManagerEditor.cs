// Steve Yeager
// 4.27.2014

using System.Collections.Generic;
using Annotations;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creator/Editor for PoolManager class.
/// </summary>
[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Creator<PoolManager>
{
    #region Const Fields

    private const string PrefabName = "_PoolManager";
    private const string PrefabPath = "Assets/Global/";

    #endregion

    #region Editor Fields

    private SerializedProperty cull;
    private SerializedProperty cullDelay;
    private SerializedProperty poolList;

    #endregion

    #region Private Fields

    private bool poolListToggle;
    private List<bool> poolToggles;

    #endregion

    #region Creator Methods

    [MenuItem("GameObject/Singletons/Pool Manager", false, 3)]
    public static void Create()
    {
        Create(PrefabName, PrefabPath, true);
    }

    #endregion

    #region Editor Overrides

    [UsedImplicitly]
    private void OnEnable()
    {
        cull = serializedObject.FindProperty("cull");
        cullDelay = serializedObject.FindProperty("cullDelay");
        poolList = serializedObject.FindProperty("poolList");

        poolToggles = new List<bool>(poolList.arraySize);
        poolToggles.Initialize(false, poolList.arraySize);
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Cull();
        EditorGUILayout.Space();
        PoolList();
        DropAreaGUI();

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void Cull()
    {
        EditorGUILayout.PropertyField(cull);
        if (cull.boolValue)
        {
            EditorGUILayout.PropertyField(cullDelay);
        }
    }


    private void PoolList()
    {
        poolListToggle = EditorGUILayout.Foldout(poolListToggle, "Pool List");
        if (poolListToggle)
        {
            EditorGUI.indentLevel++;

            // buttons
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Open", EditorStyles.miniButtonLeft))
                {
                    poolToggles.SetAll(true);
                }
                if (GUILayout.Button("Close", EditorStyles.miniButtonRight))
                {
                    poolToggles.SetAll(false);
                }
            }
            EditorGUILayout.EndHorizontal();

            // pools
            for (int i = 0; i < poolList.arraySize; i++)
            {
                Pool(i);
            }
            EditorGUI.indentLevel--;
        }
    }


    private void Pool(int index)
    {
        SerializedProperty pool = poolList.GetArrayElementAtIndex(index);
        SerializedProperty prefab = pool.FindPropertyRelative("prefab");
        SerializedProperty preAllocate = pool.FindPropertyRelative("preAllocate");
        SerializedProperty allocateBlock = pool.FindPropertyRelative("allocateBlock");
        SerializedProperty hardLimit = pool.FindPropertyRelative("hardLimit");
        SerializedProperty limit = pool.FindPropertyRelative("limit");
        SerializedProperty cull = pool.FindPropertyRelative("cull");
        SerializedProperty cullLimit = pool.FindPropertyRelative("cullLimit");
        SerializedProperty parent = pool.FindPropertyRelative("parent");

        EditorGUILayout.BeginHorizontal();
        {
            poolToggles[index] = EditorGUILayout.Foldout(poolToggles[index], index + " " + (prefab.objectReferenceValue == null ? "---" : prefab.objectReferenceValue.name));

            if (GUILayout.Button("-", EditorStyles.miniButton))
            {
                RemovePool(index);
                return;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (poolToggles[index])
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(preAllocate);
            EditorGUILayout.PropertyField(allocateBlock);
            EditorGUILayout.PropertyField(hardLimit);
            if (hardLimit.boolValue)
            {
                EditorGUILayout.PropertyField(limit);
            }
            EditorGUILayout.PropertyField(cull);
            if (cull.boolValue)
            {
                EditorGUILayout.PropertyField(cullLimit);
            }
            EditorGUILayout.PropertyField(parent);

            EditorGUI.indentLevel--;
        }
    }


    private void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Add Pool Object");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged in DragAndDrop.objectReferences)
                    {
                        if (dragged is GameObject && ((GameObject)dragged).GetComponent<PoolObject>())
                        {
                            Debug.Log("Added: " + dragged.name);
                            AddPool(poolList.arraySize, dragged as GameObject);
                        }
                        else
                        {
                            Debug.Log("Ignored: " + dragged.name);
                        }
                    }
                }
                break;
        }
    }


    private void AddPool(int index, GameObject prefab)
    {
        serializedObject.Update();
        poolList.InsertArrayElementAtIndex(index);
        poolList.GetArrayElementAtIndex(index).FindPropertyRelative("prefab").objectReferenceValue = prefab;
        poolToggles.Add(true);
        serializedObject.ApplyModifiedProperties();
    }


    private void RemovePool(int index)
    {
        serializedObject.Update();
        poolList.DeleteArrayElementAtIndex(index);
        poolToggles.RemoveAt(index);
        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}