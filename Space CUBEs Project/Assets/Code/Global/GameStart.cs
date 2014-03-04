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
        ConstructionGrid.SaveBuild("Test Build", "Test Build|31|31|34|28|2/(4.0, 5.0, 4.0)/(0.0, 0.0, 0.0)/-1/12~30~|16/(6.0, 5.0, 4.0)/(0.0, 0.0, 0.0)/-1/17~|16/(3.0, 5.0, 4.0)/(0.0, 0.0, 180.0)/-1/17~|7/(5.0, 4.0, 3.0)/(0.0, 0.0, 180.0)/-1/12~29~|12/(7.0, 4.0, 3.0)/(90.0, 180.0, 0.0)/-1/14~20~|12/(2.0, 4.0, 3.0)/(90.0, 180.0, 0.0)/-1/14~20~|100/(3.0, 4.0, 5.0)/(0.0, 0.0, 180.0)/0/7~27~|101/(6.0, 4.0, 5.0)/(0.0, 0.0, 180.0)/1/17~23~|103/(5.0, 4.0, 5.0)/(0.0, 0.0, 180.0)/2/14~25~21~|104/(7.0, 6.0, 4.0)/(0.0, 0.0, 90.0)/3/11~20~|111/(2.0, 6.0, 4.0)/(0.0, 0.0, 90.0)/-1/11~|115/(2.0, 6.0, 5.0)/(270.0, 180.0, 0.0)/-1/20~|");
    }

    #endregion

}