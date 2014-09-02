﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.30
// Edited: 2014.08.31

using System.Collections.Generic;
using System.Linq;
using LittleByte.NGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoreManager))]
public class StoreManagerInspector : Editor
{
    #region Private Fields

    private StoreManager manager;
    private StoreItem prefab;
    private CUBEInfo[] allInfo;

    #endregion

    #region Const Fields

    private const string Description = "Click for Descr";
    private const string Stats = "Click for Stats";

    #endregion

    #region Editor Overrides

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10f);
        GUIStyle style = GUI.skin.button;
        style.alignment = TextAnchor.MiddleCenter;
        if (GUILayout.Button("Create Item Buttons", style))
        {
            manager = (StoreManager)target;
            allInfo = CUBE.LoadAllCUBEInfo();
            Clear();
            CreateButtons();
        }
    }

    #endregion

    #region Private Methods

    private void CreateButtons()
    {
        EditorUtility.SetDirty(target);

        prefab = manager.buttonPrefab;

        // CUBEs
        CreateCUBEItems(0, allInfo.Where(c => c.type == CUBE.Types.System), Stats, true);
        CreateCUBEItems(1, allInfo.Where(c => c.type == CUBE.Types.Hull), Description);
        CreateCUBEItems(2, allInfo.Where(c => c.type == CUBE.Types.Weapon), Description);
        CreateCUBEItems(3, allInfo.Where(c => c.type == CUBE.Types.Augmentation), Description);

        // upgrades
        CreateUpgradeItems();
    }


    private void CreateCUBEItems(int index, IEnumerable<CUBEInfo> cubeInfo, string infoText, bool enabled = false)
    {
        manager.itemGrids[index].SetActive(true);
        UIGrid grid = manager.itemGrids[index].GetComponentInChildren<UIGrid>();
        foreach (CUBEInfo info in cubeInfo)
        {
            CreateButton(grid, info.name, StoreManager.ItemTypes.CUBE, info.ID, infoText, info.price, info.cost);
        }
        grid.GetComponent<UIGrid>().Reposition();
        manager.itemGrids[index].SetActive(enabled);
    }


    private void CreateUpgradeItems()
    {
        UIGrid grid = manager.itemGrids[4].GetComponentInChildren<UIGrid>();

        // core
        const string coreName = "Core ";
        for (int i = 0; i < BuildStats.CoreCapacities.Length; i++)
        {
            CreateButton(grid, coreName + (i + 1), StoreManager.ItemTypes.Core, i, Description, BuildStats.CorePrices[i], 0);
        }

        // weapon exp
        const string weaponExpName = "Weapon Exp ";
        for (int i = 0; i < BuildStats.WeaponExpansions.Length; i++)
        {
            CreateButton(grid, weaponExpName + (i + 1), StoreManager.ItemTypes.Weapon, i,  Description, BuildStats.WeaponPrices[i], 0);
        }

        // aug exp
        const string augExpName = "Aug Exp ";
        for (int i = 0; i < BuildStats.AugmentationExpansions.Length; i++)
        {
            CreateButton(grid, augExpName + (i + 1), StoreManager.ItemTypes.Augmentation, i,  Description, BuildStats.AugmentationPrices[i], 0);
        }

        manager.itemGrids[4].SetActive(false);
    }


    private void CreateButton(UIGrid parent, string name, StoreManager.ItemTypes itemType, int id, string infoText, int price, int cp)
    {
        StoreItem button = PrefabUtility.InstantiatePrefab(prefab) as StoreItem;
        parent.AddChild(button.transform);
        button.transform.localScale = Vector3.one;
        button.Initialize(itemType, id, name, infoText, price, cp);
        button.GetComponent<SelectableButton>().group = 0;
    }


    private void Clear()
    {
        EditorUtility.SetDirty(target);

        foreach (GameObject itemGrid in manager.itemGrids)
        {
            itemGrid.SetActive(true);
            Transform grid = itemGrid.GetComponentInChildren<UIGrid>().transform;
            int childCount = grid.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(grid.GetChild(0).gameObject);
            }
        }
    }

    #endregion
}