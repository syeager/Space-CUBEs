﻿// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Edit inventory in data.
/// </summary>
public class InventoryTool : EditorWindow
{
    #region Fields

    private static CUBEInfo[] info;
    private static int[] inventory;
    private int setAll;

    #endregion


    #region EditorWindow

    [MenuItem("Tools/Inventory Tool")]
    private static void Init()
    {
        InventoryTool window = EditorWindow.GetWindow<InventoryTool>(false, "Inventory Tool");
        info = CUBE.LoadAllCUBEInfo();
        inventory = CUBE.GetInventory();
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Load Inventory"))
        {
            inventory = CUBE.GetInventory();
        }

        EditorGUILayout.BeginHorizontal();
        {
            setAll = EditorGUILayout.IntField("All", setAll);
            if (GUILayout.Button("Set"))
            {
                for (int i = 0; i < inventory.Length; i++)
                {
                    inventory[i] = setAll;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < inventory.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(i + " " + info[i].name);
                inventory[i] = EditorGUILayout.IntField(inventory[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save Inventory"))
        {
            CUBE.SetInventory(inventory);
        }
    }

    #endregion
}