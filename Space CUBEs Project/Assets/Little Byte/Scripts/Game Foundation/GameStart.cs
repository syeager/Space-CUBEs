// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.15
// Edited: 2014.09.09

using System;
using System.Collections.Generic;
using Annotations;
using LittleByte.Data;
using LittleByte.GooglePlay;
using SpaceCUBEs;

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

    #endregion

    #region Const Fields

    private const string APIKey = "AIzaSyBJJPlSfdMRPvQkmUsqO-4EznFrL1YxgaU"; 

    #endregion

    #region Events

    public static event Action GameStartedEvent;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    protected override void Awake()
    {
        base.Awake();
        if (!enabled) return;

        LoadGame();
        UpdateVersions(resetVersion);

        if (GameStartedEvent != null)
        {
            GameStartedEvent.Invoke();
        }
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
        ConstructionGrid.Load();
        GoogleProfilePic.SetKey(APIKey);
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
        foreach (string build in ConstructionGrid.DevBuilds)
        {
            ConstructionGrid.SaveBuild(build, SaveData.LoadFromResources<BuildInfo>(build));
        }
    }

    #endregion
}