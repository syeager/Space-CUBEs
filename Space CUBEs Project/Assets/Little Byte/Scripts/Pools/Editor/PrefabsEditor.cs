// Steve Yeager
// 4.27.2014

using System.Collections.Generic;
using Annotations;
using LittleByte.Extensions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creator/Editor for PoolManager class.
/// </summary>
[CustomEditor(typeof(Prefabs))]
public class PrefabsEditor : Creator<Prefabs>
{
    #region Const Fields

    private const string PrefabName = "_PrefabPoolManager";
    private const string PrefabPath = "Assets/Global/";

    #endregion

    #region Editor Fields

    private Prefabs prefabsSource;
    private SerializedObject poolManager;
    private SerializedProperty cull;
    private SerializedProperty cullDelay;
    private SerializedProperty poolList;

    #endregion

    #region Private Fields

    private bool poolListToggle = true;
    private List<bool> poolToggles;

    #endregion

    #region Creator Methods

    [MenuItem("GameObject/Singletons/Prefab Pool Manager", false, 3)]
    public static void Create()
    {
        Create(PrefabName, PrefabPath, true);
    }

    #endregion

    #region Editor Overrides

    [UsedImplicitly]
    private void OnEnable()
    {
        if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab) return;

        prefabsSource = target as Prefabs;
        if (prefabsSource.poolManager == null) return;

        poolManager = new SerializedObject(prefabsSource.poolManager);
        cull = poolManager.FindProperty("cull");
        cullDelay = poolManager.FindProperty("cullDelay");
        poolList = poolManager.FindProperty("poolList");

        poolToggles = new List<bool>(poolList.arraySize);
        poolToggles.Initialize(false, poolList.arraySize);
    }


    public override void OnInspectorGUI()
    {
        Repaint();
        GUIStyle textStyle = new GUIStyle { normal = { textColor = Color.white }, wordWrap = true, richText = true };
        if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
        {
            EditorGUILayout.LabelField("<color=#ffff00>Needs to be an instance.</color>", textStyle);
            return;
        }
        else if (poolManager == null)
        {
            EditorGUILayout.LabelField("<color=#ff0000>No pool manager instance.</color>", textStyle);
            return;
        }

        poolManager.Update();

        Cull();
        EditorGUILayout.Space();
        PoolList();
        DropAreaGUI();

        poolManager.ApplyModifiedProperties();
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
        // buttons
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Open", EditorStyles.miniButtonLeft))
            {
                poolListToggle = true;
                poolToggles.SetAll(true);
            }
            if (GUILayout.Button("Close", EditorStyles.miniButtonRight))
            {
                poolListToggle = false;
                poolToggles.SetAll(false);
            }
        }

        EditorGUILayout.EndHorizontal();
        poolListToggle = EditorGUILayout.Foldout(poolListToggle, "Pool List");
        if (poolListToggle)
        {
            EditorGUI.indentLevel++;

            if (poolList.arraySize == 0)
            {
                GUILayout.Label("Empty");
            }
            else
            {
                // pools
                for (int i = 0; i < poolList.arraySize; i++)
                {
                    Pool(i);
                }
            }
            EditorGUI.indentLevel--;
        }
    }


    private void Pool(int index)
    {
        if (poolToggles == null || poolToggles.Count - 1 < index)
        {
            poolToggles = new List<bool>(poolList.arraySize);
            poolToggles.Initialize(false, poolList.arraySize);
        }
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

            GUILayout.FlexibleSpace();
            Color cachedColor = GUI.color;
            Pool p = prefabsSource.poolManager.poolList[index];
            float percent = p.cull ? (float)p.PoolCount / p.cullLimit : 0f;
            GUI.color = Color.Lerp(Color.green, percent >= 1f ? Color.red : Color.yellow, p.cull ? percent : 0f);
            GUILayout.Label(p.ActiveCount.ToString("000") + "+" + p.InactiveCount.ToString("000") + "/" + (p.cull ? p.cullLimit.ToString("000") : "∞∞") + "=" + p.PoolCount.ToString("000"));
            GUI.color = cachedColor;

            GUI.enabled = index > 0;
            if (GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(30f)))
            {
                MovePool(index, index - 1);
            }
            GUI.enabled = index < poolList.arraySize - 1;
            if (GUILayout.Button("↓", EditorStyles.miniButtonRight, GUILayout.Width(30f)))
            {
                MovePool(index, index + 1);
            }
            GUI.enabled = true;
            if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(30f)))
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
                            PoolObject poolObject = (dragged as GameObject).GetComponent<PoolObject>();
                            if (poolObject != null)
                            {
                                Debugger.Log("Added: " + dragged.name);
                                AddPool(poolList.arraySize, poolObject);
                            }
                        }
                        else
                        {
                            Debugger.Log("Ignored: " + dragged.name);
                        }
                    }
                }
                break;
        }
    }


    private void AddPool(int index, PoolObject prefab)
    {
        poolManager.Update();
        poolList.InsertArrayElementAtIndex(index);
        poolManager.ApplyModifiedProperties();

        UpdatePool(index, prefab);
    }


    private void UpdatePool(int index, PoolObject prefab)
    {
        bool foundParent = PrefabUtility.GetPrefabType(prefab) == PrefabType.PrefabInstance && prefab.transform.parent.GetComponents<Component>().Length == 1;
        prefabsSource.poolManager.poolList[index] = new Pool(prefabsSource.poolManager, prefab, foundParent ? prefab.transform.parent : null);
        poolToggles.Add(true);
        
    }


    private void RemovePool(int index)
    {
        serializedObject.Update();
        poolList.DeleteArrayElementAtIndex(index);
        poolToggles.RemoveAt(index);
        serializedObject.ApplyModifiedProperties();
    }


    private void MovePool(int index, int dest)
    {
        poolList.MoveArrayElement(index, dest);
    }

    #endregion
}