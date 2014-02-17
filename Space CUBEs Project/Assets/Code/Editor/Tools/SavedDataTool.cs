// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// Edit saved data.
/// </summary>
public class SavedDataTool : EditorWindow
{
    #region GUI Fields

    private enum Menus
    {
        Money,
        Inventory,
        Builds,
    }
    private static Menus menu;

    #endregion

    #region Money Fields

    private static int bank;

    #endregion

    #region Inventory Fields

    private static int[] inventory;
    private static CUBEInfo[] info;
    private int setAll;
    private Vector2 inventoryScroll;

    #endregion

    #region Build Fields

    private static List<string> builds; 

    #endregion


    #region EditorWindow

    [MenuItem("Tools/Saved Data Tool")]
    private static void Init()
    {
        EditorWindow.GetWindow<SavedDataTool>(false, "Saved Data Tool");
        bank = MoneyManager.Balance();
        info = CUBE.LoadAllCUBEInfo();
        inventory = CUBE.GetInventory();
        builds = ConstructionGrid.BuildNames();
        menu = (Menus)EditorPrefs.GetInt("Menu");
    }


    private void OnDestroy()
    {
        // save open menu
        EditorPrefs.SetInt("Menu", (int)menu);
    }


    private void OnGUI()
    {
        string[] menus = Enum.GetNames(typeof(Menus));
        menu = (Menus)GUILayout.SelectionGrid((int)menu, menus, menus.Length);
        EditorGUILayout.Space();

        switch (menu)
        {
            case Menus.Money:
                Money();
                break;
            case Menus.Inventory:
                Inventory();
                break;
            case Menus.Builds:
                Builds();
                break;
        }
    }

    #endregion

    #region Private Methods

    private void Money()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Bank");
            if (GUILayout.Button("Load"))
            {
                bank = MoneyManager.Balance();
            }
            bank = EditorGUILayout.IntField(bank);
            if (GUILayout.Button("Save"))
            {
                MoneyManager.SetBalance(bank);
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    private void Inventory()
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
        inventoryScroll = EditorGUILayout.BeginScrollView(inventoryScroll);
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(i + " " + info[i].name);
                    inventory[i] = EditorGUILayout.IntField(inventory[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save Inventory"))
        {
            CUBE.SetInventory(inventory);
        }
    }


    private void Builds()
    {
#if DEVMODE
        EditorGUILayout.LabelField("Dev Builds");
#else
        EditorGUILayout.LabelField("User Builds");
#endif

        for (int i = 0; i < builds.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // number
                EditorGUILayout.LabelField(i + ")", GUILayout.Width(25f));
                // build name
                string oldName = builds[i];
                EditorGUI.BeginChangeCheck();
                {
                    builds[i] = EditorGUILayout.TextField(builds[i]);
                }
                // rename
                if (EditorGUI.EndChangeCheck())
                {
                    ConstructionGrid.RenameBuild(oldName, builds[i]);
                }
                // delete
                if (GUILayout.Button("-", EditorStyles.miniButtonRight))
                {
                    ConstructionGrid.DeleteBuild(builds[i]);
                    builds.RemoveAt(i);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("New Build"))
        {
            ConstructionGrid.SaveBuild("", BuildInfo.Empty);
            builds.Add("");
        }
    }

    #endregion
}