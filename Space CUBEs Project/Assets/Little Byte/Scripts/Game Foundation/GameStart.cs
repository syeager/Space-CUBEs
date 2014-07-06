// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.15
// Edited: 2014.06.20

using System;
using System.Collections.Generic;
using Annotations;
using LittleByte.Data;

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

    public static readonly string[] DevBuilds =
    {
        "Avenger",
        "Berserker"
    };

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    protected override void Awake()
    {
        base.Awake();
        if (!enabled) return;

        LoadGame();
        UpdateVersions(resetVersion);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reset"></param>
    public void UpdateVersions(bool reset)
    {
        int previousVersion = reset ? 0 : SaveData.Load<int>("Version", @"Build/");
        while (previousVersion < version)
        {
            Debugger.Log("GameStart: " + previousVersion, gameObject, Debugger.LogTypes.Data);
            StartActions[previousVersion].Invoke();
            previousVersion++;
        }
        SaveData.Save("Version", version, @"Build/");
    }

    #endregion

    #region Private Methods

    private static void LoadGame()
    {
        GameTime.Initialize();
        CUBE.LoadAllCUBEInfo();
    }


    private static void Version1()
    {
        // initial inventory
        int count = CUBE.AllCUBES.Length;

        var inventory = new int[count];
        for (int i = 0; i < count; i++)
        {
            inventory[i] = 100;
        }

        CUBE.SetInventory(inventory);

        // default ship build
        foreach (string build in DevBuilds)
        {
            ConstructionGrid.SaveBuild(build, SaveData.LoadFromResources<BuildInfo>(build));
        }
    }

    #endregion
}