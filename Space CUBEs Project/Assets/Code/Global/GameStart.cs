// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.15
// Edited: 2014.05.31

using System;
using System.Collections.Generic;
using Annotations;
using UnityEngine;

/// <summary>
/// Runs all methods needed at game start.
/// </summary>
public class GameStart : Singleton<GameStart>
{
    #region Public Fields

    public int version;
    public bool resetVersion;

    #endregion

    #region Readonly Fields

    private static readonly List<Action> StartActions = new List<Action> {Version1};

    private static readonly Dictionary<string, string> DevBuilds = new Dictionary<string, string>
    {
        {
            "Avenger",
            "Avenger|36|22|41|7|2/(4.0, 5.0, 4.0)/(0.0, 0.0, 0.0)/-1/-1/1~30~|104/(5.0, 4.0, 5.0)/(0.0, 0.0, 0.0)/3/-1/29~1~|100/(6.0, 4.0, 6.0)/(0.0, 0.0, 0.0)/0/-1/29~0~|27/(5.0, 5.0, 3.0)/(0.0, 180.0, 0.0)/-1/-1/0~29~|59/(7.0, 5.0, 5.0)/(0.0, 180.0, 180.0)/-1/-1/0~|59/(2.0, 5.0, 5.0)/(0.0, 180.0, 0.0)/-1/-1/0~|26/(6.0, 4.0, 3.0)/(0.0, 180.0, 270.0)/-1/-1/1~29~|26/(3.0, 4.0, 3.0)/(0.0, 180.0, 270.0)/-1/-1/1~29~|108/(3.0, 6.0, 4.0)/(0.0, 0.0, 0.0)/-1/0/27~0~31~|110/(6.0, 6.0, 4.0)/(0.0, 180.0, 0.0)/-1/1/17~26~|101/(3.0, 4.0, 6.0)/(0.0, 0.0, 0.0)/1/-1/29~0~|105/(4.0, 4.0, 4.0)/(0.0, 0.0, 180.0)/2/-1/29~1~|111/(6.0, 5.0, 5.0)/(0.0, 0.0, 180.0)/-1/-1/25~|111/(6.0, 5.0, 4.0)/(0.0, 0.0, 180.0)/-1/-1/25~|111/(3.0, 5.0, 4.0)/(0.0, 90.0, 0.0)/-1/-1/25~|111/(3.0, 5.0, 5.0)/(0.0, 90.0, 0.0)/-1/-1/25~|16/(7.0, 4.0, 4.0)/(0.0, 0.0, 0.0)/-1/-1/30~|16/(2.0, 4.0, 4.0)/(0.0, 0.0, 180.0)/-1/-1/30~|"
        },
    };

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    protected override void Awake()
    {
        base.Awake();
        if (!enabled) return;

        GameTime.Initialize();
        LoadGame();
        UpdateVersions();
    }

    #endregion

    #region Private Methods

    private static void LoadGame()
    {
        CUBE.LoadAllCUBEInfo();
    }


    private void UpdateVersions()
    {
        int previousVersion = resetVersion ? 0 : PlayerPrefs.GetInt("Build Version");
        while (previousVersion < version)
        {
            Debugger.Log("GameStart: " + previousVersion, gameObject, Debugger.LogTypes.Data, true);
            StartActions[previousVersion].Invoke();
            previousVersion++;
        }
        PlayerPrefs.SetInt("Build Version", version);
    }


    private static void Version1()
    {
        // initial inventory
        int count = CUBE.allCUBES.Length;

        var inventory = new int[count];
        for (int i = 0; i < count; i++)
        {
            inventory[i] = 100;
        }

        CUBE.SetInventory(inventory);

        // initial bank
        MoneyManager.SetBalance(10000);

        // default ship build
        foreach (var build in DevBuilds)
        {
            ConstructionGrid.SaveBuild(build.Key, build.Value);
        }
    }

    #endregion
}