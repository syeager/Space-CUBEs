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
        for (int i = 0; i < 10; i++)
        ConstructionGrid.SaveBuild(DEFAULTBUILDNAME + i, DEFAULTBUILD);
    }

    #endregion

}