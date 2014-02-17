// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Runs all methods needed at game start.
/// </summary>
public class GameStart : MonoBehaviour
{
    #region Public Fields

    public int version;
    public bool resetVersion;

    #endregion

    #region Readonly Fields

    private static readonly List<Action> startActions = new List<Action> { Version1 };

    #endregion

    #region Const Fields

    private const string DEFAULTBUILDNAME = "Alphie";
    private const string DEFAULTBUILD = "";

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        LoadGame();
        UpdateVersions();
    }

    #endregion


    #region Private Methods

    private void LoadGame()
    {
        CUBE.LoadAllCUBEInfo();
    }


    private void UpdateVersions()
    {
        int previousVersion = resetVersion ? 0 : PlayerPrefs.GetInt("Build Version");
        while (previousVersion < version)
        {
            Debugger.Log("GameStart: " + previousVersion, null, true, Debugger.LogTypes.Data);
            startActions[previousVersion].Invoke();
            previousVersion++;
        }
        PlayerPrefs.SetInt("Build Version", version);
    }


    private static void Version1()
    {
        // initial inventory
        int CUBECount = CUBE.allCUBES.Length;

        int[] inventory = new int[CUBECount];
        for (int i = 0; i < CUBECount; i++)
        {
            inventory[i] = 10;
        }

        CUBE.SetInventory(inventory);

        // initial bank
        MoneyManager.SetBalance(10000);

        // default ship build
        ConstructionGrid.SaveBuild("Test Build", "Test Build|14|14|37|9|0|(4.0, 5.0, 4.0)|(0.0, 0.0, 0.0)|-1|5|(2.0, 5.0, 2.0)|(0.0, 0.0, 0.0)|-1|5|(4.0, 5.0, 2.0)|(0.0, 0.0, 0.0)|-1|5|(5.0, 5.0, 2.0)|(0.0, 0.0, 0.0)|-1|5|(7.0, 5.0, 2.0)|(0.0, 0.0, 0.0)|-1|5|(5.0, 4.0, 2.0)|(0.0, 0.0, 0.0)|-1|5|(4.0, 4.0, 2.0)|(0.0, 0.0, 0.0)|-1|15|(6.0, 5.0, 4.0)|(0.0, 0.0, 0.0)|-1|15|(3.0, 5.0, 4.0)|(0.0, 0.0, 180.0)|-1|100|(3.0, 6.0, 5.0)|(0.0, 0.0, 180.0)|0|100|(6.0, 6.0, 5.0)|(0.0, 0.0, 180.0)|2|101|(8.0, 4.0, 4.0)|(0.0, 0.0, 180.0)|1|102|(1.0, 4.0, 4.0)|(0.0, 0.0, 180.0)|3|106|(3.0, 5.0, 3.0)|(0.0, 0.0, 180.0)|-1|106|(6.0, 5.0, 3.0)|(0.0, 0.0, 180.0)|-1|");
    }

    #endregion

}