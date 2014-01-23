// Steve Yeager
// 1.15.2014

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Runs all methods needed at game start.
public class GameStart : MonoBehaviour
{
    #region Public Fields

    public int version;
    public bool resetVersion;

    #endregion

    #region Readonly Fields

    private static readonly List<Action> startActions = new List<Action> { Version1 };

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
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
    }

    #endregion

}