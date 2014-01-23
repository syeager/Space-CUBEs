// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Edit saved data.
/// </summary>
public class SavedDataTool : EditorWindow
{
    #region Fields

    private static int bank;
    private static CUBEInfo[] info;
    private static int[] inventory;
    private int setAll;

    #endregion


    #region EditorWindow

    [MenuItem("Tools/Saved Data Tool")]
    private static void Init()
    {
        SavedDataTool window = EditorWindow.GetWindow<SavedDataTool>(false, "Saved Data Tool");
        bank = MoneyManager.Balance();
        info = CUBE.LoadAllCUBEInfo();
        inventory = CUBE.GetInventory();
    }


    private void OnGUI()
    {
        Money();
        Inventory();
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