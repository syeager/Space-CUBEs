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
        ConstructionGrid.SaveBuild("Test Build", "Test Build|35|35|46|34|2|(4.0, 5.0, 4.0)|(0.0, 0.0, 0.0)|-1|9|(4.0, 3.0, 2.0)|(0.0, 0.0, 0.0)|-1|13|(4.0, 4.0, 5.0)|(90.0, 0.0, 0.0)|-1|13|(5.0, 4.0, 5.0)|(90.0, 0.0, 0.0)|-1|16|(7.0, 5.0, 4.0)|(0.0, 0.0, 0.0)|-1|16|(2.0, 5.0, 4.0)|(0.0, 0.0, 180.0)|-1|111|(6.0, 5.0, 4.0)|(0.0, 0.0, 0.0)|-1|111|(6.0, 5.0, 5.0)|(0.0, 0.0, 0.0)|-1|111|(3.0, 5.0, 5.0)|(0.0, 0.0, 0.0)|-1|111|(3.0, 5.0, 4.0)|(0.0, 0.0, 0.0)|-1|113|(4.0, 5.0, 6.0)|(0.0, 270.0, 0.0)|-1|113|(5.0, 5.0, 6.0)|(0.0, 270.0, 0.0)|-1|114|(3.0, 5.0, 6.0)|(0.0, 270.0, 0.0)|-1|114|(6.0, 5.0, 6.0)|(0.0, 0.0, 0.0)|-1|115|(6.0, 5.0, 3.0)|(270.0, 0.0, 0.0)|-1|115|(3.0, 5.0, 3.0)|(270.0, 0.0, 0.0)|-1|100|(6.0, 6.0, 5.0)|(0.0, 0.0, 270.0)|0|101|(3.0, 6.0, 5.0)|(0.0, 0.0, 270.0)|1|104|(7.0, 6.0, 4.0)|(0.0, 0.0, 270.0)|2|103|(4.0, 3.0, 5.0)|(0.0, 0.0, 270.0)|3|");
    }

    #endregion

}